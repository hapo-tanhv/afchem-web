using CsvHelper;

using Hino.Getdata.Common;

using OfficeOpenXml;

using System;

using System.Collections.Generic;

using System.Globalization;

using System.IO;

using System.Linq;

using System.Text;

using System.Threading;

using System.Web;

using System.Web.Mvc;

using System.Data;



namespace LongDucProject.Controllers

{

    public class EventController : Controller

    {

        [HttpGet]

        public JsonResult GetBatches(string date)

        {

            try

            {

                var connector = new Hino.DatabaseConnector.MySQLConnect()

                {

                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"

                };

                string query = "SELECT id, name, status FROM batches ORDER BY id DESC";

                if (!string.IsNullOrEmpty(date))

                {

                    if (DateTime.TryParseExact(date, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))

                    {

                        string formattedDate = parsedDate.ToString("yyyy-MM-dd");

                        query = $"SELECT id, name, status FROM batches WHERE DATE(start_time) = '{formattedDate}' ORDER BY id DESC";

                    }

                }

                var dt = connector.ExecuteQuery(query);

                var list = new List<object>();

                if (dt != null)

                {

                    foreach (DataRow row in dt.Rows)

                    {

                        list.Add(new

                        {

                            id = Convert.ToInt32(row["id"]),

                            name = row["name"].ToString(),

                            status = row["status"] != DBNull.Value ? row["status"].ToString() : ""

                        });

                    }

                }

                return Json(list, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)

            {

                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);

            }

        }



        [HttpGet]
        public JsonResult GetEventLogRealtime(string batchId, string date, string runId)
        {
            try
            {
                var connector = new Hino.DatabaseConnector.MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };

                var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, batchId, runId, date);
                int selectedBatchId = resolution.BatchId;
                int selectedRunId = resolution.RunId;

                if (selectedBatchId <= 0)
                {
                    return Json(new {
                        batchId = 0,
                        resolvedRunId = 0,
                        batchName = "-",
                        cycleSummary = new {
                            status = "PENDING",
                            statusLabel = "Không tìm thấy dữ liệu mẻ",
                            batchId = "-",
                            productName = "-",
                            endTime = "-",
                            formula = "-",
                            totalTime = "-",
                            weight = "-",
                            startTime = "-"
                        },
                        phases = new List<object>(),
                        events = new List<object>(),
                        note = "Không tìm thấy dữ liệu mẻ."
                    }, JsonRequestBehavior.AllowGet);
                }

                // 1. Query Batch Details
                var dtBatch = connector.ExecuteQuery($"SELECT id, name, status, start_time, end_time, device_name FROM batches WHERE id = {selectedBatchId}");
                string batchName = "-";
                string batchStatus = "";
                string startStr = "-";
                string endStr = "-";
                string durationStr = "-";
                string deviceName = "-";

                string batchDateStr = "";
                if (dtBatch != null && dtBatch.Rows.Count > 0)
                {
                    var rowBatch = dtBatch.Rows[0];
                    batchName = rowBatch["name"].ToString();
                    batchStatus = rowBatch["status"].ToString();
                    deviceName = rowBatch["device_name"] != DBNull.Value ? rowBatch["device_name"].ToString() : "TX01 A";

                    if (rowBatch["start_time"] != DBNull.Value)
                    {
                        DateTime startTime = Convert.ToDateTime(rowBatch["start_time"]);
                        batchDateStr = startTime.ToString("yyyy-MM-dd");
                        startStr = startTime.ToString("HH:mm:ss");
                        if (rowBatch["end_time"] != DBNull.Value)
                        {
                            DateTime endTime = Convert.ToDateTime(rowBatch["end_time"]);
                            endStr = endTime.ToString("HH:mm:ss");
                            double totalSeconds = (endTime - startTime).TotalSeconds;
                            durationStr = FormatDuration(totalSeconds);
                        }
                        else
                        {
                            double totalSeconds = (DateTime.Now - startTime).TotalSeconds;
                            durationStr = FormatDuration(totalSeconds);
                        }
                    }
                }

