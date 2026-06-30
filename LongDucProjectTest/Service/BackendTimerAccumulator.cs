using LongDucProjectTest.ServiceReference1;
using Hino.DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace LongDucProjectTest.Service
{
    public class BackendTimerAccumulator
    {
        private static readonly BackendTimerAccumulator _instance = new BackendTimerAccumulator();
        public static BackendTimerAccumulator Instance => _instance;

        private Thread _workerThread;
        private bool _isRunning;
        private readonly string _connStr = "Server=localhost;Database=scada;Uid=root;Pwd=101101;";

        // Cache for previous PLC timer values
        private readonly Dictionary<int, double> _prevTimerValues = new Dictionary<int, double>();
        private int _lastRunId = -1;
        private bool _wasPaused = false;

        private readonly Dictionary<int, string> _stepTags = new Dictionary<int, string>
        {
            { 1, "AFChemTX01.ThoiGianCapLieu" },
            { 2, "AFChemTX01.ThoiGianTron1" },
            { 3, "AFChemTX01.ThoiGianXaDay" },
            { 4, "AFChemTX01.ThoiGianRungXaDay" },
            { 5, "AFChemTX01.ThoiGianHutXaDay" },
            { 6, "AFChemTX01.ThoiGianTron2" },
            { 7, "AFChemTX01.ThoiGianXaHang" },
            { 8, "AFChemTX01.ThoiGianRungXaHang" }
        };

        private BackendTimerAccumulator()
        {
            // Initialize cache
            foreach (var key in _stepTags.Keys)
            {
                _prevTimerValues[key] = 0;
            }
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            _workerThread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = "BackendTimerAccumulatorWorker"
            };
            _workerThread.Start();
            System.Diagnostics.Debug.WriteLine("[BackendTimerAccumulator] Service started.");
        }

        public void Stop()
        {
            _isRunning = false;
            if (_workerThread != null && _workerThread.IsAlive)
            {
                _workerThread.Join(1000);
            }
            System.Diagnostics.Debug.WriteLine("[BackendTimerAccumulator] Service stopped.");
        }

        private void WorkerLoop()
        {
            // Ensure table is created at startup
            try
            {
                var connector = new MySQLConnect { ConnectionString = _connStr };
                EnsureTableCreated(connector);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendTimerAccumulator] Initial table creation check failed: {ex.Message}");
            }

            while (_isRunning)
            {
                try
                {
                    var connector = new MySQLConnect { ConnectionString = _connStr };
                    var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, null, null);
                    int activeRunId = resolution.RunId;

                    if (activeRunId > 0)
                    {
                        var dtRun = connector.ExecuteQuery($"SELECT status, is_paused FROM runs WHERE id = {activeRunId} LIMIT 1");
                        if (dtRun != null && dtRun.Rows.Count > 0)
                        {
                            string status = dtRun.Rows[0]["status"].ToString().Trim();
                            bool isActive = status.Equals("Active", StringComparison.OrdinalIgnoreCase);
                            bool isPaused = Convert.ToInt32(dtRun.Rows[0]["is_paused"]) == 1;

                            if (isActive)
                            {
                                ProcessAccumulation(connector, activeRunId, isPaused);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[BackendTimerAccumulator] Error in worker loop: {ex.Message}");
                }

                Thread.Sleep(500); // Poll every 500ms
            }
        }

        private void ProcessAccumulation(MySQLConnect connector, int runId, bool isPaused)
        {
            // 1. Detect run switch or service startup for this run
            if (_lastRunId != runId)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendTimerAccumulator] Run switched from {_lastRunId} to {runId}. Re-seeding baseline.");
                _lastRunId = runId;
                _wasPaused = isPaused;

                // Ensure rows exist in database
                InitializeDatabaseRows(connector, runId);

                // Read current PLC values to seed baseline
                var currentPlcValues = ReadPlcTimerValues();
                foreach (var stepCode in _stepTags.Keys)
                {
                    double currentVal = currentPlcValues.ContainsKey(stepCode) ? currentPlcValues[stepCode] : 0;
                    // If the run just switched and the value is small (e.g. < 15s), it means the step has just started,
                    // so we should accumulate this initial value. Set baseline to 0.
                    // Otherwise, if it's large, it might be a residual value or the run was restarted mid-way, so we seed with currentVal.
                    if (currentVal < 15)
                    {
                        _prevTimerValues[stepCode] = 0;
                    }
                    else
                    {
                        _prevTimerValues[stepCode] = currentVal;
                    }
                }
                return;
            }

            // 2. Handle Pause state
            if (isPaused)
            {
                // If paused, we do not calculate delta, just flag that we were paused
                _wasPaused = true;
                return;
            }

            // 3. Handle Resume state
            bool isResume = _wasPaused && !isPaused;
            _wasPaused = false;

            var plcValues = ReadPlcTimerValues();

            foreach (var stepCode in _stepTags.Keys)
            {
                if (!plcValues.ContainsKey(stepCode)) continue;

                double tNew = plcValues[stepCode];
                double tPrev = _prevTimerValues[stepCode];
                double delta = 0;

                if (isResume)
                {
                    // On resume, PLC resets timer back to 0 (or a small start value).
                    // We set T_prev to this value and do not add any delta for this transition cycle.
                    _prevTimerValues[stepCode] = tNew;
                    System.Diagnostics.Debug.WriteLine($"[BackendTimerAccumulator] Step {stepCode} resume detected. Baseline reset to {tNew}.");
                    continue;
                }

                if (tNew < tPrev)
                {
                    // Fallback reset check: if values dropped even without explicit paused transition
                    delta = tNew;
                }
                else
                {
                    delta = tNew - tPrev;
                }

                _prevTimerValues[stepCode] = tNew;

                if (delta > 0)
                {
                    // Update accumulated time in database
                    connector.ExecuteNonQuery($"UPDATE run_step_accumulated_times SET accumulatedTime = accumulatedTime + {delta} WHERE runId = {runId} AND stepCode = {stepCode}");
                }
            }
        }

        private void InitializeDatabaseRows(MySQLConnect connector, int runId)
        {
            EnsureTableCreated(connector);
            foreach (var stepCode in _stepTags.Keys)
            {
                var dt = connector.ExecuteQuery($"SELECT COUNT(*) as cnt FROM run_step_accumulated_times WHERE runId = {runId} AND stepCode = {stepCode}");
                if (dt == null || dt.Rows.Count == 0 || Convert.ToInt32(dt.Rows[0]["cnt"]) == 0)
                {
                    connector.ExecuteNonQuery($"INSERT IGNORE INTO run_step_accumulated_times (runId, stepCode, accumulatedTime) VALUES ({runId}, {stepCode}, 0)");
                }
            }
        }

        private void EnsureTableCreated(MySQLConnect connector)
        {
            try
            {
                string createTableSql = @"
                    CREATE TABLE IF NOT EXISTS `run_step_accumulated_times` (
                      `runId` INT(11) NOT NULL,
                      `stepCode` INT(11) NOT NULL,
                      `accumulatedTime` DOUBLE NOT NULL DEFAULT 0,
                      PRIMARY KEY (`runId`, `stepCode`),
                      FOREIGN KEY (`runId`) REFERENCES `runs`(`id`) ON DELETE CASCADE
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
                connector.ExecuteNonQuery(createTableSql);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendTimerAccumulator] Error ensuring table created: {ex.Message}");
            }
        }

        private Dictionary<int, double> ReadPlcTimerValues()
        {
            var values = new Dictionary<int, double>();
            try
            {
                string[] names = _stepTags.Values.ToArray();
                var results = RealtimeService.Instance.Read(names);
                if (results != null)
                {
                    foreach (var res in results)
                    {
                        if (res == null || string.IsNullOrEmpty(res.Name)) continue;
                        
                        var stepEntry = _stepTags.FirstOrDefault(x => x.Value.Equals(res.Name, StringComparison.OrdinalIgnoreCase));
                        if (stepEntry.Key > 0)
                        {
                            if (double.TryParse(res.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                            {
                                values[stepEntry.Key] = val;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackendTimerAccumulator] Error reading PLC: {ex.Message}");
            }
            return values;
        }
    }
}
