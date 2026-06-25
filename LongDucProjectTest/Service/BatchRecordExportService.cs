using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Hino.DatabaseConnector;
using OfficeOpenXml;

namespace LongDucProjectTest.Service
{
    public class BatchRecordExportService : IBatchRecordExportService
    {
        private readonly MySQLConnect _connector;
        private readonly string _templatePath;

        public BatchRecordExportService(MySQLConnect connector, string templatePath)
        {
            _connector = connector;
            _templatePath = templatePath;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public byte[] ExportBatchRecord(int batchId, out string fileName)
        {
            if (!File.Exists(_templatePath))
            {
                throw new FileNotFoundException($"Không tìm thấy file mẫu báo cáo Nhật ký sản xuất tại: {_templatePath}");
            }

            // 1. Fetch Batch information
            var dtBatch = _connector.ExecuteQuery($"SELECT name, created_at, status, device_name, product_name, product_code, manufacturer, target_weight, formula, start_time, end_time FROM batches WHERE id = {batchId} LIMIT 1");
            if (dtBatch == null || dtBatch.Rows.Count == 0)
            {
                throw new KeyNotFoundException($"Không tìm thấy lô sản xuất (Batch) với ID: {batchId}");
            }

            var batchRow = dtBatch.Rows[0];
            string batchName = batchRow["name"] != DBNull.Value ? batchRow["name"].ToString() : "";
            DateTime createdAt = Convert.ToDateTime(batchRow["created_at"]);
            string batchStatus = batchRow["status"] != DBNull.Value ? batchRow["status"].ToString() : "";
            string deviceName = batchRow["device_name"] != DBNull.Value ? batchRow["device_name"].ToString() : "";
            string dbProductName = batchRow["product_name"] != DBNull.Value ? batchRow["product_name"].ToString() : "";
            string dbProductCode = batchRow["product_code"] != DBNull.Value ? batchRow["product_code"].ToString() : "";
            string manufacturer = batchRow["manufacturer"] != DBNull.Value ? batchRow["manufacturer"].ToString() : "";
            double dbTargetWeight = batchRow["target_weight"] != DBNull.Value ? Convert.ToDouble(batchRow["target_weight"]) : 0;
            string formula = batchRow["formula"] != DBNull.Value ? batchRow["formula"].ToString() : "";
            string startTimeStr = batchRow["start_time"] != DBNull.Value ? Convert.ToDateTime(batchRow["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "-";
            string endTimeStr = batchRow["end_time"] != DBNull.Value ? Convert.ToDateTime(batchRow["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "-";

            // 2. Fetch runs
            var dtRuns = _connector.ExecuteQuery($"SELECT id, name, run_number, status, start_time, end_time, sp_thoi_gian_cap_lieu, sp_thoi_gian_tron1, sp_thoi_gian_xa_day, sp_thoi_gian_rung_xa_day, sp_thoi_gian_hut_xa_day_them, sp_thoi_gian_tron2, sp_thoi_gian_xa_hang, sp_thoi_gian_rung_xa_hang FROM runs WHERE batch_id = {batchId} ORDER BY run_number ASC");
            var runsList = new List<RunDto>();
            if (dtRuns != null)
            {
                foreach (DataRow r in dtRuns.Rows)
                {
                    runsList.Add(new RunDto
                    {
                        Id = Convert.ToInt32(r["id"]),
                        Name = r["name"].ToString(),
                        RunNumber = Convert.ToInt32(r["run_number"]),
                        Status = r["status"].ToString(),
                        StartTime = r["start_time"] != DBNull.Value ? Convert.ToDateTime(r["start_time"]) : (DateTime?)null,
                        EndTime = r["end_time"] != DBNull.Value ? Convert.ToDateTime(r["end_time"]) : (DateTime?)null,
                        SpCapLieu = r["sp_thoi_gian_cap_lieu"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_cap_lieu"]) : 0,
                        SpTron1 = r["sp_thoi_gian_tron1"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_tron1"]) : 0,
                        SpXaDay = r["sp_thoi_gian_xa_day"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_xa_day"]) : 0,
                        SpRungXaDay = r["sp_thoi_gian_rung_xa_day"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_rung_xa_day"]) : 0,
                        SpHutXaDay = r["sp_thoi_gian_hut_xa_day_them"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_hut_xa_day_them"]) : 0,
                        SpTron2 = r["sp_thoi_gian_tron2"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_tron2"]) : 0,
                        SpXaHang = r["sp_thoi_gian_xa_hang"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_xa_hang"]) : 0,
                        SpRungXaHang = r["sp_thoi_gian_rung_xa_hang"] != DBNull.Value ? Convert.ToInt32(r["sp_thoi_gian_rung_xa_hang"]) : 0
                    });
                }
            }

            // 3. Fetch Webhook Log
            // Query by matching received_at with batches.created_at within 2 seconds window
            string dateStr = createdAt.ToString("yyyy-MM-dd HH:mm:ss");
            var dtWebhook = _connector.ExecuteQuery($"SELECT payload FROM webhook_logs WHERE received_at >= '{createdAt.AddSeconds(-2):yyyy-MM-dd HH:mm:ss}' AND received_at <= '{createdAt.AddSeconds(2):yyyy-MM-dd HH:mm:ss}' LIMIT 1");
            
            // Fallback: If no webhook log matched within window, get latest webhook log matching batch lot_no or product_code
            if (dtWebhook == null || dtWebhook.Rows.Count == 0)
            {
                // Simple search in payload for lot_no or batch name
                dtWebhook = _connector.ExecuteQuery($"SELECT payload FROM webhook_logs WHERE payload LIKE '%{batchName}%' ORDER BY id DESC LIMIT 1");
            }

            if (dtWebhook == null || dtWebhook.Rows.Count == 0)
            {
                // Secondary search: search in payload for product code and device name
                dtWebhook = _connector.ExecuteQuery($"SELECT payload FROM webhook_logs WHERE payload LIKE '%{dbProductCode}%' AND payload LIKE '%{deviceName}%' ORDER BY id DESC LIMIT 1");
            }


            var webhookFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (dtWebhook != null && dtWebhook.Rows.Count > 0)
            {
                string rawPayload = dtWebhook.Rows[0]["payload"].ToString();
                webhookFields = ParseUrlEncodedPayload(rawPayload);
            }

            // Extract values from Webhook or DB Fallback
            string productName = GetField(webhookFields, "custom_ten_hang_hoa", dbProductName);
            string productCode = GetField(webhookFields, "custom_ma_dinh_danh", dbProductCode);
            string lotNo = GetField(webhookFields, "custom_lotno", batchName);
            string workOrder = GetField(webhookFields, "custom_ke_hoach_san_xuat", "-");
            string prodDate = GetField(webhookFields, "custom_ngay_san_xuat", createdAt.ToString("dd/MM/yyyy"));
            string packingSpec = GetField(webhookFields, "custom_quy_cach", "-");
            string unitStr = GetField(webhookFields, "custom_don_vi_tinh", "kg");

            // Extract confirmation names and times from Webhook fields
            string operatorName = GetField(webhookFields, "custom_nguoi_van_hanh", GetField(webhookFields, "nguoi_van_hanh", ""));
            string supervisorName = "";
            string qcName = "";
            string managerName = "";

            if (webhookFields.ContainsKey("follower_list"))
            {
                try
                {
                    var followerObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(webhookFields["follower_list"]);
                    if (followerObj != null && followerObj.ContainsKey("name_titles"))
                    {
                        string nameTitles = followerObj["name_titles"];
                        var parts = nameTitles.Split(',');
                        foreach (var part in parts)
                        {
                            var trimmed = part.Trim();
                            int openParen = trimmed.IndexOf('(');
                            int closeParen = trimmed.IndexOf(')');
                            if (openParen > 0 && closeParen > openParen)
                            {
                                string name = trimmed.Substring(0, openParen).Trim();
                                string title = trimmed.Substring(openParen + 1, closeParen - openParen - 1).Trim();

                                string titleLower = title.ToLower();
                                if (titleLower.Contains("trưởng khối") || titleLower.Contains("quản lý") || titleLower.Contains("giám đốc"))
                                {
                                    managerName = name;
                                }
                                else if (titleLower.Contains("trưởng phòng qc") || titleLower.Contains("tổ trưởng") || titleLower.Contains("giám sát"))
                                {
                                    supervisorName = name;
                                }
                                else if (titleLower.Contains("nhân viên kế hoạch sản xuất - qc") || titleLower.Contains("nhân viên qc") || (qcName == "" && titleLower.Contains("qc")))
                                {
                                    qcName = name;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            // Fallback for QC name to creator
            if (string.IsNullOrEmpty(qcName) && webhookFields.ContainsKey("creator"))
            {
                try
                {
                    var creatorObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(webhookFields["creator"]);
                    if (creatorObj != null && creatorObj.ContainsKey("name"))
                    {
                        qcName = creatorObj["name"];
                    }
                }
                catch { }
            }

            // Extract confirmation times
            string operatorTime = "";
            string supervisorTime = "";
            string qcTime = "";
            string managerTime = "";

            if (webhookFields.ContainsKey("moves"))
            {
                try
                {
                    var movesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(webhookFields["moves"]);
                    if (movesList != null)
                    {
                        for (int i = 0; i < movesList.Count; i++)
                        {
                            var move = movesList[i];
                            string stageId = move.ContainsKey("stage_id") ? move["stage_id"]?.ToString() : "";
                            string endVal = move.ContainsKey("stage_end") ? move["stage_end"]?.ToString() : "";
                            if (!string.IsNullOrEmpty(endVal) && long.TryParse(endVal, out long unixTime) && unixTime > 0)
                            {
                                DateTime dt = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
                                string timeStr = dt.ToString("dd/MM/yyyy HH:mm");
                                
                                if (stageId == "104048" || i == 0)
                                {
                                    qcTime = timeStr;
                                    operatorTime = dt.ToString("dd/MM/yyyy");
                                }
                                else if (stageId == "104155" || i == 1)
                                {
                                    supervisorTime = timeStr;
                                }
                                else if (i == 2)
                                {
                                    managerTime = timeStr;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            if (string.IsNullOrEmpty(operatorTime)) operatorTime = prodDate;
            if (string.IsNullOrEmpty(qcTime)) qcTime = prodDate;
            if (string.IsNullOrEmpty(supervisorTime)) supervisorTime = prodDate;
            if (string.IsNullOrEmpty(managerTime)) managerTime = prodDate;
            double targetWeight = 0;
            if (webhookFields.ContainsKey("custom_khoi_luong_muc_tieu") && double.TryParse(webhookFields["custom_khoi_luong_muc_tieu"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double wVal))
            {
                targetWeight = wVal;
            }
            else
            {
                targetWeight = dbTargetWeight;
            }

            // Extract BOM items from webhook payload or fallback to DB run_info
            var bomItems = new List<BomItemDto>();
            bool hasWebhookBom = false;

            if (webhookFields.Count > 0)
            {
                // Map suffixes: mẻ 1 = a, mẻ 2 = b, mẻ 3 = c...
                char suffix = 'a';
                for (int runIdx = 1; runIdx <= Math.Max(runsList.Count, 5); runIdx++)
                {
                    string key = $"custom_thong_tin_bom_san_xuat_{suffix}";
                    if (webhookFields.ContainsKey(key))
                    {
                        var runBom = ParseBomBase64(webhookFields[key], runIdx);
                        if (runBom.Count > 0)
                        {
                            bomItems.AddRange(runBom);
                            hasWebhookBom = true;
                        }
                    }
                    suffix++;
                }
            }

            // Fallback BOM retrieval from run_info table
            if (!hasWebhookBom)
            {
                var dtRunInfo = _connector.ExecuteQuery($@"
                    SELECT ri.code, ri.material_code, ri.quantity, ri.unit, ri.batch_no, r.run_number
                    FROM run_info ri
                    JOIN runs r ON ri.run_id = r.id
                    WHERE r.batch_id = {batchId}
                    ORDER BY r.run_number ASC, ri.id ASC");
                if (dtRunInfo != null && dtRunInfo.Rows.Count > 0)
                {
                    int stt = 1;
                    foreach (DataRow row in dtRunInfo.Rows)
                    {
                        bomItems.Add(new BomItemDto
                        {
                            STT = stt++,
                            Code = row["code"] != DBNull.Value ? row["code"].ToString() : "",
                            MaterialCode = row["material_code"] != DBNull.Value ? row["material_code"].ToString() : "",
                            Unit = row["unit"] != DBNull.Value ? row["unit"].ToString() : "",
                            Quantity = row["quantity"] != DBNull.Value ? Convert.ToDouble(row["quantity"]) : 0,
                            ActualQuantity = row["quantity"] != DBNull.Value ? Convert.ToDouble(row["quantity"]) : 0, // Fallback actual as plan
                            BatchNo = row["batch_no"] != DBNull.Value ? row["batch_no"].ToString() : "",
                            Note = $"Mẻ {row["run_number"]}"
                        });
                    }
                }
            }

            // 4. EPPlus excel generation
            using (var stream = new FileStream(_templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var package = new ExcelPackage(stream))
            {
                var ws = package.Workbook.Worksheets["Sheet1"] ?? package.Workbook.Worksheets[0];

                // --- POPULATE SECTION 1: THÔNG TIN CHUNG ---
                ws.Cells["B5"].Value = productName;
                ws.Cells["E5"].Value = productCode;
                ws.Cells["H5"].Value = lotNo;

                ws.Cells["B6"].Value = workOrder;
                ws.Cells["E6"].Value = prodDate;
                ws.Cells["H6"].Value = "Ca 1"; // Default Shift

                ws.Cells["B7"].Value = deviceName;
                ws.Cells["E7"].Value = packingSpec;
                ws.Cells["H7"].Value = unitStr;

                ws.Cells["B8"].Value = targetWeight;
                
                // Calculate actual produced weight based on actual BOM weight minus allowable loss
                double totalBomWeight = 0;
                foreach (var item in bomItems)
                {
                    if (item.Unit != null && item.Unit.Trim().Equals("kg", StringComparison.OrdinalIgnoreCase))
                    {
                        totalBomWeight += item.ActualQuantity;
                    }
                }

                var completedRunsDict = new Dictionary<int, double>();
                var dtRunWeights = _connector.ExecuteQuery($@"
                    SELECT ri.run_id, SUM(ri.quantity) as run_weight 
                    FROM run_info ri 
                    JOIN runs r ON ri.run_id = r.id 
                    WHERE r.batch_id = {batchId}
                      AND LOWER(ri.unit) = 'kg'
                    GROUP BY ri.run_id");
                if (dtRunWeights != null)
                {
                    foreach (DataRow row in dtRunWeights.Rows)
                    {
                        int rId = Convert.ToInt32(row["run_id"]);
                        double w = row["run_weight"] != DBNull.Value ? Convert.ToDouble(row["run_weight"]) : 0;
                        completedRunsDict[rId] = w;
                    }
                }

                int validRunsCount = 0;
                double totalRunInfoWeight = 0;
                foreach (var run in runsList)
                {
                    if (!run.Status.Equals("Error", StringComparison.OrdinalIgnoreCase) && 
                        !run.Status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                    {
                        validRunsCount++;
                        double w = 0;
                        if (completedRunsDict.ContainsKey(run.Id))
                        {
                            w = completedRunsDict[run.Id];
                        }
                        else
                        {
                            w = targetWeight / (runsList.Count > 0 ? runsList.Count : 1);
                        }
                        totalRunInfoWeight += w;
                    }
                }

                if (totalBomWeight <= 0)
                {
                    totalBomWeight = totalRunInfoWeight;
                }

                double allowableLoss = totalRunInfoWeight - targetWeight;
                double totalActualProduced = totalBomWeight - allowableLoss;
                if (totalActualProduced < 0) totalActualProduced = 0;

                ws.Cells["E8"].Value = totalActualProduced;
                ws.Cells["H8"].Value = batchStatus;

                ws.Cells["B9"].Value = startTimeStr;
                ws.Cells["E9"].Value = endTimeStr;
                ws.Cells["H9"].Value = lotNo; // Retained sample code as lot_no

                // --- POPULATE SECTION 2: ĐẦU VÀO SỬ DỤNG CHO LÔ ---
                int baseBomRow = 13;
                int originalBomCount = 5; // Rows 13 to 17
                int currentShift = 0;

                if (bomItems.Count > originalBomCount)
                {
                    int rowsToInsert = bomItems.Count - originalBomCount;
                    ws.InsertRow(baseBomRow + originalBomCount, rowsToInsert);
                    
                    // Copy style from row 13 to new rows
                    for (int i = 0; i < rowsToInsert; i++)
                    {
                        CopyRowStyles(ws, baseBomRow, baseBomRow + originalBomCount + i);
                    }
                    currentShift = rowsToInsert;
                }

                // Fill BOM data
                for (int i = 0; i < bomItems.Count; i++)
                {
                    int r = baseBomRow + i;
                    var item = bomItems[i];
                    ws.Cells[r, 1].Value = i + 1; // STT
                    ws.Cells[r, 2].Value = item.Code;
                    ws.Cells[r, 3].Value = item.MaterialCode;
                    ws.Cells[r, 4].Value = item.Unit;
                    ws.Cells[r, 5].Value = item.Quantity;
                    ws.Cells[r, 6].Value = item.ActualQuantity;
                    ws.Cells[r, 7].Value = item.BatchNo;
                    ws.Cells[r, 8].Value = item.Note;

                    // Force uniform row height of 14
                    ws.Row(r).Height = 14;
                    ws.Row(r).CustomHeight = true;
                }

                // --- POPULATE SECTION 3: THÔNG SỐ QUÁ TRÌNH (RUNS) ---
                // Step definitions for SCADA 8 stages
                var stepDefs = new[]
                {
                    new { Code = 1, TagNo = "T001", Name = "Cấp liệu", Standard = "60s", Alias = "ThoiGianCapLieu" },
                    new { Code = 2, TagNo = "T002", Name = "Trộn 1", Standard = "50s", Alias = "ThoiGianTron1" },
                    new { Code = 3, TagNo = "T003", Name = "Xả đáy", Standard = "60s", Alias = "ThoiGianXaDay" },
                    new { Code = 4, TagNo = "T004", Name = "Rung xả đáy", Standard = "20s", Alias = "ThoiGianRungXaDay" },
                    new { Code = 5, TagNo = "T005", Name = "Hút xả đáy", Standard = "30s", Alias = "ThoiGianHutXaDay" },
                    new { Code = 6, TagNo = "T006", Name = "Trộn 2", Standard = "45s", Alias = "ThoiGianTron2" },
                    new { Code = 7, TagNo = "T007", Name = "Xả hàng", Standard = "100s", Alias = "ThoiGianXaHang" },
                    new { Code = 8, TagNo = "T008", Name = "Rung xả hàng", Standard = "30s", Alias = "ThoiGianRungXaHang" }
                };

                int bomShift = currentShift;
                int run1StartRow = 20 + bomShift;
                int run2StartRow = 30 + bomShift;

                if (runsList.Count == 1)
                {
                    // Delete Run 2 template block
                    ws.DeleteRow(run2StartRow, 9);
                    currentShift -= 9;
                }
                else if (runsList.Count > 2)
                {
                    // Duplicate Run 2 template block for Run 3, Run 4...
                    int sourceStartRow = run2StartRow;
                    for (int runIdx = 3; runIdx <= runsList.Count; runIdx++)
                    {
                        int insertAtRow = 39 + currentShift;
                        DuplicateRunBlock(ws, sourceStartRow, insertAtRow, runIdx);
                        currentShift += 9;
                    }
                }

                // Fill telemetry data for each Run
                for (int runIdx = 0; runIdx < runsList.Count; runIdx++)
                {
                    var run = runsList[runIdx];
                    int blockStartRow = GetRunBlockStartRow(runIdx + 1, bomShift);
                    
                    // Fetch alarm log for time range of stages
                    var dtAlarmLog = _connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Status, TagNo, Description FROM alarmlog WHERE runId = {run.Id}");
                    var logRows = dtAlarmLog != null ? dtAlarmLog.AsEnumerable().ToList() : new List<DataRow>();

                    // Fetch telemetry data
                    var dtTelemetry = _connector.ExecuteQuery($"SELECT DateTime, NhietDoMoiTruong, DoAmMoiTruong, ApSuat, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi, ThoiGianCapLieu, ThoiGianTron1, ThoiGianXaDay, ThoiGianRungXaDay, ThoiGianHutXaDay, ThoiGianTron2, ThoiGianXaHang, ThoiGianRungXaHang FROM alarmreport WHERE runId = {run.Id} ORDER BY DateTime ASC");
                    var telemetryRows = dtTelemetry != null ? dtTelemetry.AsEnumerable().ToList() : new List<DataRow>();

                    // Fetch alarms for stage warnings
                    var dtAlarms = _connector.ExecuteQuery($"SELECT DateTime, CongDoan, TagName, Value, Threshold, Message FROM realtime_alarms WHERE runId = {run.Id} AND LOWER(Severity) IN ('alarm', 'warning') ORDER BY DateTime ASC");
                    var alarmRows = dtAlarms != null ? dtAlarms.AsEnumerable().ToList() : new List<DataRow>();

                    // Populate 8 stages
                    for (int stepIdx = 0; stepIdx < stepDefs.Length; stepIdx++)
                    {
                        var def = stepDefs[stepIdx];
                        int r = blockStartRow + 1 + stepIdx;

                        // Find matching log in alarmlog
                        var stepLogRow = logRows.FirstOrDefault(row => {
                            string rowTagNo = row.Table.Columns.Contains("TagNo") && row["TagNo"] != DBNull.Value ? row["TagNo"].ToString().Trim() : "";
                            if (!string.IsNullOrEmpty(rowTagNo))
                            {
                                return rowTagNo.Equals(def.TagNo, StringComparison.OrdinalIgnoreCase);
                            }
                            string desc = row["Description"] != DBNull.Value ? row["Description"].ToString() : "";
                            return desc.IndexOf(def.Name, StringComparison.OrdinalIgnoreCase) >= 0;
                        });

                        if (stepLogRow != null)
                        {
                            DateTime startTime = Convert.ToDateTime(stepLogRow["OccurrenceTime"]);
                            ws.Cells[r, 4].Value = startTime.ToString("HH:mm:ss"); // Start Time
                            
                            string statusVal = stepLogRow["Status"].ToString().Trim();
                            bool isCompleted = statusVal.Equals("Resolved", StringComparison.OrdinalIgnoreCase);

                            DateTime? endTime = null;
                            if (isCompleted && stepLogRow["RestoreTime"] != DBNull.Value)
                            {
                                endTime = Convert.ToDateTime(stepLogRow["RestoreTime"]);
                                ws.Cells[r, 5].Value = endTime.Value.ToString("HH:mm:ss"); // End Time
                                double totalSeconds = (endTime.Value - startTime).TotalSeconds;
                                if (telemetryRows.Any())
                                {
                                    double maxVal = 0;
                                    foreach (var tr in telemetryRows)
                                    {
                                        if (tr.Table.Columns.Contains(def.Alias) && tr[def.Alias] != DBNull.Value)
                                        {
                                            if (double.TryParse(tr[def.Alias].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                                            {
                                                if (val > maxVal) maxVal = val;
                                            }
                                        }
                                    }
                                    if (maxVal > 0)
                                    {
                                        totalSeconds = maxVal;
                                    }
                                }
                                ws.Cells[r, 6].Value = $"{(int)Math.Round(totalSeconds)}s"; // Duration
                            }

                            // Calculate temperatures
                            var stepTelemetry = telemetryRows.Where(tr => {
                                DateTime dt = Convert.ToDateTime(tr["DateTime"]).AddSeconds(-20); // Time-lag compensation
                                if (endTime.HasValue) return dt >= startTime && dt <= endTime.Value;
                                return dt >= startTime;
                            }).ToList();

                            if (stepTelemetry.Count == 0) // Fallback
                            {
                                stepTelemetry = telemetryRows.Where(tr => {
                                    DateTime dt = Convert.ToDateTime(tr["DateTime"]);
                                    if (endTime.HasValue) return dt >= startTime && dt <= endTime.Value;
                                    return dt >= startTime;
                                }).ToList();
                            }

                            var envTemps = new List<double>();
                            var envHumids = new List<double>();
                            var pressures = new List<double>();
                            var topTemps = new List<double>();
                            var midTemps = new List<double>();
                            var botTemps = new List<double>();

                            foreach (var tr in stepTelemetry)
                            {
                                if (tr["NhietDoMoiTruong"] != DBNull.Value && double.TryParse(tr["NhietDoMoiTruong"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double et))
                                {
                                    envTemps.Add(NormalizeTemp(et));
                                }
                                if (tr["DoAmMoiTruong"] != DBNull.Value && double.TryParse(tr["DoAmMoiTruong"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double eh))
                                {
                                    envHumids.Add(eh);
                                }
                                if (tr["ApSuat"] != DBNull.Value && double.TryParse(tr["ApSuat"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double ap))
                                {
                                    pressures.Add(ap);
                                }
                                if (tr["NhietDoBonTronTren"] != DBNull.Value && double.TryParse(tr["NhietDoBonTronTren"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double t1))
                                {
                                    topTemps.Add(NormalizeTemp(t1));
                                }
                                if (tr["NhietDoBonTronGiua"] != DBNull.Value && double.TryParse(tr["NhietDoBonTronGiua"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double t2))
                                {
                                    midTemps.Add(NormalizeTemp(t2));
                                }
                                if (tr["NhietDoBonTronDuoi"] != DBNull.Value && double.TryParse(tr["NhietDoBonTronDuoi"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double t3))
                                {
                                    botTemps.Add(NormalizeTemp(t3));
                                }
                            }

                            // Get setup parameter for this step
                            int spVal = 0;
                            switch (stepIdx)
                            {
                                case 0: spVal = run.SpCapLieu; break;
                                case 1: spVal = run.SpTron1; break;
                                case 2: spVal = run.SpXaDay; break;
                                case 3: spVal = run.SpRungXaDay; break;
                                case 4: spVal = run.SpHutXaDay; break;
                                case 5: spVal = run.SpTron2; break;
                                case 6: spVal = run.SpXaHang; break;
                                case 7: spVal = run.SpRungXaHang; break;
                            }

                            ws.Cells[r, 3].Value = $"{spVal}s"; // Column C: Thông số cài đặt
                            ws.Cells[r, 7].Value = FormatRangeForExcel(envTemps, "°C"); // Ambient Temp
                            ws.Cells[r, 8].Value = FormatRangeForExcel(envHumids, "%"); // Ambient Humid
                            ws.Cells[r, 9].Value = FormatTempForExcel(topTemps); // Top Temp
                            ws.Cells[r, 10].Value = FormatTempForExcel(midTemps); // Mid Temp
                            ws.Cells[r, 11].Value = FormatTempForExcel(botTemps); // Bot Temp
                            ws.Cells[r, 12].Value = FormatRangeForExcel(pressures, "", 2); // Pressure (2 decimal places)

                            // Fetch matching alarms for this step
                            var stepAlarms = alarmRows.Where(ar => {
                                DateTime alarmTime = Convert.ToDateTime(ar["DateTime"]);
                                if (endTime.HasValue) return alarmTime >= startTime && alarmTime <= endTime.Value;
                                return alarmTime >= startTime;
                            }).ToList();

                            if (stepAlarms.Count > 0)
                            {
                                var messages = stepAlarms
                                    .Select(ar => ar["Message"] != DBNull.Value ? ar["Message"].ToString() : "")
                                    .Where(msg => !string.IsNullOrEmpty(msg))
                                    .Distinct()
                                    .ToList();

                                if (messages.Count > 0)
                                {
                                    ws.Cells[r, 13].Value = string.Join("\n", messages);
                                    ws.Cells[r, 13].Style.WrapText = true;
                                }
                                else
                                {
                                    ws.Cells[r, 13].Value = "Bình thường";
                                }
                            }
                            else
                            {
                                ws.Cells[r, 13].Value = "Bình thường";
                            }
                        }
                        else
                        {
                            int spVal = 0;
                            switch (stepIdx)
                            {
                                case 0: spVal = run.SpCapLieu; break;
                                case 1: spVal = run.SpTron1; break;
                                case 2: spVal = run.SpXaDay; break;
                                case 3: spVal = run.SpRungXaDay; break;
                                case 4: spVal = run.SpHutXaDay; break;
                                case 5: spVal = run.SpTron2; break;
                                case 6: spVal = run.SpXaHang; break;
                                case 7: spVal = run.SpRungXaHang; break;
                            }
                            ws.Cells[r, 3].Value = $"{spVal}s"; // Column C: Thông số cài đặt
                            ws.Cells[r, 4].Value = "-";
                            ws.Cells[r, 5].Value = "-";
                            ws.Cells[r, 6].Value = "-";
                            ws.Cells[r, 7].Value = "-";
                            ws.Cells[r, 8].Value = "-";
                            ws.Cells[r, 9].Value = "-";
                            ws.Cells[r, 10].Value = "-";
                            ws.Cells[r, 11].Value = "-";
                            ws.Cells[r, 12].Value = "-";
                            ws.Cells[r, 13].Value = "Chưa thực hiện";
                        }
                    }
                }

                // --- POPULATE SECTIONS 4, 5, 6, 7 ---
                int qcSectionStart = 41 + currentShift; // Section 4 starts here dynamically
                
                // Section 4: KẾT QUẢ ĐẦU RA
                ws.Cells[qcSectionStart + 2, 2].Value = totalActualProduced; // Sản lượng đạt (totalBomWeight - allowableLoss)
                ws.Cells[qcSectionStart + 2, 6].Value = 0; // Sản lượng lỗi / loại bỏ
                
                ws.Cells[qcSectionStart + 3, 2].Value = allowableLoss; // Hao hụt (dạng số kg thực tế)
                ws.Cells[qcSectionStart + 3, 6].Value = "-"; // Rework
                ws.Cells[qcSectionStart + 4, 2].Value = lotNo; // Mã mẫu lưu
                ws.Cells[qcSectionStart + 4, 6].Value = batchStatus; // Tình trạng lô

                // Section 5: QC LÔ THÀNH PHẨM
                int qcTableStart = qcSectionStart + 7; // QC Chỉ tiêu starts
                
                // Rename Column 2 header from "Kết quả" to "Tiêu chuẩn"
                ws.Cells[qcTableStart, 2].Value = "Tiêu chuẩn";
                
                // Write standard values in Column 2 (Tiêu chuẩn)
                ws.Cells[qcTableStart + 1, 2].Value = "Đồng đều, không vón"; // Cảm quan
                ws.Cells[qcTableStart + 2, 2].Value = $"{targetWeight} kg"; // Khối lượng
                ws.Cells[qcTableStart + 3, 2].Value = "Đúng loại, kín"; // Bao bì / seal
                ws.Cells[qcTableStart + 4, 2].Value = lotNo; // Mã in trên bao bì (same as lotNo / Mã mẫu lưu)
                ws.Cells[qcTableStart + 5, 2].Value = ""; // Chỉ tiêu đặc thù (Để trống)

                // Leave Column 3 (Đạt / Không đạt) and Column 4 (Ghi chú) completely blank
                for (int r = qcTableStart + 1; r <= qcTableStart + 5; r++)
                {
                    ws.Cells[r, 3].Value = ""; // Đạt / Không đạt
                    ws.Cells[r, 4].Value = ""; // Ghi chú
                }

                // Section 6: SỰ CỐ PHÁT SINH VÀ XỬ LÝ (Populate from realtime_alarms with Severity = ALARM or System pause INFO)
                int incidentSectionStart = qcTableStart + 7;
                var dtGlobalAlarms = _connector.ExecuteQuery($@"
                    SELECT DateTime, Message, Value, Threshold, restore_time, CongDoan 
                    FROM realtime_alarms 
                    WHERE batchId = {batchId} 
                      AND (Severity = 'ALARM' OR (Severity = 'INFO' AND TagName = 'System' AND Message = 'Tạm dừng máy')) 
                    ORDER BY DateTime ASC LIMIT 4");
                
                if (dtGlobalAlarms != null && dtGlobalAlarms.Rows.Count > 0)
                {
                    for (int idx = 0; idx < dtGlobalAlarms.Rows.Count; idx++)
                    {
                        int r = incidentSectionStart + 2 + idx;
                        var row = dtGlobalAlarms.Rows[idx];
                        ws.Cells[r, 1].Value = Convert.ToDateTime(row["DateTime"]).ToString("HH:mm:ss"); // Thời điểm
                        
                        string message = row["Message"].ToString();
                        string cd = row.Table.Columns.Contains("CongDoan") && row["CongDoan"] != DBNull.Value ? row["CongDoan"].ToString().Trim() : "";
                        if (cd.Equals("T001", StringComparison.OrdinalIgnoreCase)) cd = "Cấp liệu";
                        else if (cd.Equals("T002", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 1";
                        else if (cd.Equals("T003", StringComparison.OrdinalIgnoreCase)) cd = "Xả đáy";
                        else if (cd.Equals("T004", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả đáy";
                        else if (cd.Equals("T005", StringComparison.OrdinalIgnoreCase)) cd = "Hút xả đáy";
                        else if (cd.Equals("T006", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 2";
                        else if (cd.Equals("T007", StringComparison.OrdinalIgnoreCase)) cd = "Xả hàng";
                        else if (cd.Equals("T008", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả hàng";

                        if (message == "Tạm dừng máy")
                        {
                            var startTime = Convert.ToDateTime(row["DateTime"]);
                            string suffix = !string.IsNullOrEmpty(cd) ? $" tại công đoạn {cd}" : "";
                            if (row.Table.Columns.Contains("restore_time") && row["restore_time"] != DBNull.Value)
                            {
                                var restoreTime = Convert.ToDateTime(row["restore_time"]);
                                var duration = (restoreTime - startTime).TotalSeconds;
                                message = $"Tạm dừng máy{suffix} ({duration:F0}s từ {startTime:HH:mm:ss} đến {restoreTime:HH:mm:ss})";
                            }
                            else
                            {
                                message = $"Tạm dừng máy{suffix} (Bắt đầu từ {startTime:HH:mm:ss} - chưa chạy lại)";
                            }
                        }
                        
                        ws.Cells[r, 2].Value = message; // Mô tả sự cố
                        ws.Cells[r, 3].Value = ""; // Hành động xử lý (Để trống cho người dùng tự điền)
                        ws.Cells[r, 4].Value = "Hệ thống"; // Người xử lý
                        ws.Cells[r, 5].Value = "Đã khắc phục"; // Kết quả
                    }
                }
                else
                {
                    ws.Cells[incidentSectionStart + 2, 2].Value = "Không có sự cố phát sinh.";
                }

                // --- POPULATE SECTION 7: XÁC NHẬN ---
                int signSectionStart = incidentSectionStart + 7;
                
                // Do not fill data from base into Operator, Supervisor, QC, and Manager columns (Leave blank)
                for (int r = signSectionStart + 2; r <= signSectionStart + 5; r++)
                {
                    ws.Cells[r, 2].Value = ""; // Họ tên
                    ws.Cells[r, 3].Value = ""; // Ký xác nhận
                    ws.Cells[r, 4].Value = ""; // Thời gian
                }

                // Clean up: delete other sheets
                if (package.Workbook.Worksheets["Nhat ky san xuat"] != null)
                {
                    package.Workbook.Worksheets.Delete("Nhat ky san xuat");
                }
                if (package.Workbook.Worksheets["Huong dan"] != null)
                {
                    package.Workbook.Worksheets.Delete("Huong dan");
                }
                
                // Widen columns C, F and J, K, L, M
                ws.Column(3).Width = 30;
                ws.Column(6).Width = 18;
                ws.Column(7).Width = 18;
                ws.Column(9).Width = 18;
                ws.Column(10).Width = 20;
                ws.Column(11).Width = 20;
                ws.Column(12).Width = 20;
                ws.Column(13).Width = 20;

                // Remove medium bottom border from Section 4 last row
                // We clear borders for the entire sheet first, so this is handled automatically

                // Dynamically expand backgrounds and merge ranges to column M (13 columns)
                int maxRow = ws.Dimension.End.Row;
                for (int r = 1; r <= maxRow; r++)
                {
                    var cellA = ws.Cells[r, 1];
                    string textA = cellA.Text;

                    if (ws.Cells[r, 1].Merge && ws.Cells[r, 10].Merge == false)
                    {
                        if (textA.StartsWith("MẪU") || textA.Contains("1. THÔNG") || textA.Contains("2. ĐẦU") || textA.Contains("3. THÔNG SỐ") || textA.Contains("4. KẾT QUẢ") || textA.Contains("5. QC LÔ") || textA.Contains("6. SỰ CỐ") || textA.Contains("7. XÁC NHẬN"))
                        {
                            ExpandHeaderRowToColumnM(ws, r);
                        }
                    }

                    // Expand sub-header rows with specific starts (Mục, STT, Mẻ, Chỉ tiêu, Thời điểm, Vai trò)
                    string cleanText = textA.Normalize(System.Text.NormalizationForm.FormC).Trim().ToLower();
                    bool isSubHeader = cleanText.StartsWith("mục") || 
                                       cleanText.StartsWith("stt") || 
                                       cleanText.StartsWith("mẻ") || 
                                       cleanText.StartsWith("chỉ tiêu") || 
                                       cleanText.StartsWith("thời điểm") || 
                                       cleanText.StartsWith("vai trò");

                    if (isSubHeader)
                    {
                        ExpandColoredRowToColumnM(ws, r, 2);
                    }
                    else
                    {
                        var fillH = ws.Cells[r, 8].Style.Fill;
                        var fillI = ws.Cells[r, 9].Style.Fill;
                        var fillJ = ws.Cells[r, 10].Style.Fill;
                        
                        bool hasBgH = fillH.PatternType != OfficeOpenXml.Style.ExcelFillStyle.None;
                        bool hasBgI = fillI.PatternType != OfficeOpenXml.Style.ExcelFillStyle.None;
                        bool hasBgJ = fillJ.PatternType != OfficeOpenXml.Style.ExcelFillStyle.None;

                        if ((hasBgH || hasBgI) && !hasBgJ)
                        {
                            int sourceCol = hasBgI ? 9 : 8;
                            ExpandColoredRowToColumnM(ws, r, sourceCol);
                        }
                    }
                }

                // Apply new styling, borders, and colors
                int bottomRow = signSectionStart + 5;

                // 1. Clear all borders across cells A1:M[bottomRow]
                for (int r = 1; r <= bottomRow; r++)
                {
                    for (int col = 1; col <= 13; col++)
                    {
                        var border = ws.Cells[r, col].Style.Border;
                        border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                        border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                        border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                        border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                    }
                }

                // Format Row 1 (main title) - Background chỉ đến cột I (Col 9)
                try
                {
                    ws.Cells[1, 1, 1, 13].Merge = false;
                }
                catch { }
                ws.Cells[1, 1, 1, 9].Merge = true;
                for (int col = 10; col <= Math.Max(20, ws.Dimension.End.Column); col++)
                {
                    ws.Cells[1, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                }

                // 2. Set background color #4d94d8 for headings 1-7 (text black, bold) and merge to their respective columns
                var headingRowTargets = new List<Tuple<int, int>>
                {
                    Tuple.Create(3, 9), // Section 1: Col I (9)
                    Tuple.Create(11, 8), // Section 2: Col H (8)
                    Tuple.Create(run1StartRow - 1, 13), // Section 3: Col M (13)
                    Tuple.Create(qcSectionStart, 8), // Section 4: Col H (8)
                    Tuple.Create(qcTableStart - 1, 4), // Section 5: Col D (4)
                    Tuple.Create(incidentSectionStart, 5), // Section 6: Col E (5)
                    Tuple.Create(signSectionStart, 4) // Section 7: Col D (4)
                };

                foreach (var target in headingRowTargets)
                {
                    int row = target.Item1;
                    int targetCol = target.Item2;

                    try
                    {
                        ws.Cells[row, 1, row, 13].Merge = false;
                    }
                    catch { }

                    // Apply styles cell-by-cell BEFORE merging to avoid EPPlus merged cell styling issues
                    for (int col = 1; col <= targetCol; col++)
                    {
                        var cell = ws.Cells[row, col];
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(77, 148, 216)); // #4d94d8
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black); // Black text
                        cell.Style.Font.Bold = true;
                    }

                    ws.Cells[row, 1, row, targetCol].Merge = true;

                    for (int col = targetCol + 1; col <= Math.Max(20, ws.Dimension.End.Column); col++)
                    {
                        ws.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                    }
                }

                // 3. Set background color #b4c6e7 for table sub-headers
                var subHeaderRanges = new List<Tuple<int, int, int>>
                {
                    Tuple.Create(4, 1, 9), // Section 1: Col I (9)
                    Tuple.Create(12, 1, 8), // Section 2: Col H (8)
                    Tuple.Create(qcSectionStart + 1, 1, 8), // Section 4: Col H (8)
                    Tuple.Create(qcTableStart, 1, 4), // Section 5: Col D (4)
                    Tuple.Create(incidentSectionStart + 1, 1, 5), // Section 6: Col E (5)
                    Tuple.Create(signSectionStart + 1, 1, 4) // Section 7: Col D (4)
                };

                for (int runIdx = 0; runIdx < runsList.Count; runIdx++)
                {
                    int blockStartRow = GetRunBlockStartRow(runIdx + 1, bomShift);
                    subHeaderRanges.Add(Tuple.Create(blockStartRow + 1, 1, 13)); // Section 3 (Runs): Col M (13)
                }

                foreach (var range in subHeaderRanges)
                {
                    int row = range.Item1;
                    int startCol = range.Item2;
                    int endCol = range.Item3;

                    // Apply styles cell-by-cell
                    for (int col = startCol; col <= endCol; col++)
                    {
                        var cell = ws.Cells[row, col];
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(180, 198, 231)); // #b4c6e7
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.Font.Bold = true;
                    }

                    for (int col = endCol + 1; col <= Math.Max(20, ws.Dimension.End.Column); col++)
                    {
                        ws.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                    }
                }

                // 4. Remove background color for "Mã mẫu lưu" row
                for (int col = 1; col <= Math.Max(20, ws.Dimension.End.Column); col++)
                {
                    ws.Cells[qcSectionStart + 4, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                }

                // 5. Bỏ background màu vàng và gạch chân ở ô G9 Mã mẫu lưu (cả ô tiêu đề và giá trị)
                var cellsToFixG9 = new[] { "G9", "H9", "I9" };
                foreach (var addr in cellsToFixG9)
                {
                    ws.Cells[addr].Style.Font.UnderLine = false;
                    ws.Cells[addr].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
                }

                // 6. Action helper to apply thin black borders
                Action<int, int, int, int> applyThinBorders = (startRow, endRow, startCol, endCol) =>
                {
                    for (int r = startRow; r <= endRow; r++)
                    {
                        for (int col = startCol; col <= endCol; col++)
                        {
                            var border = ws.Cells[r, col].Style.Border;
                            border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            border.Top.Color.SetColor(System.Drawing.Color.Black);
                            border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            border.Left.Color.SetColor(System.Drawing.Color.Black);
                            border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            border.Right.Color.SetColor(System.Drawing.Color.Black);
                        }
                    }
                };

                // 7. Draw thin black borders for each block
                applyThinBorders(4, 9, 1, 9); // Section 1
                applyThinBorders(12, 17 + currentShift, 1, 8); // Section 2
                
                for (int runIdx = 0; runIdx < runsList.Count; runIdx++)
                {
                    int blockStartRow = GetRunBlockStartRow(runIdx + 1, bomShift);
                    applyThinBorders(blockStartRow + 1, blockStartRow + 9, 1, 13); // Section 3
                }

                applyThinBorders(qcSectionStart + 1, qcSectionStart + 4, 1, 4); // Section 4 (left)
                applyThinBorders(qcSectionStart + 1, qcSectionStart + 4, 5, 8); // Section 4 (right)
                applyThinBorders(qcTableStart, qcTableStart + 5, 1, 4); // Section 5
                applyThinBorders(incidentSectionStart + 1, incidentSectionStart + 5, 1, 5); // Section 6
                applyThinBorders(signSectionStart + 1, signSectionStart + 5, 1, 4); // Section 7

                // Rename primary sheet
                ws.Name = "Nhật ký sản xuất";

                // Format file name: batch_record_{batch_id}_{yyyyMMdd}.xlsx
                fileName = $"batch_record_{batchId}_{createdAt:yyyyMMdd}.xlsx";

                return package.GetAsByteArray();
            }
        }

        private void ExpandHeaderRowToColumnM(ExcelWorksheet ws, int row)
        {
            // Get style of first cell BEFORE any copy or merge
            var styleSrc = ws.Cells[row, 1].Style;
            
            try
            {
                File.AppendAllText(@"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\scratch\debug_log.txt", 
                    $"[ExpandHeaderRowToColumnM] Row {row}: Text='{ws.Cells[row, 1].Text}', PatternType={styleSrc.Fill.PatternType}, ColorRgb='{styleSrc.Fill.BackgroundColor.Rgb}'\n");
            }
            catch {}

            try
            {
                ws.Cells[row, 1, row, 9].Merge = false;
            }
            catch { }
            
            // Copy style to columns 2 to 13 BEFORE merging
            for (int col = 2; col <= 13; col++)
            {
                CopyStyle(styleSrc, ws.Cells[row, col].Style);
                // Clear left/right borders for internal cells of the merged range
                ws.Cells[row, col].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
                ws.Cells[row, col].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;
            }
            // Clear right border of column 1 to prevent internal line between col 1 and 2
            ws.Cells[row, 1].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.None;

            ws.Cells[row, 1, row, 13].Merge = true;
            
            ws.Cells[row, 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
            ws.Cells[row, 13].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

            // Clear background fill for column 14 onwards to limit background to column M
            for (int col = 14; col <= Math.Max(20, ws.Dimension.End.Column); col++)
            {
                ws.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
            }
        }

        private void ExpandColoredRowToColumnM(ExcelWorksheet ws, int row, int sourceCol = 9)
        {
            var styleSrc = ws.Cells[row, sourceCol].Style;
            for (int col = sourceCol + 1; col <= 13; col++)
            {
                CopyStyle(styleSrc, ws.Cells[row, col].Style);
            }
            ws.Cells[row, 13].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

            // Clear background fill for column 14 onwards to limit background to column M
            for (int col = 14; col <= Math.Max(20, ws.Dimension.End.Column); col++)
            {
                ws.Cells[row, col].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None;
            }
        }

        private int GetRunBlockStartRow(int runNumber, int currentShift)
        {
            if (runNumber == 1) return 20 + currentShift;
            return 30 + currentShift + (runNumber - 2) * 9;
        }

        private void CopyRowStyles(ExcelWorksheet ws, int sourceRow, int destRow)
        {
            ws.Row(destRow).Height = ws.Row(sourceRow).Height;
            ws.Row(destRow).CustomHeight = true;
            for (int col = 1; col <= ws.Dimension.End.Column; col++)
            {
                var sourceCell = ws.Cells[sourceRow, col];
                var destCell = ws.Cells[destRow, col];
                CopyStyle(sourceCell.Style, destCell.Style);
            }
        }

        private void DuplicateRunBlock(ExcelWorksheet ws, int sourceStartRow, int destStartRow, int runNumber)
        {
            ws.InsertRow(destStartRow, 9);
            for (int i = 0; i < 9; i++)
            {
                int srcRow = sourceStartRow + i;
                int dstRow = destStartRow + i;
                ws.Row(dstRow).Height = ws.Row(srcRow).Height;
                ws.Row(dstRow).CustomHeight = true;

                for (int col = 1; col <= 13; col++)
                {
                    var srcCell = ws.Cells[srcRow, col];
                    var dstCell = ws.Cells[dstRow, col];
                    CopyStyle(srcCell.Style, dstCell.Style);
                    dstCell.Value = srcCell.Value;
                }
            }
            // Update run number in Column A
            ws.Cells[destStartRow + 1, 1].Value = runNumber;
        }

        private void CopyExcelColor(OfficeOpenXml.Style.ExcelColor source, OfficeOpenXml.Style.ExcelColor dest)
        {
            if (source == null || dest == null) return;
            if (source.Auto)
            {
                dest.SetAuto();
            }
            else if (source.Theme.HasValue)
            {
                dest.SetColor(source.Theme.Value);
            }
            else if (!string.IsNullOrEmpty(source.Rgb))
            {
                try
                {
                    string hex = source.Rgb;
                    if (hex.Length == 8)
                    {
                        hex = hex.Substring(2);
                    }
                    dest.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + hex));
                }
                catch
                {
                    // Fallback
                }
            }
            else if (source.Indexed >= 0)
            {
                dest.Indexed = source.Indexed;
            }
            dest.Tint = source.Tint;
        }

        private void CopyBorderItem(OfficeOpenXml.Style.ExcelBorderItem source, OfficeOpenXml.Style.ExcelBorderItem dest)
        {
            if (source == null || dest == null) return;
            dest.Style = source.Style;
            if (source.Style != OfficeOpenXml.Style.ExcelBorderStyle.None)
            {
                CopyExcelColor(source.Color, dest.Color);
            }
        }

        private void CopyStyle(OfficeOpenXml.Style.ExcelStyle source, OfficeOpenXml.Style.ExcelStyle dest)
        {
            if (source == null || dest == null) return;
            
            dest.Numberformat.Format = source.Numberformat.Format;
            
            dest.Font.Bold = source.Font.Bold;
            dest.Font.Italic = source.Font.Italic;
            dest.Font.Name = source.Font.Name;
            dest.Font.Size = source.Font.Size;
            dest.Font.Strike = source.Font.Strike;
            dest.Font.UnderLine = source.Font.UnderLine;
            if (source.Font.UnderLineType != OfficeOpenXml.Style.ExcelUnderLineType.None)
            {
                dest.Font.UnderLineType = source.Font.UnderLineType;
            }
            CopyExcelColor(source.Font.Color, dest.Font.Color);
            
            dest.Fill.PatternType = source.Fill.PatternType;
            if (source.Fill.PatternType != OfficeOpenXml.Style.ExcelFillStyle.None)
            {
                CopyExcelColor(source.Fill.BackgroundColor, dest.Fill.BackgroundColor);
                CopyExcelColor(source.Fill.PatternColor, dest.Fill.PatternColor);
            }

            CopyBorderItem(source.Border.Top, dest.Border.Top);
            CopyBorderItem(source.Border.Bottom, dest.Border.Bottom);
            CopyBorderItem(source.Border.Left, dest.Border.Left);
            CopyBorderItem(source.Border.Right, dest.Border.Right);
            CopyBorderItem(source.Border.Diagonal, dest.Border.Diagonal);
            dest.Border.DiagonalUp = source.Border.DiagonalUp;
            dest.Border.DiagonalDown = source.Border.DiagonalDown;

            dest.HorizontalAlignment = source.HorizontalAlignment;
            dest.VerticalAlignment = source.VerticalAlignment;
            dest.WrapText = source.WrapText;
            dest.TextRotation = source.TextRotation;
            dest.Indent = source.Indent;
            dest.ShrinkToFit = source.ShrinkToFit;
            dest.ReadingOrder = source.ReadingOrder;
        }

        private string FormatTempForExcel(List<double> temps)
        {
            if (temps == null || temps.Count == 0) return "-";
            double min = temps.Min();
            double max = temps.Max();
            string minStr = Math.Round(min, 1).ToString("0.#", CultureInfo.InvariantCulture);
            string maxStr = Math.Round(max, 1).ToString("0.#", CultureInfo.InvariantCulture);
            
            if (minStr == maxStr) return $"{minStr}°C";
            return $"{minStr} - {maxStr}°C";
        }

        private string FormatRangeForExcel(List<double> values, string suffix = "", int decimalPlaces = 1)
        {
            if (values == null || values.Count == 0) return "-";
            double min = values.Min();
            double max = values.Max();
            string format = decimalPlaces == 2 ? "0.00" : "0.#";
            string minStr = Math.Round(min, decimalPlaces).ToString(format, CultureInfo.InvariantCulture);
            string maxStr = Math.Round(max, decimalPlaces).ToString(format, CultureInfo.InvariantCulture);
            
            if (minStr == maxStr) return $"{minStr}{suffix}";
            return $"{minStr} - {maxStr}{suffix}";
        }

        private double NormalizeTemp(double val)
        {
            if (val == 0) return 0;
            double absVal = Math.Abs(val);
            if (absVal >= 100.0)
            {
                return val / 10.0;
            }
            if (absVal > 0 && absVal < 10.0)
            {
                return val * 10.0;
            }
            return val;
        }

        private Dictionary<string, string> ParseUrlEncodedPayload(string payload)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(payload)) return dict;
            try
            {
                var nameValues = System.Web.HttpUtility.ParseQueryString(payload);
                foreach (string key in nameValues.AllKeys)
                {
                    if (key != null)
                    {
                        dict[key] = nameValues[key];
                    }
                }
            }
            catch
            {
                var parts = payload.Split('&');
                foreach (var part in parts)
                {
                    var kv = part.Split('=');
                    if (kv.Length == 2)
                    {
                        string key = Uri.UnescapeDataString(kv[0]);
                        string val = Uri.UnescapeDataString(kv[1]);
                        dict[key] = val;
                    }
                }
            }
            return dict;
        }

        private List<BomItemDto> ParseBomBase64(string base64Data, int runNumber)
        {
            var result = new List<BomItemDto>();
            if (string.IsNullOrEmpty(base64Data)) return result;
            try
            {
                byte[] bytes = Convert.FromBase64String(base64Data);
                string json = System.Text.Encoding.UTF8.GetString(bytes);
                var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(json);
                int stt = 1;
                foreach (var item in list)
                {
                    if (item.Count >= 5)
                    {
                        result.Add(new BomItemDto
                        {
                            STT = stt++,
                            Code = item[0],
                            MaterialCode = item[1],
                            Quantity = double.TryParse(item[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double q) ? q : 0,
                            ActualQuantity = item.Count >= 4 && double.TryParse(item[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double aq) ? aq : 0,
                            Unit = item[4],
                            BatchNo = item.Count >= 6 ? item[5] : "",
                            Note = $"Mẻ {runNumber}"
                        });
                    }
                }
            }
            catch { }
            return result;
        }

        private string GetField(Dictionary<string, string> dict, string key, string fallback)
        {
            if (dict.ContainsKey(key) && !string.IsNullOrEmpty(dict[key]))
            {
                return dict[key];
            }
            return fallback;
        }

        private class RunDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int RunNumber { get; set; }
            public string Status { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public int SpCapLieu { get; set; }
            public int SpTron1 { get; set; }
            public int SpXaDay { get; set; }
            public int SpRungXaDay { get; set; }
            public int SpHutXaDay { get; set; }
            public int SpTron2 { get; set; }
            public int SpXaHang { get; set; }
            public int SpRungXaHang { get; set; }
        }

        private class BomItemDto
        {
            public int STT { get; set; }
            public string Code { get; set; }
            public string MaterialCode { get; set; }
            public string Unit { get; set; }
            public double Quantity { get; set; }
            public double ActualQuantity { get; set; }
            public string BatchNo { get; set; }
            public string Note { get; set; }
        }
    }
}