                // Override durations with selected run if resolved
                if (selectedRunId > 0)
                {
                    var dtRunInfo = connector.ExecuteQuery($"SELECT name, status, start_time, end_time FROM runs WHERE id = {selectedRunId} LIMIT 1");
                    if (dtRunInfo != null && dtRunInfo.Rows.Count > 0)
                    {
                        var rowRun = dtRunInfo.Rows[0];
                        batchName = rowRun["name"].ToString(); // Display run name instead of batch name in Event Log!
                        batchStatus = rowRun["status"].ToString();
                        if (rowRun["start_time"] != DBNull.Value)
                        {
                            DateTime runStart = Convert.ToDateTime(rowRun["start_time"]);
                            startStr = runStart.ToString("HH:mm:ss");
                            if (rowRun["end_time"] != DBNull.Value)
                            {
                                DateTime runEnd = Convert.ToDateTime(rowRun["end_time"]);
                                endStr = runEnd.ToString("HH:mm:ss");
                                double totalSeconds = (runEnd - runStart).TotalSeconds;
                                durationStr = FormatDuration(totalSeconds);
                            }
                            else
                            {
                                double totalSeconds = (DateTime.Now - runStart).TotalSeconds;
                                durationStr = FormatDuration(totalSeconds);
                            }
                        }
                    }
                }

                var cycleSummary = new {
                    status = batchStatus.ToUpper(),
                    statusLabel = batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "Chu kỳ đang chạy..." : 
                                  (batchStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ? "Chu kỳ chưa bắt đầu" : 
                                  (batchStatus.Equals("Error", StringComparison.OrdinalIgnoreCase) || batchStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase) ? "Chu kỳ bị lỗi" : "Chu kỳ hoàn tất thành công")),
                    batchId = batchName,
                    productName = deviceName,
                    endTime = endStr,
                    formula = "TX01 - Formula A",
                    totalTime = durationStr,
                    weight = "500 kg",
                    startTime = startStr
                };

                // 2. Query Phase Logs and Telemetry/Alarms
                var stepsList = new List<object>();

                DataTable dtAlarmLog = null;
                DataTable dtTelemetry = null;
                DataTable dtAlarms = null;

                if (selectedRunId > 0)
                {
                    dtAlarmLog = connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE runId = {selectedRunId}");
                    dtTelemetry = connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi FROM alarmreport WHERE runId = {selectedRunId} ORDER BY DateTime ASC");
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE runId = {selectedRunId} ORDER BY DateTime ASC, id ASC");
                }
                else
                {
                    dtAlarmLog = connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE batchId = {selectedBatchId}");
                    dtTelemetry = connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi FROM alarmreport WHERE batchId = {selectedBatchId} ORDER BY DateTime ASC");
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE batchId = {selectedBatchId} ORDER BY DateTime ASC, id ASC");
                }

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

                var logRows = dtAlarmLog != null ? dtAlarmLog.AsEnumerable().ToList() : new List<DataRow>();
                var telemetryRows = dtTelemetry != null ? dtTelemetry.AsEnumerable().ToList() : new List<DataRow>();
                var alarmRows = dtAlarms != null ? dtAlarms.AsEnumerable().ToList() : new List<DataRow>();

