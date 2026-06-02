using CsvHelper;
using Hino.GetData.Common;
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
using Hino.DatabaseConnector;
using System.Data;

namespace LongDucProject.Controllers
{
    public class OverviewController : Controller
    {
        string unit;
        string time;
        [HttpGet]
        // GET: Overview

        //Get data for energy chart (for OverView page)
        public JsonResult GetCommonSolarEnergy(int timeUnit, string starttime, string endtime)
        {
            var Energy = new EnergyPowerData();
            var list = Energy.GetSolarEnergy(timeUnit, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //Get data for energy Export Excel (for OverView page)
        public JsonResult GetdataExportCommonEnergy(int timeUnit, string starttime, string endtime, string filepath)
        {
            var Energy = new EnergyPowerData();
            var list = Energy.GetSolarEnergy(timeUnit, starttime, endtime);
            var x = filepath;
            var templatepath = "";
            var listExcel = Energy.GetSolarEnergy(timeUnit, starttime, endtime).ToList();

            try
            {
                if (timeUnit == 1)
                {
                    templatepath = "DailyEnergyTotalReport";
                    unit = "Day";
                    time = starttime.Substring(0, 10);
                }
                else if (timeUnit == 2)
                {
                    templatepath = "MonthlyEnergyTotalReport";
                    unit = "Month";
                    time = starttime.Substring(0, 7);
                }
                else if (timeUnit == 3)
                {
                    templatepath = "YearlyEnergyTotalReport";
                    unit = "Year";
                    time = starttime.Substring(0, 4);
                }


                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlxs"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\{templatepath}.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];

                    int rowstart = 3;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.SolarValue;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.GridValue;

                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    //pck.Save();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"Total Energy_{unit}_{time}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

       

        public void GetdataCSVCommonEnergy(int timeUnit, string starttime, string endtime, string filepath)
        {
            var Energy = new EnergyPowerData();
            var listExcel = Energy.GetSolarEnergy(timeUnit, starttime, endtime).ToList();

            try
            {
                string unit = "";
                string time = "";

                if (timeUnit == 1)
                {
                    unit = "Day";
                    time = starttime.Substring(0, 10);
                }
                else if (timeUnit == 2)
                {
                    unit = "Month";
                    time = starttime.Substring(0, 7);
                }
                else if (timeUnit == 3)
                {
                    unit = "Year";
                    time = starttime.Substring(0, 4);
                }

                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Solar Energy");
                    csv.WriteField("Self Consumption Energy");
                    csv.NextRecord();

                    foreach (var item in listExcel)
                    {

                        string formattedDateTime = GetFormattedDateTime(item.DateTime, timeUnit);
                        csv.WriteField(formattedDateTime);
                        string formattedSolarValue = Convert.ToInt32(item.SolarValue).ToString("N0"); // Add thousands separator
                        string formattedGridValue = Convert.ToInt32(item.GridValue).ToString("N0"); // Add thousands separator
                        csv.WriteField(formattedSolarValue);
                        csv.WriteField(formattedGridValue);
                        csv.NextRecord();
                    }
                }

                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"Total Energy_{unit}_{time}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetFormattedDateTime(DateTime dateTime, int timeUnit)
        {
            if (timeUnit == 1)
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm");
            }
            else if (timeUnit == 2)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
            else if (timeUnit == 3)
            {
                return dateTime.ToString("yyyy-MM");
            }

            return string.Empty;
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

        //Get data for PowerChart (include for chart and export Excel)
        [HttpGet]
        public JsonResult GetCommonSolarPower(string datetime)
        {
            var Power = new EnergyPowerData();
            var list = Power.GetSolarPower(datetime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Xuất dữ liệu dạng excel
        public JsonResult GetdataExportCommonPower(string datetime, string filepath)
        {
            var Power = new EnergyPowerData();

            var listExcel = Power.GetSolarPower(datetime).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\ElectricalPowerSolarTotalReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.SolarValue;
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"TotalPower_{datetime}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Xuất dữ liệu dang csv
        public void GetdataCSVCommonPower(string datetime, string filepath)
        {
            var Power = new EnergyPowerData();
            var listExcel = Power.GetSolarPower(datetime).ToList();

            try
            {
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Power");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.SolarValue);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"TotalPower_{datetime}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadPowerOverview()
        {

            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", $@"{Session["ReportName"]}");
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpGet]
        public JsonResult GetCurrentBatchStats(int? runId = null)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
                };

                int resolvedRunId = -1;
                int resolvedBatchId = -1;
                string runName = "";
                string runStatus = "";
                string runStart = "";
                string runEnd = "";
                string batchName = "";
                string batchStatus = "";
                string batchStart = "";
                string batchEnd = "";

                if (runId.HasValue && runId.Value > 0)
                {
                    // 1. Operator clicked a specific run tab
                    var dtRun = connector.ExecuteQuery($"SELECT id, batch_id, name, status, start_time, end_time FROM runs WHERE id = {runId.Value} LIMIT 1");
                    if (dtRun != null && dtRun.Rows.Count > 0)
                    {
                        resolvedRunId = Convert.ToInt32(dtRun.Rows[0]["id"]);
                        resolvedBatchId = Convert.ToInt32(dtRun.Rows[0]["batch_id"]);
                        runName = dtRun.Rows[0]["name"] != DBNull.Value ? dtRun.Rows[0]["name"].ToString() : "";
                        runStatus = dtRun.Rows[0]["status"] != DBNull.Value ? dtRun.Rows[0]["status"].ToString() : "";
                        runStart = dtRun.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtRun.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        runEnd = dtRun.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtRun.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    }
                }

                if (resolvedRunId == -1)
                {
                    // 2. Default: Resolve active run
                    var dtActiveRun = connector.ExecuteQuery("SELECT id, batch_id, name, status, start_time, end_time FROM runs WHERE status = 'Active' ORDER BY id DESC LIMIT 1");
                    if (dtActiveRun != null && dtActiveRun.Rows.Count > 0)
                    {
                        resolvedRunId = Convert.ToInt32(dtActiveRun.Rows[0]["id"]);
                        resolvedBatchId = Convert.ToInt32(dtActiveRun.Rows[0]["batch_id"]);
                        runName = dtActiveRun.Rows[0]["name"] != DBNull.Value ? dtActiveRun.Rows[0]["name"].ToString() : "";
                        runStatus = dtActiveRun.Rows[0]["status"] != DBNull.Value ? dtActiveRun.Rows[0]["status"].ToString() : "";
                        runStart = dtActiveRun.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtActiveRun.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        runEnd = dtActiveRun.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtActiveRun.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    }
                    else
                    {
                        // Fallback to the latest run of any status
                        var dtLatestRun = connector.ExecuteQuery("SELECT id, batch_id, name, status, start_time, end_time FROM runs ORDER BY id DESC LIMIT 1");
                        if (dtLatestRun != null && dtLatestRun.Rows.Count > 0)
                        {
                            resolvedRunId = Convert.ToInt32(dtLatestRun.Rows[0]["id"]);
                            resolvedBatchId = Convert.ToInt32(dtLatestRun.Rows[0]["batch_id"]);
                            runName = dtLatestRun.Rows[0]["name"] != DBNull.Value ? dtLatestRun.Rows[0]["name"].ToString() : "";
                            runStatus = dtLatestRun.Rows[0]["status"] != DBNull.Value ? dtLatestRun.Rows[0]["status"].ToString() : "";
                            runStart = dtLatestRun.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtLatestRun.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                            runEnd = dtLatestRun.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtLatestRun.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        }
                    }
                }

                // Get the batch info using resolvedBatchId
                if (resolvedBatchId != -1)
                {
                    var dtBatch = connector.ExecuteQuery($"SELECT name, status, start_time, end_time FROM batches WHERE id = {resolvedBatchId} LIMIT 1");
                    if (dtBatch != null && dtBatch.Rows.Count > 0)
                    {
                        batchName = dtBatch.Rows[0]["name"] != DBNull.Value ? dtBatch.Rows[0]["name"].ToString() : "";
                        batchStatus = dtBatch.Rows[0]["status"] != DBNull.Value ? dtBatch.Rows[0]["status"].ToString() : "";
                        batchStart = dtBatch.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtBatch.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        batchEnd = dtBatch.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtBatch.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    }
                }

                // Fallback to old batch resolution if both are still not resolved (e.g. database completely empty of runs but has batches)
                if (resolvedBatchId == -1)
                {
                    var dtActive = connector.ExecuteQuery("SELECT id, name, status, start_time, end_time FROM batches WHERE status = 'Active' LIMIT 1");
                    DataTable dtBatchFallback = null;
                    if (dtActive != null && dtActive.Rows.Count > 0)
                    {
                        dtBatchFallback = dtActive;
                    }
                    else
                    {
                        var dtCompleted = connector.ExecuteQuery("SELECT id, name, status, start_time, end_time FROM batches WHERE status = 'Completed' ORDER BY id DESC LIMIT 1");
                        if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                        {
                            dtBatchFallback = dtCompleted;
                        }
                        else
                        {
                            var dtLatest = connector.ExecuteQuery("SELECT id, name, status, start_time, end_time FROM batches ORDER BY id DESC LIMIT 1");
                            if (dtLatest != null && dtLatest.Rows.Count > 0)
                            {
                                dtBatchFallback = dtLatest;
                            }
                        }
                    }

                    if (dtBatchFallback != null && dtBatchFallback.Rows.Count > 0)
                    {
                        resolvedBatchId = Convert.ToInt32(dtBatchFallback.Rows[0]["id"]);
                        batchName = dtBatchFallback.Rows[0]["name"] != DBNull.Value ? dtBatchFallback.Rows[0]["name"].ToString() : "";
                        batchStatus = dtBatchFallback.Rows[0]["status"] != DBNull.Value ? dtBatchFallback.Rows[0]["status"].ToString() : "";
                        batchStart = dtBatchFallback.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtBatchFallback.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        batchEnd = dtBatchFallback.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtBatchFallback.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    }
                }

                // 2. Fetch alarmlog for active run (fallback to batchId if no runId resolved)
                DataTable dtAlarmLog = null;
                if (resolvedRunId != -1)
                {
                    dtAlarmLog = connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE runId = {resolvedRunId}");
                }
                else if (resolvedBatchId != -1)
                {
                    dtAlarmLog = connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE batchId = {resolvedBatchId}");
                }

                // 3. Fetch alarmreport (telemetry) for active run
                DataTable dtTelemetry = null;
                if (resolvedRunId != -1)
                {
                    dtTelemetry = connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi FROM alarmreport WHERE runId = {resolvedRunId} ORDER BY DateTime ASC");
                }
                else if (resolvedBatchId != -1)
                {
                    dtTelemetry = connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi FROM alarmreport WHERE batchId = {resolvedBatchId} ORDER BY DateTime ASC");
                }

                // 4. Fetch realtime_alarms for active run
                DataTable dtAlarms = null;
                if (resolvedRunId != -1)
                {
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE runId = {resolvedRunId} AND Severity IN ('ALARM', 'WARNING') ORDER BY DateTime ASC, id ASC");
                }
                else if (resolvedBatchId != -1)
                {
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE batchId = {resolvedBatchId} AND Severity IN ('ALARM', 'WARNING') ORDER BY DateTime ASC, id ASC");
                }

                // 5. Set up standard steps with their alarmlog TagNo mapping & keywords
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

                var stepsList = new List<object>();
                var globalAlarms = new List<object>();

                var logRows = dtAlarmLog != null 
                    ? dtAlarmLog.AsEnumerable()
                                 .OrderByDescending(r => r["OccurrenceTime"] != DBNull.Value ? Convert.ToDateTime(r["OccurrenceTime"]) : DateTime.MinValue)
                                 .ToList() 
                    : new List<DataRow>();
                var telemetryRows = dtTelemetry != null ? dtTelemetry.AsEnumerable().ToList() : new List<DataRow>();
                var alarmRows = dtAlarms != null ? dtAlarms.AsEnumerable().ToList() : new List<DataRow>();

                // 5. Determine the active step and calculate header/panel metrics (pre-calculated to allow stepsList rendering)
                var activeLogRows = logRows.Where(r => r["Status"] != DBNull.Value && r["Status"].ToString().Trim().Equals("Alarm", StringComparison.OrdinalIgnoreCase)).ToList();
                int activeStepCode = 0;
                string activeStepName = "";
                DateTime? activeStepStartTime = null;

                if (activeLogRows.Count > 0)
                {
                    foreach (var def in stepDefs)
                    {
                        var match = activeLogRows.FirstOrDefault(r => {
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

                        if (match != null)
                        {
                            activeStepCode = def.Code;
                            activeStepName = def.Name;
                            activeStepStartTime = Convert.ToDateTime(match["OccurrenceTime"]);
                            break;
                        }
                    }
                }

                string headerStepName = activeStepName;
                if (batchStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    activeStepCode = 8;
                    headerStepName = "";
                    
                    var step8Row = logRows.FirstOrDefault(r => {
                        string rowTagNo = r.Table.Columns.Contains("TagNo") && r["TagNo"] != DBNull.Value ? r["TagNo"].ToString().Trim() : "";
                        if (!string.IsNullOrEmpty(rowTagNo))
                        {
                            return rowTagNo.Equals("T008", StringComparison.OrdinalIgnoreCase);
                        }
                        string desc = r["Description"] != DBNull.Value ? r["Description"].ToString() : "";
                        return desc.IndexOf("Rung Xả H", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa H", StringComparison.OrdinalIgnoreCase) >= 0;
                    });
                    if (step8Row != null)
                    {
                        activeStepStartTime = Convert.ToDateTime(step8Row["OccurrenceTime"]);
                    }
                }
                else if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && activeStepCode == 0)
                {
                    // Fallback Option 1: Query the latest CongDoanMay from alarmreport telemetry
                    int inferredCd = 0;
                    try
                    {
                        var dtTelemetryCD = connector.ExecuteQuery($"SELECT CongDoanMay FROM alarmreport WHERE runId = {resolvedRunId} ORDER BY id DESC LIMIT 1");
                        if (dtTelemetryCD != null && dtTelemetryCD.Rows.Count > 0 && dtTelemetryCD.Rows[0]["CongDoanMay"] != DBNull.Value)
                        {
                            inferredCd = Convert.ToInt32(dtTelemetryCD.Rows[0]["CongDoanMay"]);
                        }
                    }
                    catch { }

                    if (inferredCd > 0)
                    {
                        if (inferredCd == 1) activeStepCode = 1;
                        else if (inferredCd == 2) activeStepCode = 2;
                        else if (inferredCd == 3)
                        {
                            // In CongDoanMay=3, check which steps are resolved
                            bool isT004Resolved = logRows.Any(r => r["TagNo"] != DBNull.Value && r["TagNo"].ToString().Trim().Equals("T004", StringComparison.OrdinalIgnoreCase) && r["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase));
                            bool isT003Resolved = logRows.Any(r => r["TagNo"] != DBNull.Value && r["TagNo"].ToString().Trim().Equals("T003", StringComparison.OrdinalIgnoreCase) && r["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase));
                            if (isT004Resolved) activeStepCode = 5; // Hut Xa Day
                            else if (isT003Resolved) activeStepCode = 4; // Rung Xa Day
                            else activeStepCode = 3; // Xa Day
                        }
                        else if (inferredCd == 4) activeStepCode = 6;
                        else if (inferredCd == 5)
                        {
                            // In CongDoanMay=5, check which steps are resolved
                            bool isT007Resolved = logRows.Any(r => r["TagNo"] != DBNull.Value && r["TagNo"].ToString().Trim().Equals("T007", StringComparison.OrdinalIgnoreCase) && r["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase));
                            if (isT007Resolved) activeStepCode = 8; // Rung Xa Hang
                            else activeStepCode = 7; // Xa Hang
                        }
                    }

                    // Fallback Option 2: Sequential step deduction if telemetry has no logs yet
                    if (activeStepCode == 0)
                    {
                        int maxResolvedCode = 0;
                        foreach (var def in stepDefs)
                        {
                            var match = logRows.FirstOrDefault(r => {
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

                            if (match != null && match["Status"] != DBNull.Value && match["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase))
                            {
                                if (def.Code > maxResolvedCode)
                                {
                                    maxResolvedCode = def.Code;
                                }
                            }
                        }

                        if (maxResolvedCode < 8)
                        {
                            activeStepCode = maxResolvedCode + 1;
                        }
                    }

                    if (activeStepCode > 0)
                    {
                        var inferredDef = stepDefs.FirstOrDefault(d => d.Code == activeStepCode);
                        if (inferredDef != null)
                        {
                            activeStepName = inferredDef.Name;
                            headerStepName = inferredDef.Name;

                            // Set start time to the end time of the previous step if it was resolved
                            int prevStepCode = activeStepCode - 1;
                            var prevDef = stepDefs.FirstOrDefault(d => d.Code == prevStepCode);
                            if (prevDef != null)
                            {
                                var prevMatch = logRows.FirstOrDefault(r => {
                                    string rowTagNo = r.Table.Columns.Contains("TagNo") && r["TagNo"] != DBNull.Value ? r["TagNo"].ToString().Trim() : "";
                                    if (!string.IsNullOrEmpty(rowTagNo))
                                    {
                                        return rowTagNo.Equals(prevDef.TagNo, StringComparison.OrdinalIgnoreCase);
                                    }
                                    string desc = r["Description"] != DBNull.Value ? r["Description"].ToString() : "";
                                    if (prevDef.Code == 1 && (desc.IndexOf("Cấp Liệu", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Cap Lieu", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    if (prevDef.Code == 2 && (desc.IndexOf("Trộn 1", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Tron 1", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    if (prevDef.Code == 3 && (desc.IndexOf("Xả Đáy", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Xa Day", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    if (prevDef.Code == 4 && (desc.IndexOf("Rung Xả Đ", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa D", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    if (prevDef.Code == 5 && (desc.IndexOf("Hút Xả Đáy", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Hut Xa Day", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    if (prevDef.Code == 6 && (desc.IndexOf("Trộn 2", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Tron 2", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    if (prevDef.Code == 7 && (desc.IndexOf("Xả Hàng", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Xa Hang", StringComparison.OrdinalIgnoreCase) >= 0) && desc.IndexOf("Rung", StringComparison.OrdinalIgnoreCase) < 0) return true;
                                    if (prevDef.Code == 8 && (desc.IndexOf("Rung Xả H", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa H", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                                    return false;
                                });

                                if (prevMatch != null && prevMatch["RestoreTime"] != DBNull.Value)
                                {
                                    activeStepStartTime = Convert.ToDateTime(prevMatch["RestoreTime"]);
                                }
                            }
                        }
                    }

                    if (!activeStepStartTime.HasValue)
                    {
                        if (!string.IsNullOrEmpty(runStart))
                        {
                            activeStepStartTime = Convert.ToDateTime(runStart);
                        }
                        else if (!string.IsNullOrEmpty(batchStart))
                        {
                            activeStepStartTime = Convert.ToDateTime(batchStart);
                        }
                    }
                }
                else if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && !activeStepStartTime.HasValue)
                {
                    if (!string.IsNullOrEmpty(runStart))
                    {
                        activeStepStartTime = Convert.ToDateTime(runStart);
                    }
                    else if (!string.IsNullOrEmpty(batchStart))
                    {
                        activeStepStartTime = Convert.ToDateTime(batchStart);
                    }
                }

                // Thresholds building removed as Time-Lag Compensation handles leakage without alarm thresholds


                foreach (var def in stepDefs)
                {
                    // Find matching log in alarmlog using TagNo first, with Keyword-based description matching as a fallback
                    var stepLogRow = logRows.FirstOrDefault(r => {
                        string rowTagNo = r.Table.Columns.Contains("TagNo") && r["TagNo"] != DBNull.Value ? r["TagNo"].ToString().Trim() : "";
                        
                        // If TagNo is explicitly populated in the database row, we MUST use it for strict matching
                        if (!string.IsNullOrEmpty(rowTagNo))
                        {
                            return rowTagNo.Equals(def.TagNo, StringComparison.OrdinalIgnoreCase);
                        }

                        // Fallback to keyword-based description matching ONLY when TagNo is not present
                        string desc = r["Description"] != DBNull.Value ? r["Description"].ToString() : "";
                        if (def.Code == 1 && (desc.IndexOf("Cấp Liệu", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Cap Lieu", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 2 && (desc.IndexOf("Trộn 1", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Tron 1", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 3 && (desc.IndexOf("Xả Đáy", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Xa Day", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 4 && (desc.IndexOf("Rung Xả Đ", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa D", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 5 && (desc.IndexOf("Hút Xả Đáy", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Hut Xa Day", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        if (def.Code == 6 && (desc.IndexOf("Trộn 2", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Tron 2", StringComparison.OrdinalIgnoreCase) >= 0)) return true;
                        
                        // For step 7 (Xả Hàng), exclude descriptions containing "Rung" (which belongs to step 8)
                        if (def.Code == 7 && 
                            (desc.IndexOf("Xả Hàng", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Xa Hang", StringComparison.OrdinalIgnoreCase) >= 0) &&
                            desc.IndexOf("Rung", StringComparison.OrdinalIgnoreCase) < 0) return true;
                            
                        if (def.Code == 8 && (desc.IndexOf("Rung Xả H", StringComparison.OrdinalIgnoreCase) >= 0 || desc.IndexOf("Rung Xa H", StringComparison.OrdinalIgnoreCase) >= 0)) return true;

                        return false;
                    });

                    if (stepLogRow == null)
                    {
                        if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && def.Code == activeStepCode)
                        {
                            // Inferred active step: treat as in-progress
                            stepsList.Add(new
                            {
                                process = def.Name,
                                standard = def.Standard,
                                start = activeStepStartTime.HasValue ? activeStepStartTime.Value.ToString("HH:mm:ss") : "-",
                                end = "-",
                                duration = "-",
                                tempTop = "-",
                                tempMid = "-",
                                tempBot = "-",
                                status = "in-progress",
                                statusText = "Đang thực hiện",
                                alerts = new List<object>()
                            });
                        }
                        else
                        {
                            // Step has not started yet
                            stepsList.Add(new
                            {
                                process = def.Name,
                                standard = def.Standard,
                                start = "-",
                                end = "-",
                                duration = "-",
                                tempTop = "-",
                                tempMid = "-",
                                tempBot = "-",
                                status = "pending",
                                statusText = "Chưa bắt đầu",
                                alerts = new List<object>()
                            });
                        }
                    }
                    else
                    {
                        // Step has started
                        DateTime startTime = Convert.ToDateTime(stepLogRow["OccurrenceTime"]);
                        string startStr = startTime.ToString("HH:mm:ss");
                        string endStr = "-";
                        string durationStr = "-";
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
                                endStr = endTime.Value.ToString("HH:mm:ss");
                                double totalSeconds = (endTime.Value - startTime).TotalSeconds;
                                durationStr = $"{(int)totalSeconds}s";
                            }
                        }

                        // Filter telemetry logs for this step's time range with Time-Lag Compensation (Option A)
                        int telemetryOffsetSeconds = -20;
                        var stepTelemetry = telemetryRows.Where(r => {
                            DateTime dt = Convert.ToDateTime(r["DateTime"]).AddSeconds(telemetryOffsetSeconds);
                            if (endTime.HasValue)
                            {
                                return dt >= startTime && dt <= endTime.Value;
                            }
                            else
                            {
                                return dt >= startTime;
                            }
                        }).ToList();

                        // Fallback for extremely short steps (e.g. 16s) to prevent data loss
                        if (stepTelemetry.Count == 0)
                        {
                            stepTelemetry = telemetryRows.Where(r => {
                                DateTime dt = Convert.ToDateTime(r["DateTime"]);
                                if (endTime.HasValue)
                                {
                                    return dt >= startTime && dt <= endTime.Value;
                                }
                                else
                                {
                                    return dt >= startTime;
                                }
                            }).ToList();
                        }

                        // Find alerts for this step using Option C (both code mapping and time-range overlap)
                        var stepAlertsList = new List<object>();
                        var stepAlarms = alarmRows.Where(r => {
                            // Check if alarm time falls within step range
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
                                // 1. Match by TagNo (T001 - T008)
                                if (cd.Equals(def.TagNo, StringComparison.OrdinalIgnoreCase)) codeMatches = true;

                                // 2. Match by Name (Cấp liệu, Trộn 1...)
                                else if (cd.Equals(def.Name, StringComparison.OrdinalIgnoreCase)) codeMatches = true;

                                // 3. Robust check: compare unaccented / lower-case variations
                                else
                                {
                                    string cdLower = RemoveSign4VietnameseString(cd).ToLower();
                                    string defNameLower = RemoveSign4VietnameseString(def.Name).ToLower();
                                    if (cdLower == defNameLower) codeMatches = true;

                                    // 4. Support partial keyword matching or alternate mapping (e.g. "Cap lieu" -> "cap lieu")
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

                        // Calculate temperatures (Bồn trên, Bồn giữa, Bồn dưới)
                        var topTemps = new List<double>();
                        var midTemps = new List<double>();
                        var botTemps = new List<double>();

                        foreach (var row in stepTelemetry)
                        {
                            if (row["NhietDoBonTronTren"] != DBNull.Value)
                            {
                                topTemps.Add(Convert.ToDouble(row["NhietDoBonTronTren"]));
                            }
                            if (row["NhietDoBonTronGiua"] != DBNull.Value)
                            {
                                midTemps.Add(Convert.ToDouble(row["NhietDoBonTronGiua"]));
                            }
                            if (row["NhietDoBonTronDuoi"] != DBNull.Value)
                            {
                                botTemps.Add(Convert.ToDouble(row["NhietDoBonTronDuoi"]));
                            }
                        }

                        // Inject real-time alarm peak values into temperature calculation (Chiều xuôi)
                        foreach (var row in stepAlarms)
                        {
                            string tagName = row["TagName"] != DBNull.Value ? row["TagName"].ToString() : "";
                            if (row["Value"] != DBNull.Value)
                            {
                                double val = Convert.ToDouble(row["Value"]);
                                if (tagName.IndexOf("NhietDoBonTronTren", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    topTemps.Add(val);
                                }
                                else if (tagName.IndexOf("NhietDoBonTronGiua", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    midTemps.Add(val);
                                }
                                else if (tagName.IndexOf("NhietDoBonTronDuoi", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    botTemps.Add(val);
                                }
                            }
                        }

                        double? topThreshold = null;
                        double? midThreshold = null;
                        double? botThreshold = null;

                        foreach (var row in stepAlarms)
                        {
                            string tagName = row["TagName"] != DBNull.Value ? row["TagName"].ToString() : "";
                            if (row["Threshold"] != DBNull.Value)
                            {
                                double thresh = Convert.ToDouble(row["Threshold"]);
                                if (tagName.IndexOf("NhietDoBonTronTren", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    topThreshold = thresh;
                                }
                                else if (tagName.IndexOf("NhietDoBonTronGiua", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    midThreshold = thresh;
                                }
                                else if (tagName.IndexOf("NhietDoBonTronDuoi", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    botThreshold = thresh;
                                }
                            }
                        }

                        string tempTopStr = FormatTempRange(topTemps, topThreshold);
                        string tempMidStr = FormatTempRange(midTemps, midThreshold);
                        string tempBotStr = FormatTempRange(botTemps, botThreshold);

                        foreach (var row in stepAlarms)
                        {
                            DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                            string severity = row["Severity"].ToString();
                            string tagName = row["TagName"].ToString();
                            double val = Convert.ToDouble(row["Value"]);
                            double threshold = Convert.ToDouble(row["Threshold"]);
                            string msg = row["Message"].ToString();

                            string unit = tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0 ? "°C" :
                                         tagName.IndexOf("ApSuat", StringComparison.OrdinalIgnoreCase) >= 0 ? "bar" : "";

                            string detailMessage = $"Giá trị: {val.ToString("0.#", CultureInfo.InvariantCulture)} {unit} (ngưỡng: {threshold.ToString("0.#", CultureInfo.InvariantCulture)} {unit})";

                            var alertObj = new
                            {
                                id = Convert.ToInt32(row["id"]),
                                time = alarmTime.ToString("HH:mm:ss"),
                                type = severity,
                                title = msg,
                                message = detailMessage
                            };

                            stepAlertsList.Add(alertObj);
                            globalAlarms.Add(alertObj);
                        }

                        stepsList.Add(new
                        {
                            process = def.Name,
                            standard = def.Standard,
                            start = startStr,
                            end = endStr,
                            duration = durationStr,
                            tempTop = tempTopStr,
                            tempMid = tempMidStr,
                            tempBot = tempBotStr,
                            status = status,
                            statusText = statusText,
                            alerts = stepAlertsList
                        });
                    }
                }

                // 6. Determine the active step and calculate header/panel metrics
                DataRow activeStepRow = null;
                // Determine active step metrics (already pre-calculated at the beginning of Step 5)

                // Calculate running time:
                // - Sum the durations of all steps (completed steps actual duration and currently running step's elapsed time)
                double runningSeconds = 0;
                if (resolvedRunId != -1 || resolvedBatchId != -1)
                {
                    foreach (var def in stepDefs)
                    {
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

                        if (stepLogRow != null)
                        {
                            DateTime startTime = Convert.ToDateTime(stepLogRow["OccurrenceTime"]);
                            string statusVal = stepLogRow["Status"].ToString().Trim();
                            bool isCompleted = statusVal.Equals("Resolved", StringComparison.OrdinalIgnoreCase);

                            if (isCompleted && stepLogRow["RestoreTime"] != DBNull.Value)
                            {
                                DateTime endTime = Convert.ToDateTime(stepLogRow["RestoreTime"]);
                                runningSeconds += (endTime - startTime).TotalSeconds;
                            }
                            else if (!isCompleted)
                            {
                                runningSeconds += (DateTime.Now - startTime).TotalSeconds;
                            }
                        }
                    }
                }
                if (runningSeconds < 0) runningSeconds = 0;

                // Calculate alarm count excluding INFO severity
                int alarmCount = 0;
                if (resolvedRunId != -1)
                {
                    var dtAlarmCount = connector.ExecuteQuery($"SELECT COUNT(*) FROM realtime_alarms WHERE runId = {resolvedRunId} AND Severity != 'INFO'");
                    if (dtAlarmCount != null && dtAlarmCount.Rows.Count > 0)
                    {
                        alarmCount = Convert.ToInt32(dtAlarmCount.Rows[0][0]);
                    }
                }
                else if (resolvedBatchId != -1)
                {
                    var dtAlarmCount = connector.ExecuteQuery($"SELECT COUNT(*) FROM realtime_alarms WHERE batchId = {resolvedBatchId} AND Severity != 'INFO'");
                    if (dtAlarmCount != null && dtAlarmCount.Rows.Count > 0)
                    {
                        alarmCount = Convert.ToInt32(dtAlarmCount.Rows[0][0]);
                    }
                }

                // Helper to format runningSeconds as raw seconds
                string headerRunningTimeStr = "0s";
                if (runningSeconds >= 0)
                {
                    headerRunningTimeStr = $"{(int)runningSeconds}s";
                }

                // Gather runs metadata for tab selector
                var runsList = new List<object>();
                if (resolvedBatchId != -1)
                {
                    var dtRuns = connector.ExecuteQuery($"SELECT id, run_number, name, status, start_time, end_time FROM runs WHERE batch_id = {resolvedBatchId} ORDER BY run_number ASC");
                    if (dtRuns != null)
                    {
                        foreach (DataRow row in dtRuns.Rows)
                        {
                            runsList.Add(new
                            {
                                id = Convert.ToInt32(row["id"]),
                                run_number = Convert.ToInt32(row["run_number"]),
                                name = row["name"].ToString(),
                                status = row["status"].ToString(),
                                start_time = row["start_time"] != DBNull.Value ? Convert.ToDateTime(row["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "",
                                end_time = row["end_time"] != DBNull.Value ? Convert.ToDateTime(row["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : ""
                            });
                        }
                    }
                }

                var batchInfo = new
                {
                    batchId = resolvedBatchId,
                    batchName = batchName,
                    batchStatus = batchStatus,
                    machineStatus = runStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "RUNNING" : "COMPLETED",
                    activeStepCode = activeStepCode,
                    activeStepName = activeStepName,
                    headerStepName = headerStepName,
                    activeStepStartTime = activeStepStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    batchStartTime = !string.IsNullOrEmpty(runStart) ? runStart : batchStart, // fallback to batch start
                    batchTotalSeconds = runningSeconds, // total elapsed seconds of the resolved run
                    headerRunningTime = headerRunningTimeStr,
                    alarmCount = alarmCount,
                    serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    
                    // Run specific metadata
                    runId = resolvedRunId,
                    runName = runName,
                    runStatus = runStatus,
                    runs = runsList
                };

                // Prevent duplicate global alarms and sort descending by database id, then take top 5
                var sortedGlobalAlarms = globalAlarms.Cast<dynamic>()
                    .GroupBy(a => new { a.time, a.title })
                    .Select(g => g.First())
                    .OrderByDescending(a => (int)a.id)
                    .Take(5)
                    .ToList();

                // 5. Fetch daily batches produced today
                var dailyBatchesList = new List<object>();
                var dtDaily = connector.ExecuteQuery($"SELECT id, name, status, start_time, end_time FROM batches WHERE DATE(created_at) = '{DateTime.Today.ToString("yyyy-MM-dd")}' OR DATE(start_time) = '{DateTime.Today.ToString("yyyy-MM-dd")}' ORDER BY id ASC");
                if (dtDaily != null)
                {
                    foreach (DataRow row in dtDaily.Rows)
                    {
                        int bId = Convert.ToInt32(row["id"]);
                        string bName = row["name"].ToString();
                        string bStatus = row["status"] != DBNull.Value ? row["status"].ToString() : "";
                        
                        DateTime? firstOccurrence = null;
                        DateTime? lastRestore = null;
                        var dtFirst = connector.ExecuteQuery($"SELECT OccurrenceTime FROM alarmlog WHERE batchId = {bId} ORDER BY OccurrenceTime ASC LIMIT 1");
                        if (dtFirst != null && dtFirst.Rows.Count > 0 && dtFirst.Rows[0][0] != DBNull.Value)
                        {
                            firstOccurrence = Convert.ToDateTime(dtFirst.Rows[0][0]);
                        }
                        var dtLast = connector.ExecuteQuery($"SELECT RestoreTime FROM alarmlog WHERE batchId = {bId} AND Status = 'Resolved' ORDER BY RestoreTime DESC LIMIT 1");
                        if (dtLast != null && dtLast.Rows.Count > 0 && dtLast.Rows[0][0] != DBNull.Value)
                        {
                            lastRestore = Convert.ToDateTime(dtLast.Rows[0][0]);
                        }
                        
                        string durationStr = "-";
                        if (bStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
                        {
                            if (row["start_time"] != DBNull.Value)
                            {
                                DateTime startTimeVal = Convert.ToDateTime(row["start_time"]);
                                durationStr = $"{startTimeVal.ToString("H:mm")} - {DateTime.Now.ToString("H:mm")}";
                            }
                        }
                        else
                        {
                            if (firstOccurrence.HasValue && lastRestore.HasValue)
                            {
                                durationStr = $"{firstOccurrence.Value.ToString("H:mm")} - {lastRestore.Value.ToString("H:mm")}";
                            }
                            else if (row["start_time"] != DBNull.Value && row["end_time"] != DBNull.Value)
                            {
                                DateTime startTimeVal = Convert.ToDateTime(row["start_time"]);
                                DateTime endTimeVal = Convert.ToDateTime(row["end_time"]);
                                durationStr = $"{startTimeVal.ToString("H:mm")} - {endTimeVal.ToString("H:mm")}";
                            }
                        }
                        
                        dailyBatchesList.Add(new
                        {
                            id = bId,
                            name = bName,
                            weight = "500kg",
                            duration = durationStr,
                            status = bStatus
                        });
                    }
                }

                return Json(new { steps = stepsList, globalAlarms = sortedGlobalAlarms, batchInfo = batchInfo, dailyBatches = dailyBatchesList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRecentAlarms()
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
                };

                // 1. Get active run
                var dtActive = connector.ExecuteQuery("SELECT id FROM runs WHERE status = 'Active' LIMIT 1");
                int runId = -1;
                if (dtActive != null && dtActive.Rows.Count > 0)
                {
                    runId = Convert.ToInt32(dtActive.Rows[0]["id"]);
                }
                else
                {
                    // Fallback to the most recently completed run
                    var dtCompleted = connector.ExecuteQuery("SELECT id FROM runs WHERE status = 'Completed' ORDER BY id DESC LIMIT 1");
                    if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                    {
                        runId = Convert.ToInt32(dtCompleted.Rows[0]["id"]);
                    }
                    else
                    {
                        // Final fallback to the latest run of any status
                        var dtLatest = connector.ExecuteQuery("SELECT id FROM runs ORDER BY id DESC LIMIT 1");
                        if (dtLatest != null && dtLatest.Rows.Count > 0)
                        {
                            runId = Convert.ToInt32(dtLatest.Rows[0]["id"]);
                        }
                    }
                }

                if (runId == -1)
                {
                    return Json(new List<object>(), JsonRequestBehavior.AllowGet);
                }

                // 2. Fetch realtime_alarms for active run (limit to top 30 to de-duplicate, then take top 5)
                DataTable dtAlarms = connector.ExecuteQuery(
                    $"SELECT id, DateTime, Severity, TagName, Value, Threshold, Message FROM realtime_alarms " +
                    $"WHERE runId = {runId} AND Severity IN ('ALARM', 'WARNING') " +
                    $"ORDER BY DateTime DESC, id DESC LIMIT 30"
                );

                var globalAlarms = new List<object>();
                if (dtAlarms != null)
                {
                    foreach (DataRow row in dtAlarms.Rows)
                    {
                        DateTime alarmTime = Convert.ToDateTime(row["DateTime"]);
                        string severity = row["Severity"].ToString();
                        string tagName = row["TagName"].ToString();
                        double val = Convert.ToDouble(row["Value"]);
                        double threshold = Convert.ToDouble(row["Threshold"]);
                        string msg = row["Message"].ToString();

                        string unit = tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0 ? "°C" :
                                     tagName.IndexOf("ApSuat", StringComparison.OrdinalIgnoreCase) >= 0 ? "bar" : "";

                        string detailMessage = $"Giá trị: {val.ToString("0.#", CultureInfo.InvariantCulture)} {unit} (ngưỡng: {threshold.ToString("0.#", CultureInfo.InvariantCulture)} {unit})";

                        globalAlarms.Add(new
                        {
                            id = Convert.ToInt32(row["id"]),
                            time = alarmTime.ToString("HH:mm:ss"),
                            type = severity,
                            title = msg,
                            message = detailMessage
                        });
                    }
                }

                // De-duplicate and take top 5 sorted descending by id
                var sortedGlobalAlarms = globalAlarms.Cast<dynamic>()
                    .GroupBy(a => new { a.time, a.title })
                    .Select(g => g.First())
                    .OrderByDescending(a => (int)a.id)
                    .Take(5)
                    .ToList();

                return Json(sortedGlobalAlarms, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRuns(int batch_id)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
                };
                var dt = connector.ExecuteQuery($"SELECT id, name, status, run_number FROM runs WHERE batch_id = {batch_id} ORDER BY run_number ASC");
                var list = new List<object>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new
                        {
                            id = Convert.ToInt32(row["id"]),
                            name = row["name"].ToString(),
                            status = row["status"].ToString(),
                            run_number = Convert.ToInt32(row["run_number"])
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
        public JsonResult SearchRuns(string query)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
                };
                string safeQuery = (query ?? "").Replace("'", "''");
                var dt = connector.ExecuteQuery($"SELECT r.id, r.name, r.batch_id, r.status, b.name as batch_name, b.start_time, b.end_time FROM runs r INNER JOIN batches b ON r.batch_id = b.id WHERE r.name LIKE '%{safeQuery}%' ORDER BY r.id DESC LIMIT 10");
                var list = new List<object>();
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string batchStart = row["start_time"] != DBNull.Value ? Convert.ToDateTime(row["start_time"]).ToString("yyyy/MM/dd") : "";
                        string batchEnd = row["end_time"] != DBNull.Value ? Convert.ToDateTime(row["end_time"]).ToString("yyyy/MM/dd") : "";
                        list.Add(new
                        {
                            value = Convert.ToInt32(row["id"]),
                            label = row["name"].ToString(),
                            batch_id = Convert.ToInt32(row["batch_id"]),
                            batch_name = row["batch_name"].ToString(),
                            status = row["status"].ToString(),
                            batch_start = batchStart,
                            batch_end = batchEnd
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

        private string FormatTempRange(List<double> temps, double? threshold = null)
        {
            if (temps == null || temps.Count == 0) return "-";

            double min = temps.Min();
            double max = temps.Max();

            string minStr = Math.Round(min, 1).ToString("0.#", CultureInfo.InvariantCulture);
            string maxStr = Math.Round(max, 1).ToString("0.#", CultureInfo.InvariantCulture);

            bool isMinExceeded = threshold.HasValue && min >= threshold.Value;
            bool isMaxExceeded = threshold.HasValue && max >= threshold.Value;

            string formattedMin = isMinExceeded 
                ? $"<span style='color: #ef4444; font-weight: bold;'>{minStr}</span>" 
                : minStr;

            string formattedMax = isMaxExceeded 
                ? $"<span style='color: #ef4444; font-weight: bold;'>{maxStr}</span>" 
                : maxStr;

            if (minStr == maxStr)
            {
                bool eitherExceeded = isMinExceeded || isMaxExceeded;
                string formattedVal = eitherExceeded 
                    ? $"<span style='color: #ef4444; font-weight: bold;'>{minStr}</span>" 
                    : minStr;
                return $"{formattedVal}°C";
            }
            else
            {
                return $"{formattedMin}-{formattedMax}°C";
            }
        }

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
    }
}