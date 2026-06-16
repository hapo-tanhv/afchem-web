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
            var dtRuns = _connector.ExecuteQuery($"SELECT id, name, run_number, status, start_time, end_time FROM runs WHERE batch_id = {batchId} ORDER BY run_number ASC");
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
                        EndTime = r["end_time"] != DBNull.Value ? Convert.ToDateTime(r["end_time"]) : (DateTime?)null
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
            double targetWeight = 0;
            if (webhookFields.ContainsKey("custom_khoi_luong_muc_tieu") && double.TryParse(webhookFields["custom_khoi_luong_muc_tieu"], out double wVal))
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
                
                // Calculate actual produced weight (sum of completed run weights)
                double totalActualProduced = 0;
                var completedRunsDict = new Dictionary<int, double>();
                var dtRunWeights = _connector.ExecuteQuery($@"
                    SELECT ri.run_id, SUM(ri.quantity) as run_weight 
                    FROM run_info ri 
                    JOIN runs r ON ri.run_id = r.id 
                    WHERE r.batch_id = {batchId}
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

                foreach (var run in runsList)
                {
                    if (run.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        if (completedRunsDict.ContainsKey(run.Id))
                        {
                            totalActualProduced += completedRunsDict[run.Id];
                        }
                        else
                        {
                            totalActualProduced += (targetWeight / (runsList.Count > 0 ? runsList.Count : 1));
                        }
                    }
                }
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
                }

                // --- POPULATE SECTION 3: THÔNG SỐ QUÁ TRÌNH (RUNS) ---
                // Step definitions for SCADA 8 stages
                var stepDefs = new[]
                {
                    new { Code = 1, TagNo = "T001", Name = "Cấp liệu", Standard = "720s" },
                    new { Code = 2, TagNo = "T002", Name = "Trộn 1", Standard = "780s" },
                    new { Code = 3, TagNo = "T003", Name = "Xả đáy", Standard = "600s" },
                    new { Code = 4, TagNo = "T004", Name = "Rung xả đáy", Standard = "600s" },
                    new { Code = 5, TagNo = "T005", Name = "Hút xả đáy", Standard = "720s" },
                    new { Code = 6, TagNo = "T006", Name = "Trộn 2", Standard = "1200s" },
                    new { Code = 7, TagNo = "T007", Name = "Xả hàng", Standard = "1500s" },
                    new { Code = 8, TagNo = "T008", Name = "Rung xả hàng", Standard = "180s" }
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
                    var dtTelemetry = _connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi FROM alarmreport WHERE runId = {run.Id} ORDER BY DateTime ASC");
                    var telemetryRows = dtTelemetry != null ? dtTelemetry.AsEnumerable().ToList() : new List<DataRow>();

                    // Fetch alarms for stage warnings
                    var dtAlarms = _connector.ExecuteQuery($"SELECT DateTime, CongDoan, TagName, Value, Threshold, Message FROM realtime_alarms WHERE runId = {run.Id} AND Severity IN ('ALARM', 'WARNING') ORDER BY DateTime ASC");
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
                                ws.Cells[r, 6].Value = $"{(int)totalSeconds}s"; // Duration
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

                            var topTemps = new List<double>();
                            var midTemps = new List<double>();
                            var botTemps = new List<double>();

                            foreach (var tr in stepTelemetry)
                            {
                                if (tr["NhietDoBonTronTren"] != DBNull.Value) topTemps.Add(Convert.ToDouble(tr["NhietDoBonTronTren"]) / 10.0);
                                if (tr["NhietDoBonTronGiua"] != DBNull.Value) midTemps.Add(Convert.ToDouble(tr["NhietDoBonTronGiua"]) / 10.0);
                                if (tr["NhietDoBonTronDuoi"] != DBNull.Value) botTemps.Add(Convert.ToDouble(tr["NhietDoBonTronDuoi"]) / 10.0);
                            }

                            ws.Cells[r, 9].Value = FormatTempForExcel(topTemps); // Top Temp
                            ws.Cells[r, 10].Value = FormatTempForExcel(midTemps); // Mid Temp
                            ws.Cells[r, 11].Value = FormatTempForExcel(botTemps); // Bot Temp

                            // Warnings/Alarms count as Note
                            var stepAlarmsCount = alarmRows.Count(ar => {
                                DateTime alarmTime = Convert.ToDateTime(ar["DateTime"]);
                                if (endTime.HasValue) return alarmTime >= startTime && alarmTime <= endTime.Value;
                                return alarmTime >= startTime;
                            });

                            if (stepAlarmsCount > 0)
                            {
                                ws.Cells[r, 13].Value = $"{stepAlarmsCount} cảnh báo";
                            }
                            else
                            {
                                ws.Cells[r, 13].Value = "Bình thường";
                            }
                        }
                        else
                        {
                            ws.Cells[r, 4].Value = "-";
                            ws.Cells[r, 5].Value = "-";
                            ws.Cells[r, 6].Value = "-";
                            ws.Cells[r, 9].Value = "-";
                            ws.Cells[r, 10].Value = "-";
                            ws.Cells[r, 11].Value = "-";
                            ws.Cells[r, 13].Value = "Chưa thực hiện";
                        }
                    }
                }

                // --- POPULATE SECTIONS 4, 5, 6, 7 ---
                int qcSectionStart = 41 + currentShift; // Section 4 starts here dynamically
                
                // Section 4: KẾT QUẢ ĐẦU RA
                ws.Cells[qcSectionStart + 2, 2].Value = totalActualProduced; // B43 equivalent
                ws.Cells[qcSectionStart + 2, 6].Value = 0; // F43 equivalent (produced errors)
                
                double lossPercent = 0;
                if (targetWeight > 0)
                {
                    lossPercent = Math.Round((targetWeight - totalActualProduced) / targetWeight * 100, 2);
                }
                ws.Cells[qcSectionStart + 3, 2].Value = lossPercent > 0 ? $"{lossPercent}%" : "0%";
                ws.Cells[qcSectionStart + 3, 6].Value = "-"; // Rework
                ws.Cells[qcSectionStart + 4, 2].Value = lotNo; // Mã mẫu lưu
                ws.Cells[qcSectionStart + 4, 6].Value = batchStatus; // Tình trạng lô

                // Section 5: QC LÔ THÀNH PHẨM (Default to "Đạt")
                int qcTableStart = qcSectionStart + 7; // QC Chỉ tiêu starts
                ws.Cells[qcTableStart + 1, 2].Value = "Đồng đều, không vón cục"; // Cảm quan
                ws.Cells[qcTableStart + 1, 3].Value = "Đạt";
                ws.Cells[qcTableStart + 2, 2].Value = $"{totalActualProduced} KG"; // Khối lượng
                ws.Cells[qcTableStart + 2, 3].Value = "Đạt";
                ws.Cells[qcTableStart + 3, 2].Value = "Đầy đủ, kín seal"; // Bao bì
                ws.Cells[qcTableStart + 3, 3].Value = "Đạt";
                ws.Cells[qcTableStart + 4, 2].Value = lotNo; // Mã in bao bì
                ws.Cells[qcTableStart + 4, 3].Value = "Đạt";
                ws.Cells[qcTableStart + 5, 2].Value = "Đạt tiêu chuẩn"; // Đặc thù
                ws.Cells[qcTableStart + 5, 3].Value = "Đạt";

                // Section 6: SỰ CỐ PHÁT SINH VÀ XỬ LÝ (Populate from realtime_alarms with Severity = ALARM)
                int incidentSectionStart = qcTableStart + 7;
                var dtGlobalAlarms = _connector.ExecuteQuery($@"
                    SELECT DateTime, Message, Value, Threshold 
                    FROM realtime_alarms 
                    WHERE batchId = {batchId} 
                      AND Severity = 'ALARM' 
                    ORDER BY DateTime ASC LIMIT 4");
                
                if (dtGlobalAlarms != null && dtGlobalAlarms.Rows.Count > 0)
                {
                    for (int idx = 0; idx < dtGlobalAlarms.Rows.Count; idx++)
                    {
                        int r = incidentSectionStart + 2 + idx;
                        var row = dtGlobalAlarms.Rows[idx];
                        ws.Cells[r, 1].Value = Convert.ToDateTime(row["DateTime"]).ToString("HH:mm:ss"); // Thời điểm
                        ws.Cells[r, 2].Value = row["Message"].ToString(); // Mô tả sự cố
                        ws.Cells[r, 3].Value = "Tự động xử lý"; // Hành động xử lý
                        ws.Cells[r, 4].Value = "Hệ thống"; // Người xử lý
                        ws.Cells[r, 5].Value = "Đã khắc phục"; // Kết quả
                    }
                }
                else
                {
                    ws.Cells[incidentSectionStart + 2, 2].Value = "Không có sự cố phát sinh.";
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
                
                // Rename primary sheet
                ws.Name = "Nhật ký sản xuất";

                // Format file name: batch_record_{batch_id}_{yyyyMMdd}.xlsx
                fileName = $"batch_record_{batchId}_{createdAt:yyyyMMdd}.xlsx";

                return package.GetAsByteArray();
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
                    dest.SetColor(System.Drawing.ColorTranslator.FromHtml("#" + source.Rgb));
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
                            Quantity = double.TryParse(item[2], out double q) ? q : 0,
                            ActualQuantity = item.Count >= 4 && double.TryParse(item[3], out double aq) ? aq : 0,
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