                for (int i = 0; i < stepDefs.Length; i++)
                {
                    var def = stepDefs[i];
                    var stepLogRow = logRows.FirstOrDefault(r => {
                        string rowTagNo = r.Table.Columns.Contains("TagNo") && r["TagNo"] != DBNull.Value ? r["TagNo"].ToString().Trim() : "";
                        if (!string.IsNullOrEmpty(rowTagNo))
                        {
                            return rowTagNo.Equals(def.TagNo, StringComparison.OrdinalIgnoreCase);
                        }
                        string desc = r["Description"] != DBNull.Value ? r["Description"].ToString() : "";
                        if (def.Code == 1 && (desc.IndexOf("Cấp Liệu", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Cap Lieu", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 2 && (desc.IndexOf("Trộn 1", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Tron 1", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 3 && (desc.IndexOf("Xả Đáy", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Xa Day", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 4 && (desc.IndexOf("Rung Xả Đ", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa D", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 5 && (desc.IndexOf("Hút Xả Đáy", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Hut Xa Day", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 6 && (desc.IndexOf("Trộn 2", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Tron 2", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 7 && (desc.IndexOf("Xả Hàng", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Xa Hang", StringComparison.OrdinalIgnoreCase) >= 0) && desc.IndexOf("Rung", StringComparison.OrdinalIgnoreCase) < 0) return true;
                        if (def.Code == 8 && (desc.IndexOf("Rung Xả H", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa H", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        return false;
                    });

                    if (stepLogRow == null)
                    {
                        stepsList.Add(new
                        {
                            num = def.Code,
                            name = def.Name,
                            startTime = "-",
                            endTime = "-",
                            status = "pending",
                            statusText = "Chưa bắt đầu",
                            duration = "-",
                            alerts = new List<object>()
                        });
                    }
                    else
                    {
                        DateTime startTime = Convert.ToDateTime(stepLogRow["OccurrenceTime"]);
                        string stepStartStr = startTime.ToString("HH:mm:ss");
                        string stepEndStr = "-";
                        string stepDurationStr = "-";
                        string status = "in-progress";
                        string statusText = "Đang thực hiện";

                        string statusVal = stepLogRow["Status"].ToString().Trim();
                        bool isCompleted = statusVal.Equals("Resolved", StringComparison.OrdinalIgnoreCase);

                        DateTime? endTime = null;
                        if (isCompleted)
                        {
                            status = "completed";
                            statusText = "Hoàn thành";
                            if (stepLogRow["RestoreTime"] != DBNull.Value)
                            {
                                endTime = Convert.ToDateTime(stepLogRow["RestoreTime"]);
                                stepEndStr = endTime.Value.ToString("HH:mm:ss");
                                double totalSeconds = (endTime.Value - startTime).TotalSeconds;
                                stepDurationStr = $"{(int)totalSeconds}s";
                            }
                        }

                        // Find alerts for this step
                        var stepAlertsList = new List<object>();
                        var stepAlarms = alarmRows.Where(r => {
                            DateTime alarmTime = Convert.ToDateTime(r["DateTime"]);
                            bool timeInStep = false;
                            if (endTime.HasValue)
                            {
                                timeInStep = alarmTime >= startTime && alarmTime <= endTime.Value;
                            }
                            else
                            {
                                timeInStep = alarmTime >= startTime;
                            }

                            string cd = r["CongDoan"] != DBNull.Value ? r["CongDoan"].ToString().Trim() : "";
                            bool codeMatches = false;
                            if (!string.IsNullOrEmpty(cd))
                            {
                                if (cd.Equals(def.TagNo, StringComparison.OrdinalIgnoreCase)) codeMatches = true;
                                else if (cd.Equals(def.Name, StringComparison.OrdinalIgnoreCase)) codeMatches = true;
                                else
                                {
                                    string cdLower = RemoveSign4VietnameseString(cd).ToLower();
                                    string defNameLower = RemoveSign4VietnameseString(def.Name).ToLower();
                                    if (cdLower == defNameLower) codeMatches = true;
                                    else if (def.Code == 1 && (cdLower.Contains("cap lieu") || cdLower.Contains("cấp liệu") || cdLower.Contains("t001"))) codeMatches = true;
                                    else if (def.Code == 2 && (cdLower.Contains("tron 1") || cdLower.Contains("trộn 1") || cdLower.Contains("t002"))) codeMatches = true;
                                    else if (def.Code == 3 && (cdLower.Contains("xa day") || cdLower.Contains("xả đáy") || cdLower.Contains("t003"))) codeMatches = true;
                                    else if (def.Code == 4 && (cdLower.Contains("rung xa day") || cdLower.Contains("rung xả đáy") || cdLower.Contains("t004"))) codeMatches = true;
                                    else if (def.Code == 5 && (cdLower.Contains("hut xa day") || cdLower.Contains("hút xả đáy") || cdLower.Contains("t005"))) codeMatches = true;
                                    else if (def.Code == 6 && (cdLower.Contains("tron 2") || cdLower.Contains("trộn 2") || cdLower.Contains("t006"))) codeMatches = true;
                                    else if (def.Code == 7 && (cdLower.Contains("xa hang") || cdLower.Contains("xả hàng") || cdLower.Contains("t007")) && !cdLower.Contains("rung")) codeMatches = true;
                                    else if (def.Code == 8 && (cdLower.Contains("rung xa hang") || cdLower.Contains("rung xả hàng") || cdLower.Contains("t008"))) codeMatches = true;
                                }
                            }

                            return codeMatches || timeInStep;
                        }).ToList();

                        foreach (var row in stepAlarms)
                        {
                            DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                            string severity = row["Severity"].ToString();
                            string tagName = row["TagName"].ToString();
                            double val = Convert.ToDouble(row["Value"]);
                            double threshold = Convert.ToDouble(row["Threshold"]);
                            if (tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                val = val / 10.0;
                                threshold = threshold / 10.0;
                            }
                            string msg = row["Message"].ToString();

                            string unit = tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0 ? "°C" :
                                         tagName.IndexOf("ApSuat", StringComparison.OrdinalIgnoreCase) >= 0 ? "bar" : "";

                            string detailMessage = $"Giá trị: {val.ToString("0.#", CultureInfo.InvariantCulture)} {unit} (ngưỡng: {threshold.ToString("0.#", CultureInfo.InvariantCulture)} {unit})";

                            stepAlertsList.Add(new
                            {
                                time = alarmTime.ToString("HH:mm:ss"),
                                type = severity.ToUpper(),
                                title = msg,
                                message = detailMessage
                            });
                        }

                        stepsList.Add(new
                        {
                            num = def.Code,
                            name = def.Name,
                            startTime = stepStartStr,
                            endTime = stepEndStr,
                            status = status,
                            statusText = statusText,
                            duration = stepDurationStr,
                            alerts = stepAlertsList
                        });
                    }
                }

                // 3. Query All Events
                var eventsList = new List<object>();
                foreach (var row in alarmRows)
                {
                    DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                    string severity = row["Severity"].ToString();
                    string msg = row["Message"].ToString();
                    string cd = row["CongDoan"] != DBNull.Value ? row["CongDoan"].ToString().Trim() : "";

                    if (cd.Equals("T001", StringComparison.OrdinalIgnoreCase)) cd = "Cấp liệu";
                    else if (cd.Equals("T002", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 1";
                    else if (cd.Equals("T003", StringComparison.OrdinalIgnoreCase)) cd = "Xả đáy";
                    else if (cd.Equals("T004", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả đáy";
                    else if (cd.Equals("T005", StringComparison.OrdinalIgnoreCase)) cd = "Hút xả đáy";
                    else if (cd.Equals("T006", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 2";
                    else if (cd.Equals("T007", StringComparison.OrdinalIgnoreCase)) cd = "Xả hàng";
                    else if (cd.Equals("T008", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả hàng";

                    eventsList.Add(new
                    {
                        time = alarmTime.ToString("HH:mm:ss"),
                        phase = cd,
                        @event = msg,
                        severity = severity.ToUpper()
                    });
                }

                string note = batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase)
                    ? "Chu kỳ đang chạy. Chất lượng sản phẩm: <strong class='text-warning'>ĐANG ĐÁNH GIÁ</strong>"
                    : (batchStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)
                        ? "Chu kỳ đang chờ chạy, chưa có dữ liệu đánh giá chất lượng."
                        : (batchStatus.Equals("Error", StringComparison.OrdinalIgnoreCase) || batchStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase)
                            ? "Chu kỳ bị lỗi. Chất lượng sản phẩm: <strong class='text-danger'>KHÔNG ĐẠT (LỖI)</strong>"
                            : "Chu kỳ hoàn tất. Chất lượng sản phẩm: <strong class='text-success'>ĐẠT</strong>"));

                return Json(new {
                    batchId = selectedBatchId,
                    batchName = batchName,
                    resolvedRunId = selectedRunId,
                    batchDate = batchDateStr,
                    cycleSummary = cycleSummary,
                    phases = stepsList,
                    events = eventsList,
                    note = note
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public FileResult ExportEventExcel(string starttime, string endtime, string batchId, string runId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var connector = new Hino.DatabaseConnector.MySQLConnect()
            {
                ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
            };

            var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, batchId, runId, starttime);
            int selectedBatchId = resolution.BatchId;
            int selectedRunId = resolution.RunId;

            var list = new List<EventExportDto>();
            if (selectedRunId > 0)
            {
                var dtAlarms = connector.ExecuteQuery($"SELECT DateTime, CongDoan, Message, Severity FROM realtime_alarms WHERE runId = {selectedRunId} ORDER BY DateTime ASC, id ASC");
                if (dtAlarms != null)
                {
                    foreach (DataRow row in dtAlarms.Rows)
                    {
                        DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                        string severity = row["Severity"].ToString();
                        string msg = row["Message"].ToString();
                        string cd = row["CongDoan"] != DBNull.Value ? row["CongDoan"].ToString().Trim() : "";

                        if (cd.Equals("T001", StringComparison.OrdinalIgnoreCase)) cd = "Cấp liệu";
                        else if (cd.Equals("T002", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 1";
                        else if (cd.Equals("T003", StringComparison.OrdinalIgnoreCase)) cd = "Xả đáy";
                        else if (cd.Equals("T004", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả đáy";
                        else if (cd.Equals("T005", StringComparison.OrdinalIgnoreCase)) cd = "Hút xả đáy";
                        else if (cd.Equals("T006", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 2";
                        else if (cd.Equals("T007", StringComparison.OrdinalIgnoreCase)) cd = "Xả hàng";
                        else if (cd.Equals("T008", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả hàng";

                        list.Add(new EventExportDto
                        {
                            ThoiGian = alarmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            CongDoan = cd,
                            SuKien = msg,
                            MucDo = severity.ToUpper()
                        });
                    }
                }
            }
            else if (selectedBatchId > 0)
            {
                var dtAlarms = connector.ExecuteQuery($"SELECT DateTime, CongDoan, Message, Severity FROM realtime_alarms WHERE batchId = {selectedBatchId} ORDER BY DateTime ASC, id ASC");
                if (dtAlarms != null)
                {
                    foreach (DataRow row in dtAlarms.Rows)
                    {
                        DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                        string severity = row["Severity"].ToString();
                        string msg = row["Message"].ToString();
                        string cd = row["CongDoan"] != DBNull.Value ? row["CongDoan"].ToString().Trim() : "";

                        if (cd.Equals("T001", StringComparison.OrdinalIgnoreCase)) cd = "Cấp liệu";
                        else if (cd.Equals("T002", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 1";
                        else if (cd.Equals("T003", StringComparison.OrdinalIgnoreCase)) cd = "Xả đáy";
                        else if (cd.Equals("T004", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả đáy";
                        else if (cd.Equals("T005", StringComparison.OrdinalIgnoreCase)) cd = "Hút xả đáy";
                        else if (cd.Equals("T006", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 2";
                        else if (cd.Equals("T007", StringComparison.OrdinalIgnoreCase)) cd = "Xả hàng";
                        else if (cd.Equals("T008", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả hàng";

                        list.Add(new EventExportDto
                        {
                            ThoiGian = alarmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            CongDoan = cd,
                            SuKien = msg,
                            MucDo = severity.ToUpper()
                        });
                    }
                }
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToExcel(list, "Events");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Events_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        public FileResult ExportEventCsv(string starttime, string endtime, string batchId, string runId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var connector = new Hino.DatabaseConnector.MySQLConnect()
            {
                ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
            };

            var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, batchId, runId, starttime);
            int selectedBatchId = resolution.BatchId;
            int selectedRunId = resolution.RunId;

            var list = new List<EventExportDto>();

            if (selectedRunId > 0)
            {
                var dtAlarms = connector.ExecuteQuery($"SELECT DateTime, CongDoan, Message, Severity FROM realtime_alarms WHERE runId = {selectedRunId} ORDER BY DateTime ASC, id ASC");
                if (dtAlarms != null)
                {
                    foreach (DataRow row in dtAlarms.Rows)
                    {
                        DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                        string severity = row["Severity"].ToString();
                        string msg = row["Message"].ToString();
                        string cd = row["CongDoan"] != DBNull.Value ? row["CongDoan"].ToString().Trim() : "";

                        if (cd.Equals("T001", StringComparison.OrdinalIgnoreCase)) cd = "Cấp liệu";
                        else if (cd.Equals("T002", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 1";
                        else if (cd.Equals("T003", StringComparison.OrdinalIgnoreCase)) cd = "Xả đáy";
                        else if (cd.Equals("T004", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả đáy";
                        else if (cd.Equals("T005", StringComparison.OrdinalIgnoreCase)) cd = "Hút xả đáy";
                        else if (cd.Equals("T006", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 2";
                        else if (cd.Equals("T007", StringComparison.OrdinalIgnoreCase)) cd = "Xả hàng";
                        else if (cd.Equals("T008", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả hàng";

                        list.Add(new EventExportDto
                        {
                            ThoiGian = alarmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            CongDoan = cd,
                            SuKien = msg,
                            MucDo = severity.ToUpper()
                        });
                    }
                }
            }
            else if (selectedBatchId > 0)
            {
                var dtAlarms = connector.ExecuteQuery($"SELECT DateTime, CongDoan, Message, Severity FROM realtime_alarms WHERE batchId = {selectedBatchId} ORDER BY DateTime ASC, id ASC");
                if (dtAlarms != null)
                {
                    foreach (DataRow row in dtAlarms.Rows)
                    {
                        DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                        string severity = row["Severity"].ToString();
                        string msg = row["Message"].ToString();
                        string cd = row["CongDoan"] != DBNull.Value ? row["CongDoan"].ToString().Trim() : "";

                        if (cd.Equals("T001", StringComparison.OrdinalIgnoreCase)) cd = "Cấp liệu";
                        else if (cd.Equals("T002", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 1";
                        else if (cd.Equals("T003", StringComparison.OrdinalIgnoreCase)) cd = "Xả đáy";
                        else if (cd.Equals("T004", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả đáy";
                        else if (cd.Equals("T005", StringComparison.OrdinalIgnoreCase)) cd = "Hút xả đáy";
                        else if (cd.Equals("T006", StringComparison.OrdinalIgnoreCase)) cd = "Trộn 2";
                        else if (cd.Equals("T007", StringComparison.OrdinalIgnoreCase)) cd = "Xả hàng";
                        else if (cd.Equals("T008", StringComparison.OrdinalIgnoreCase)) cd = "Rung xả hàng";

                        list.Add(new EventExportDto
                        {
                            ThoiGian = alarmTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            CongDoan = cd,
                            SuKien = msg,
                            MucDo = severity.ToUpper()
                        });
                    }
                }
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToCsv(list);
            return File(fileBytes, "text/csv", $"Events_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        // ================= HELPERS =================

        private static string RemoveSign4VietnameseString(string str)

        {

            if (string.IsNullOrEmpty(str)) return "";

            string[] signedPattern = new string[]

            {

                "aàảãáạăằẳẵắặâầẩẫấậ",

                "dđ",

                "eèẻẽéẹêềểễếệ",

                "iìỉĩíị",

                "oòỏõóọôồổỗốộơờởỡớợ",

                "uùủũúụưừửữứự",

                "yỳỷỹýỵ",

                "AÀẢÃÁẠĂẰẲẴẮẶÂẦẨẪẤẬ",

                "DĐ",

                "EÈẺẼÉẸÊỀỂỄẾỆ",

                "IÌỈĨÍỊ",

                "OÒỎÕÓỌÔỒỔỖỐỘƠỜỞỠỚỢ",

                "UÙỦŨÚỤƯỪỬỮỨỰ",

                "YỲỶỸÝÝ"

            };

            string[] unsignedReplacement = new string[]

            {

                "a", "d", "e", "i", "o", "u", "y",

                "A", "D", "E", "I", "O", "U", "Y"

            };

            for (int i = 0; i < signedPattern.Length; i++)

            {

                for (int j = 0; j < signedPattern[i].Length; j++)

                {

                    str = str.Replace(signedPattern[i][j], unsignedReplacement[i][0]);

                }

            }

            return str;

        }



        private static string FormatDuration(double totalSeconds)

        {

            TimeSpan t = TimeSpan.FromSeconds(totalSeconds);

            if (t.Hours > 0)

            {

                return string.Format("{0}h {1}m {2}s", t.Hours, t.Minutes, t.Seconds);

            }

            return string.Format("{0}m {1}s", t.Minutes, t.Seconds);

        }

    }



    public class EventExportDto

    {

        public string ThoiGian { get; set; }

        public string CongDoan { get; set; }

        public string SuKien { get; set; }

        public string MucDo { get; set; }

    }

}

