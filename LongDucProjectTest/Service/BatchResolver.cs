﻿﻿﻿﻿using System;
using System.Data;
using System.Globalization;
using Hino.DatabaseConnector;

namespace LongDucProject.Helpers
{
    public class BatchRunResolution
    {
        public int BatchId { get; set; }
        public int RunId { get; set; }
    }

    public static class BatchResolver
    {
        /// <summary>
        /// Resolves the batch ID and run ID based on optional parameters and overall priority rules.
        /// Priority:
        /// 1. Specific run ID if provided -> resolve corresponding batch ID.
        /// 2. Specific batch ID if provided -> resolve run ID (Active run -> Last Completed run -> First Run of Batch).
        /// 3. Date filter if provided -> get latest batch of that date -> resolve run ID.
        /// 4. Fallback overall priority (Active Run/Batch -> Latest Completed Run -> Today's Pending Batch first run -> Any Pending Batch first run -> Ultimate Fallback first run).
        /// </summary>
        public static BatchRunResolution Resolve(MySQLConnect connector, string batchIdStr, string runIdStr, string dateStr = null)
        {
            int selectedBatchId = -1;
            int selectedRunId = -1;

            // 1. Resolve runId parameter first
            if (!string.IsNullOrEmpty(runIdStr) && int.TryParse(runIdStr, out int rId) && rId > 0)
            {
                selectedRunId = rId;
                var dtRun = connector.ExecuteQuery($"SELECT batch_id FROM runs WHERE id = {selectedRunId} LIMIT 1");
                if (dtRun != null && dtRun.Rows.Count > 0)
                {
                    selectedBatchId = Convert.ToInt32(dtRun.Rows[0]["batch_id"]);
                }
            }

            // 2. Resolve batchId parameter if not set by runId
            if (selectedBatchId <= 0 && !string.IsNullOrEmpty(batchIdStr) && int.TryParse(batchIdStr, out int bId) && bId > 0)
            {
                selectedBatchId = bId;
            }

            // 3. If batchId is still not resolved, try date filter or fallback to priority logic
            if (selectedBatchId <= 0)
            {
                string dateFilter = "";
                if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParseExact(dateStr, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    dateFilter = $"WHERE DATE(start_time) = '{parsedDate.ToString("yyyy-MM-dd")}'";
                }

                if (!string.IsNullOrEmpty(dateFilter))
                {
                    // Get latest batch of that date
                    var dtLatestDate = connector.ExecuteQuery($"SELECT id FROM batches {dateFilter} ORDER BY id DESC LIMIT 1");
                    if (dtLatestDate != null && dtLatestDate.Rows.Count > 0)
                    {
                        selectedBatchId = Convert.ToInt32(dtLatestDate.Rows[0]["id"]);
                    }
                    else
                    {
                        selectedBatchId = -2; // Date filter applied but no batch found
                    }
                }

                // If selectedBatchId is -1, it means no date filter or it wasn't matched, so apply global fallback logic
                if (selectedBatchId == -1)
                {
                    // Priority 1: Active run
                    var dtActiveRun = connector.ExecuteQuery("SELECT id, batch_id FROM runs WHERE status = 'Active' ORDER BY id DESC LIMIT 1");
                    if (dtActiveRun != null && dtActiveRun.Rows.Count > 0)
                    {
                        selectedRunId = Convert.ToInt32(dtActiveRun.Rows[0]["id"]);
                        selectedBatchId = Convert.ToInt32(dtActiveRun.Rows[0]["batch_id"]);
                    }
                    else
                    {
                        // Check if there is an Active batch
                        var dtActiveBatch = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Active' LIMIT 1");
                        if (dtActiveBatch != null && dtActiveBatch.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtActiveBatch.Rows[0]["id"]);
                            // Find the first run of this active batch
                            var dtRunOfActive = connector.ExecuteQuery($"SELECT id FROM runs WHERE batch_id = {selectedBatchId} ORDER BY id ASC LIMIT 1");
                            if (dtRunOfActive != null && dtRunOfActive.Rows.Count > 0)
                            {
                                selectedRunId = Convert.ToInt32(dtRunOfActive.Rows[0]["id"]);
                            }
                        }
                    }

                    // Priority 2: Completed run with latest end_time
                    if (selectedRunId == -1)
                    {
                        var dtDoneRun = connector.ExecuteQuery("SELECT id, batch_id FROM runs WHERE status = 'Completed' ORDER BY end_time DESC, id DESC LIMIT 1");
                        if (dtDoneRun != null && dtDoneRun.Rows.Count > 0)
                        {
                            selectedRunId = Convert.ToInt32(dtDoneRun.Rows[0]["id"]);
                            selectedBatchId = Convert.ToInt32(dtDoneRun.Rows[0]["batch_id"]);
                        }
                    }

                    // Priority 3: Pending batch of today, selecting the first run
                    if (selectedRunId == -1)
                    {
                        var dtPendingBatch = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Pending' AND (DATE(created_at) = CURDATE() OR DATE(start_time) = CURDATE()) ORDER BY id ASC LIMIT 1");
                        if (dtPendingBatch != null && dtPendingBatch.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtPendingBatch.Rows[0]["id"]);
                        }
                        else
                        {
                            // General pending batch fallback
                            var dtAnyPendingBatch = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Pending' ORDER BY id ASC LIMIT 1");
                            if (dtAnyPendingBatch != null && dtAnyPendingBatch.Rows.Count > 0)
                            {
                                selectedBatchId = Convert.ToInt32(dtAnyPendingBatch.Rows[0]["id"]);
                            }
                        }

                        if (selectedBatchId != -1)
                        {
                            // First run of pending batch
                            var dtFirstRun = connector.ExecuteQuery($"SELECT id FROM runs WHERE batch_id = {selectedBatchId} ORDER BY id ASC LIMIT 1");
                            if (dtFirstRun != null && dtFirstRun.Rows.Count > 0)
                            {
                                selectedRunId = Convert.ToInt32(dtFirstRun.Rows[0]["id"]);
                            }
                        }
                    }

                    // Ultimate Fallback: get latest batch and its first run
                    if (selectedBatchId == -1)
                    {
                        var dtLatestBatch = connector.ExecuteQuery("SELECT id FROM batches ORDER BY id DESC LIMIT 1");
                        if (dtLatestBatch != null && dtLatestBatch.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtLatestBatch.Rows[0]["id"]);
                            var dtFirstRun = connector.ExecuteQuery($"SELECT id FROM runs WHERE batch_id = {selectedBatchId} ORDER BY id ASC LIMIT 1");
                            if (dtFirstRun != null && dtFirstRun.Rows.Count > 0)
                            {
                                selectedRunId = Convert.ToInt32(dtFirstRun.Rows[0]["id"]);
                            }
                        }
                    }
                }
            }

            // 4. If batchId resolved but runId is not, auto-resolve runId of this batch
            if (selectedBatchId > 0 && selectedRunId <= 0)
            {
                // Priority A: Active run in this batch
                var dtActiveRun = connector.ExecuteQuery($"SELECT id FROM runs WHERE batch_id = {selectedBatchId} AND status = 'Active' LIMIT 1");
                if (dtActiveRun != null && dtActiveRun.Rows.Count > 0)
                {
                    selectedRunId = Convert.ToInt32(dtActiveRun.Rows[0]["id"]);
                }
                else
                {
                    // Priority B: Last completed run in this batch (most recently completed)
                    var dtCompletedRun = connector.ExecuteQuery($"SELECT id FROM runs WHERE batch_id = {selectedBatchId} AND status = 'Completed' ORDER BY end_time DESC, id DESC LIMIT 1");
                    if (dtCompletedRun != null && dtCompletedRun.Rows.Count > 0)
                    {
                        selectedRunId = Convert.ToInt32(dtCompletedRun.Rows[0]["id"]);
                    }
                    else
                    {
                        // Priority C: First run of this batch (cũ nhất)
                        var dtFirstRun = connector.ExecuteQuery($"SELECT id FROM runs WHERE batch_id = {selectedBatchId} ORDER BY id ASC LIMIT 1");
                        if (dtFirstRun != null && dtFirstRun.Rows.Count > 0)
                        {
                            selectedRunId = Convert.ToInt32(dtFirstRun.Rows[0]["id"]);
                        }
                    }
                }
            }

            return new BatchRunResolution
            {
                BatchId = selectedBatchId,
                RunId = selectedRunId
            };
        }
    }
}
