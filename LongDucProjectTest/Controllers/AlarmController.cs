using CsvHelper;
using Hino.Getdata.Common;
using Microsoft.Win32;
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
using System.Windows.Forms;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Hino.DatabaseConnector;
using System.Data;

namespace LongDucProject.Controllers
{
    public class AlarmController : Controller
    {
        [HttpGet]
        // GET: Alarm
        public JsonResult GetAlarmLog(string starttime, string endtime)
        {
            var Alarm = new AlarmCommon();
            var list = Alarm.GetAlarmLog(starttime, endtime).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBatches(string starttime, string endtime)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };
                string query = "SELECT id, name FROM batches ORDER BY id DESC";
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                bool hasFilter = false;

                if (!string.IsNullOrEmpty(starttime) && DateTime.TryParseExact(starttime, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out startDate))
                {
                    hasFilter = true;
                }
                if (!string.IsNullOrEmpty(endtime) && DateTime.TryParseExact(endtime, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out endDate))
                {
                    if (endDate.TimeOfDay == TimeSpan.Zero)
                    {
                        endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    }
                    hasFilter = true;
                }

                if (hasFilter)
                {
                    query = $"SELECT id, name FROM batches WHERE start_time >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND start_time <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}' ORDER BY id DESC";
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
                            name = row["name"].ToString()
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

        [HttpPost]
        public JsonResult GetAlarmsData(string starttime, string endtime, string batchId, int draw, int start, int length, bool? isInitialLoad)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };

                int parsedBatchId = 0;
                bool hasBatchFilter = int.TryParse(batchId, out parsedBatchId) && parsedBatchId > 0;

                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

                int resolvedBatchId = 0;
                string batchDateStart = "";
                string batchDateEnd = "";

                if (isInitialLoad == true)
                {
                    var dtActive = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Active' LIMIT 1");
                    if (dtActive != null && dtActive.Rows.Count > 0)
                    {
                        resolvedBatchId = Convert.ToInt32(dtActive.Rows[0]["id"]);
                    }
                    else
                    {
                        var dtCompleted = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Completed' ORDER BY id DESC LIMIT 1");
                        if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                        {
                            resolvedBatchId = Convert.ToInt32(dtCompleted.Rows[0]["id"]);
                        }
                        else
                        {
                            var dtLatest = connector.ExecuteQuery("SELECT id FROM batches ORDER BY id DESC LIMIT 1");
                            if (dtLatest != null && dtLatest.Rows.Count > 0)
                            {
                                resolvedBatchId = Convert.ToInt32(dtLatest.Rows[0]["id"]);
                            }
                        }
                    }

                    if (resolvedBatchId > 0)
                    {
                        var dtBatchInfo = connector.ExecuteQuery($"SELECT start_time, end_time FROM batches WHERE id = {resolvedBatchId}");
                        if (dtBatchInfo != null && dtBatchInfo.Rows.Count > 0)
                        {
                            if (dtBatchInfo.Rows[0]["start_time"] != DBNull.Value)
                            {
                                startDate = Convert.ToDateTime(dtBatchInfo.Rows[0]["start_time"]);
                                batchDateStart = startDate.ToString("yyyy/MM/dd");
                            }
                            if (dtBatchInfo.Rows[0]["end_time"] != DBNull.Value)
                            {
                                endDate = Convert.ToDateTime(dtBatchInfo.Rows[0]["end_time"]);
                                batchDateEnd = endDate.ToString("yyyy/MM/dd");
                            }
                            else
                            {
                                endDate = startDate;
                                batchDateEnd = batchDateStart;
                            }
                        }
                        hasBatchFilter = true;
                        parsedBatchId = resolvedBatchId;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(starttime))
                    {
                        if (!DateTime.TryParse(starttime, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                        {
                            DateTime.TryParseExact(starttime, new[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                        }
                    }

                    if (!string.IsNullOrEmpty(endtime))
                    {
                        if (!DateTime.TryParse(endtime, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                        {
                            DateTime.TryParseExact(endtime, new[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                        }
                        if (endDate.TimeOfDay == TimeSpan.Zero)
                        {
                            endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                        }
                    }
                }

                string baseQuery = "FROM realtime_alarms a INNER JOIN batches b ON a.batchId = b.id WHERE a.Severity IN ('ALARM', 'WARNING')";
                string filterQuery = "";

                if (isInitialLoad == true && resolvedBatchId > 0)
                {
                    filterQuery = $" AND a.batchId = {resolvedBatchId}";
                }
                else
                {
                    filterQuery = $" AND a.DateTime >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND a.DateTime <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    if (hasBatchFilter)
                    {
                        filterQuery += $" AND a.batchId = {parsedBatchId}";
                    }
                }

                string countQuery = $"SELECT COUNT(*) {baseQuery} {filterQuery}";
                int recordsFiltered = Convert.ToInt32(connector.ExecuteScalarQuery(countQuery));
                int recordsTotal = recordsFiltered;

                string dataQuery = $"SELECT a.id, a.DateTime, a.restore_time, a.Message, a.Severity, a.TagName, b.name AS BatchName {baseQuery} {filterQuery} ORDER BY a.DateTime DESC, a.id DESC LIMIT {length} OFFSET {start}";

                var data = new List<object>();
                var dt = connector.ExecuteQuery(dataQuery);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var occurrenceTimeVal = row["DateTime"] != DBNull.Value ? Convert.ToDateTime(row["DateTime"]) : DateTime.MinValue;
                        var restoreTimeVal = row["restore_time"] != DBNull.Value ? Convert.ToDateTime(row["restore_time"]) : (DateTime?)null;

                        string occurrenceTimeStr = occurrenceTimeVal != DateTime.MinValue ? occurrenceTimeVal.ToString("yyyy/MM/dd HH:mm:ss") : "-";
                        string restoreTimeStr = restoreTimeVal.HasValue ? restoreTimeVal.Value.ToString("yyyy/MM/dd HH:mm:ss") : "-";
                        string statusStr = restoreTimeVal.HasValue ? "Đã khôi phục" : "Cảnh báo";

                        data.Add(new
                        {
                            BatchId = row["BatchName"].ToString(),
                            OccurrenceTime = occurrenceTimeStr,
                            RestoreTime = restoreTimeStr,
                            Description = row["Message"].ToString(),
                            Status = statusStr
                        });
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = data,
                    resolvedBatchId = resolvedBatchId,
                    batchDateStart = batchDateStart,
                    batchDateEnd = batchDateEnd
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult GetAlarmReportData(string starttime, string endtime, string batchId, int draw, int start, int length)
        {
            // TODO: Replace with actual query to `alarmreport` table if different from `realtime_alarms`.
            // Currently using GetAlarmLog as a fallback placeholder.
            var Alarm = new AlarmCommon();
            var resultList = Alarm.GetAlarmLog(starttime, endtime);
            var list = resultList != null ? resultList.ToList() : new List<Hino.Parameter.Common.AlarmParameter>();

            var searchValue = Request.Form["search[value]"];
            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.TagNo != null && x.TagNo.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            int recordsTotal = list.Count;
            var data = list.Skip(start).Take(length).ToList();

            return Json(new {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = data
            });
        }

        [HttpGet]
        public FileResult ExportAlarmsExcel(string starttime, string endtime, string batchId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var connector = new MySQLConnect()
            {
                ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
            };

            int parsedBatchId = 0;
            bool hasBatchFilter = int.TryParse(batchId, out parsedBatchId) && parsedBatchId > 0;

            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            if (!string.IsNullOrEmpty(starttime))
            {
                if (!DateTime.TryParse(starttime, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    DateTime.TryParseExact(starttime, new[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                }
            }

            if (!string.IsNullOrEmpty(endtime))
            {
                if (!DateTime.TryParse(endtime, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    DateTime.TryParseExact(endtime, new[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                }
                if (endDate.TimeOfDay == TimeSpan.Zero)
                {
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                }
            }

            string baseQuery = "FROM realtime_alarms a INNER JOIN batches b ON a.batchId = b.id WHERE a.Severity IN ('ALARM', 'WARNING')";
            string filterQuery = $" AND a.DateTime >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND a.DateTime <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (hasBatchFilter)
            {
                filterQuery += $" AND a.batchId = {parsedBatchId}";
            }

            string query = $"SELECT a.id, a.DateTime, a.restore_time, a.Message, a.Severity, a.TagName, b.name AS BatchName {baseQuery} {filterQuery} ORDER BY a.DateTime DESC, a.id DESC";

            var list = new List<AlarmExportDto>();
            var dt = connector.ExecuteQuery(query);
            if (dt != null)
            {
                int index = 1;
                foreach (DataRow row in dt.Rows)
                {
                    var occurrenceTimeVal = row["DateTime"] != DBNull.Value ? Convert.ToDateTime(row["DateTime"]) : DateTime.MinValue;
                    var restoreTimeVal = row["restore_time"] != DBNull.Value ? Convert.ToDateTime(row["restore_time"]) : (DateTime?)null;

                    string occurrenceTimeStr = occurrenceTimeVal != DateTime.MinValue ? occurrenceTimeVal.ToString("yyyy/MM/dd HH:mm:ss") : "-";
                    string restoreTimeStr = restoreTimeVal.HasValue ? restoreTimeVal.Value.ToString("yyyy/MM/dd HH:mm:ss") : "-";
                    string statusStr = restoreTimeVal.HasValue ? "Đã khôi phục" : "Cảnh báo";

                    list.Add(new AlarmExportDto
                    {
                        STT = index++,
                        Batch = row["BatchName"].ToString(),
                        OccurrenceTime = occurrenceTimeStr,
                        RestoreTime = restoreTimeStr,
                        Description = row["Message"].ToString(),
                        Status = statusStr
                    });
                }
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToExcel(list, "Alarms");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Alarms_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        public FileResult ExportAlarmsCsv(string starttime, string endtime, string batchId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var connector = new MySQLConnect()
            {
                ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
            };

            int parsedBatchId = 0;
            bool hasBatchFilter = int.TryParse(batchId, out parsedBatchId) && parsedBatchId > 0;

            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

            if (!string.IsNullOrEmpty(starttime))
            {
                if (!DateTime.TryParse(starttime, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                {
                    DateTime.TryParseExact(starttime, new[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                }
            }

            if (!string.IsNullOrEmpty(endtime))
            {
                if (!DateTime.TryParse(endtime, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    DateTime.TryParseExact(endtime, new[] { "yyyy/MM/dd", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                }
                if (endDate.TimeOfDay == TimeSpan.Zero)
                {
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                }
            }

            string baseQuery = "FROM realtime_alarms a INNER JOIN batches b ON a.batchId = b.id WHERE a.Severity IN ('ALARM', 'WARNING')";
            string filterQuery = $" AND a.DateTime >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND a.DateTime <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (hasBatchFilter)
            {
                filterQuery += $" AND a.batchId = {parsedBatchId}";
            }

            string query = $"SELECT a.id, a.DateTime, a.restore_time, a.Message, a.Severity, a.TagName, b.name AS BatchName {baseQuery} {filterQuery} ORDER BY a.DateTime DESC, a.id DESC";

            var list = new List<AlarmExportDto>();
            var dt = connector.ExecuteQuery(query);
            if (dt != null)
            {
                int index = 1;
                foreach (DataRow row in dt.Rows)
                {
                    var occurrenceTimeVal = row["DateTime"] != DBNull.Value ? Convert.ToDateTime(row["DateTime"]) : DateTime.MinValue;
                    var restoreTimeVal = row["restore_time"] != DBNull.Value ? Convert.ToDateTime(row["restore_time"]) : (DateTime?)null;

                    string occurrenceTimeStr = occurrenceTimeVal != DateTime.MinValue ? occurrenceTimeVal.ToString("yyyy/MM/dd HH:mm:ss") : "-";
                    string restoreTimeStr = restoreTimeVal.HasValue ? restoreTimeVal.Value.ToString("yyyy/MM/dd HH:mm:ss") : "-";
                    string statusStr = restoreTimeVal.HasValue ? "Đã khôi phục" : "Cảnh báo";

                    list.Add(new AlarmExportDto
                    {
                        STT = index++,
                        Batch = row["BatchName"].ToString(),
                        OccurrenceTime = occurrenceTimeStr,
                        RestoreTime = restoreTimeStr,
                        Description = row["Message"].ToString(),
                        Status = statusStr
                    });
                }
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToCsv(list);
            return File(fileBytes, "text/csv", $"Alarms_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        [HttpGet]
        public FileResult ExportAlarmReportExcel(string starttime, string endtime, string batchId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var Alarm = new AlarmCommon();
            var list = Alarm.GetAlarmLog(starttime, endtime) ?? new List<Hino.Parameter.Common.AlarmParameter>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.TagNo != null && x.TagNo.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToExcel(list, "AlarmReport");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AlarmReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        public FileResult ExportAlarmReportCsv(string starttime, string endtime, string batchId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var Alarm = new AlarmCommon();
            var list = Alarm.GetAlarmLog(starttime, endtime) ?? new List<Hino.Parameter.Common.AlarmParameter>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.TagNo != null && x.TagNo.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToCsv(list);
            return File(fileBytes, "text/csv", $"AlarmReport_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        //Xuất dữ liệu ra file Excel
        public ActionResult GetdataExportAlarm(string starttime, string endtime, string filepath)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var Alarm = new AlarmCommon();
            var list = Alarm.GetAlarmLog(starttime, endtime);
            var x = filepath;
            var listExcel = Alarm.GetAlarmLog(starttime, endtime).ToList();
            try
            {

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlxs"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\AlarmReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["Alarm"];

                    int rowstart = 11;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.OccurrenceTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.RestoreTime;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.TagNo;
                        ws.Cells[string.Format("D{0}", rowstart)].Value = item.Location;
                        ws.Cells[string.Format("E{0}", rowstart)].Value = item.FaultCode;
                        ws.Cells[string.Format("F{0}", rowstart)].Value = item.Description;
                        ws.Cells[string.Format("G{0}", rowstart)].Value = item.Status;
     

                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    //pck.Save();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"Alarm_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Xuất dữ liệu ra file CSV
        public void GetdataCSVExportAlarm(string starttime, string endtime, string filepath)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

            var Alarm = new AlarmCommon();
            var listExcel = Alarm.GetAlarmLog(starttime, endtime).ToList();
            try
            {

                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("OccurrenceTime");
                    csv.WriteField("RestoreTime");
                    csv.WriteField("PlantName");
                    csv.WriteField("Device");
                    csv.WriteField("FaultCode");
                    csv.WriteField("Description");
                    csv.WriteField("Status");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.OccurrenceTime);
                        csv.WriteField(item.RestoreTime);
                        csv.WriteField(item.TagNo);
                        csv.WriteField(item.Location);
                        csv.WriteField(item.FaultCode);
                        csv.WriteField(item.Description);
                        csv.WriteField(item.Status);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"Alarm_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult Download()
        {

            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", $@"{ Session["ReportName"]}");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }

    public class AlarmExportDto
    {
        public int STT { get; set; }
        public string Batch { get; set; }
        public string OccurrenceTime { get; set; }
        public string RestoreTime { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}