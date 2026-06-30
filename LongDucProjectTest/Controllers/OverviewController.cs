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
using MySql.Data.MySqlClient;

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

                var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, null, runId?.ToString());
                int resolvedBatchId = resolution.BatchId;
                int resolvedRunId = resolution.RunId;

                int isPaused = 0;
                int spCapLieu = 0;
                int spTron1 = 0;
                int spXaDay = 0;
                int spRungXaDay = 0;
                int spHutXaDay = 0;
                int spTron2 = 0;
                int spXaHang = 0;
                int spRungXaHang = 0;

                string runName = "";
                string runStatus = "";
                string runStart = "";
                string runEnd = "";
                string batchName = "";
                string batchStatus = "";
                string batchStart = "";
                string batchEnd = "";

                if (resolvedRunId > 0)
                {
                    var dtRun = connector.ExecuteQuery($"SELECT id, batch_id, name, status, is_paused, start_time, end_time, sp_thoi_gian_cap_lieu, sp_thoi_gian_tron1, sp_thoi_gian_xa_day, sp_thoi_gian_rung_xa_day, sp_thoi_gian_hut_xa_day_them, sp_thoi_gian_tron2, sp_thoi_gian_xa_hang, sp_thoi_gian_rung_xa_hang FROM runs WHERE id = {resolvedRunId} LIMIT 1");
                    if (dtRun != null && dtRun.Rows.Count > 0)
                    {
                        runName = dtRun.Rows[0]["name"] != DBNull.Value ? dtRun.Rows[0]["name"].ToString() : "";
                        runStatus = dtRun.Rows[0]["status"] != DBNull.Value ? dtRun.Rows[0]["status"].ToString() : "";
                        isPaused = dtRun.Rows[0]["is_paused"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["is_paused"]) : 0;
                        runStart = dtRun.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtRun.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        runEnd = dtRun.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtRun.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";

                        spCapLieu = dtRun.Rows[0]["sp_thoi_gian_cap_lieu"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_cap_lieu"]) : 0;
                        spTron1 = dtRun.Rows[0]["sp_thoi_gian_tron1"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_tron1"]) : 0;
                        spXaDay = dtRun.Rows[0]["sp_thoi_gian_xa_day"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_xa_day"]) : 0;
                        spRungXaDay = dtRun.Rows[0]["sp_thoi_gian_rung_xa_day"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_rung_xa_day"]) : 0;
                        spHutXaDay = dtRun.Rows[0]["sp_thoi_gian_hut_xa_day_them"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_hut_xa_day_them"]) : 0;
                        spTron2 = dtRun.Rows[0]["sp_thoi_gian_tron2"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_tron2"]) : 0;
                        spXaHang = dtRun.Rows[0]["sp_thoi_gian_xa_hang"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_xa_hang"]) : 0;
                        spRungXaHang = dtRun.Rows[0]["sp_thoi_gian_rung_xa_hang"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_rung_xa_hang"]) : 0;
                    }
                }

                if (resolvedBatchId > 0)
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

                string productName = "-";
                string formula = "-";
                double targetWeight = 0;
                int totalRuns = 0;
                int completedRuns = 0;
                string batchActualStart = "-";
                double totalTargetWeight = 0;
                double totalProducedWeight = 0;
                double percent = 0;

                if (resolvedBatchId != -1)
                {
                    var dtBatchDetail = connector.ExecuteQuery($"SELECT product_name, formula, target_weight, total_runs, start_time FROM batches WHERE id = {resolvedBatchId} LIMIT 1");
                    if (dtBatchDetail != null && dtBatchDetail.Rows.Count > 0)
                    {
                        var row = dtBatchDetail.Rows[0];
                        productName = row["product_name"] != DBNull.Value ? row["product_name"].ToString() : "-";
                        formula = row["formula"] != DBNull.Value ? row["formula"].ToString() : "-";
                        targetWeight = row["target_weight"] != DBNull.Value ? Convert.ToDouble(row["target_weight"]) : 0;
                        totalRuns = row["total_runs"] != DBNull.Value ? Convert.ToInt32(row["total_runs"]) : 0;
                        batchActualStart = row["start_time"] != DBNull.Value ? Convert.ToDateTime(row["start_time"]).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) : "-";
                    }
                }

                // Weight calculation block moved below activeStepCode resolution

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
                    dtTelemetry = connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi, ThoiGianCapLieu, ThoiGianTron1, ThoiGianXaDay, ThoiGianRungXaDay, ThoiGianHutXaDay, ThoiGianTron2, ThoiGianXaHang, ThoiGianRungXaHang FROM alarmreport WHERE runId = {resolvedRunId} ORDER BY DateTime ASC");
                }
                else if (resolvedBatchId != -1)
                {
                    dtTelemetry = connector.ExecuteQuery($"SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi, ThoiGianCapLieu, ThoiGianTron1, ThoiGianXaDay, ThoiGianRungXaDay, ThoiGianHutXaDay, ThoiGianTron2, ThoiGianXaHang, ThoiGianRungXaHang FROM alarmreport WHERE batchId = {resolvedBatchId} ORDER BY DateTime ASC");
                }

                var accumulatedValues = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    { "ThoiGianCapLieu", 0 },
                    { "ThoiGianTron1", 0 },
                    { "ThoiGianXaDay", 0 },
                    { "ThoiGianRungXaDay", 0 },
                    { "ThoiGianHutXaDay", 0 },
                    { "ThoiGianTron2", 0 },
                    { "ThoiGianXaHang", 0 },
                    { "ThoiGianRungXaHang", 0 }
                };

                if (resolvedRunId != -1)
                {
                    var dtAcc = connector.ExecuteQuery($"SELECT stepCode, accumulatedTime FROM run_step_accumulated_times WHERE runId = {resolvedRunId}");
                    if (dtAcc != null && dtAcc.Rows.Count > 0)
                    {
                        var mapping = new Dictionary<int, string>
                        {
                            { 1, "ThoiGianCapLieu" },
                            { 2, "ThoiGianTron1" },
                            { 3, "ThoiGianXaDay" },
                            { 4, "ThoiGianRungXaDay" },
                            { 5, "ThoiGianHutXaDay" },
                            { 6, "ThoiGianTron2" },
                            { 7, "ThoiGianXaHang" },
                            { 8, "ThoiGianRungXaHang" }
                        };

                        foreach (DataRow row in dtAcc.Rows)
                        {
                            if (row["stepCode"] != DBNull.Value && row["accumulatedTime"] != DBNull.Value)
                            {
                                int code = Convert.ToInt32(row["stepCode"]);
                                double accTime = Convert.ToDouble(row["accumulatedTime"]);
                                if (mapping.ContainsKey(code))
                                {
                                    accumulatedValues[mapping[code]] = accTime;
                                }
                            }
                        }
                    }

                    // Always apply telemetry-max calculations to refine/correct accumulated times (recover lost seconds from polling delay)
                    if (dtTelemetry != null && dtTelemetry.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtTelemetry.Rows)
                        {
                            var keys = new List<string>(accumulatedValues.Keys);
                            foreach (var key in keys)
                            {
                                if (row.Table.Columns.Contains(key) && row[key] != DBNull.Value)
                                {
                                    if (double.TryParse(row[key].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                                    {
                                        if (val > accumulatedValues[key])
                                        {
                                            accumulatedValues[key] = val;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // 4. Fetch realtime_alarms for active run
                DataTable dtAlarms = null;
                if (resolvedRunId != -1)
                {
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE runId = {resolvedRunId} AND Severity IN ('ALARM', 'WARNING', 'HIGH', 'AVERAGE', 'LOW') AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0)) ORDER BY DateTime ASC, id ASC");
                }
                else if (resolvedBatchId != -1)
                {
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE batchId = {resolvedBatchId} AND Severity IN ('ALARM', 'WARNING', 'HIGH', 'AVERAGE', 'LOW') AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0)) ORDER BY DateTime ASC, id ASC");
                }

                // 5. Set up standard steps with their alarmlog TagNo mapping & keywords
                var stepDefs = new[]
                {
                    new { Code = 1, TagNo = "T001", Name = "Cấp liệu", Standard = spCapLieu > 0 ? $"{spCapLieu}s" : "0s", Alias = "ThoiGianCapLieu" },
                    new { Code = 2, TagNo = "T002", Name = "Trộn 1", Standard = spTron1 > 0 ? $"{spTron1}s" : "0s", Alias = "ThoiGianTron1" },
                    new { Code = 3, TagNo = "T003", Name = "Xả đáy", Standard = spXaDay > 0 ? $"{spXaDay}s" : "0s", Alias = "ThoiGianXaDay" },
                    new { Code = 4, TagNo = "T004", Name = "Rung xả đáy", Standard = spRungXaDay > 0 ? $"{spRungXaDay}s" : "0s", Alias = "ThoiGianRungXaDay" },
                    new { Code = 5, TagNo = "T005", Name = "Hút xả đáy", Standard = spHutXaDay > 0 ? $"{spHutXaDay}s" : "0s", Alias = "ThoiGianHutXaDay" },
                    new { Code = 6, TagNo = "T006", Name = "Trộn 2", Standard = spTron2 > 0 ? $"{spTron2}s" : "0s", Alias = "ThoiGianTron2" },
                    new { Code = 7, TagNo = "T007", Name = "Xả hàng", Standard = spXaHang > 0 ? $"{spXaHang}s" : "0s", Alias = "ThoiGianXaHang" },
                    new { Code = 8, TagNo = "T008", Name = "Rung xả hàng", Standard = spRungXaHang > 0 ? $"{spRungXaHang}s" : "0s", Alias = "ThoiGianRungXaHang" }
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
                var activeStepCodes = new List<int>();
                var activeStepNames = new List<string>();
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
                            activeStepCodes.Add(def.Code);
                            activeStepNames.Add(def.Name);
                            var t = Convert.ToDateTime(match["OccurrenceTime"]);
                            if (!activeStepStartTime.HasValue || t < activeStepStartTime.Value)
                            {
                                activeStepStartTime = t;
                            }
                        }
                    }

                    if (activeStepCodes.Count > 0)
                    {
                        activeStepCode = activeStepCodes[0];
                        activeStepName = string.Join(", ", activeStepNames);
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

                // Synchronize activeStepCodes list if fallback logic resolved activeStepCode but list is empty
                if (activeStepCode > 0 && activeStepCodes.Count == 0)
                {
                    activeStepCodes.Add(activeStepCode);
                }

                // Thresholds building removed as Time-Lag Compensation handles leakage without alarm thresholds

                // Calculate weight metrics based on activeStepCode
                if (resolvedBatchId != -1)
                {
                    // Count valid (non-error) runs and completed runs
                    int validRunsCount = 0;
                    int completedRunsCount = 0;
                    var runsListForWeight = new List<Tuple<int, string>>();

                    var dtRunsAll = connector.ExecuteQuery($"SELECT id, status FROM runs WHERE batch_id = {resolvedBatchId}");
                    if (dtRunsAll != null)
                    {
                        foreach (DataRow row in dtRunsAll.Rows)
                        {
                            int rId = Convert.ToInt32(row["id"]);
                            string statusVal = row["status"] != DBNull.Value ? row["status"].ToString().Trim() : "";
                            runsListForWeight.Add(Tuple.Create(rId, statusVal));

                            if (!statusVal.Equals("Error", StringComparison.OrdinalIgnoreCase) && 
                                !statusVal.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                            {
                                validRunsCount++;
                            }
                            if (statusVal.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                            {
                                completedRunsCount++;
                            }
                        }
                    }

                    // Query the sum of quantities in run_info for all runs in the batch
                    var runWeightsDict = new Dictionary<int, double>();
                    var dtRunWeights = connector.ExecuteQuery($@"
                        SELECT ri.run_id, SUM(ri.quantity) as run_weight 
                        FROM run_info ri 
                        JOIN runs r ON ri.run_id = r.id 
                        WHERE r.batch_id = {resolvedBatchId}
                          AND LOWER(ri.unit) = 'kg'
                        GROUP BY ri.run_id");
                    if (dtRunWeights != null)
                    {
                        foreach (DataRow row in dtRunWeights.Rows)
                        {
                            int rId = Convert.ToInt32(row["run_id"]);
                            double w = row["run_weight"] != DBNull.Value ? Convert.ToDouble(row["run_weight"]) : 0;
                            runWeightsDict[rId] = w;
                        }
                    }

                    double averageRunWeight = targetWeight / (totalRuns > 0 ? totalRuns : 1);

                    double totalActualRunsWeight = 0;
                    double totalCompletedRunsWeight = 0;

                    foreach (var run in runsListForWeight)
                    {
                        // Exclude error runs completely
                        if (run.Item2.Equals("Error", StringComparison.OrdinalIgnoreCase) || 
                            run.Item2.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        double runWeight = averageRunWeight;
                        if (runWeightsDict.ContainsKey(run.Item1) && runWeightsDict[run.Item1] > 0)
                        {
                            runWeight = runWeightsDict[run.Item1];
                        }

                        totalActualRunsWeight += runWeight;
                        if (run.Item2.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                        {
                            totalCompletedRunsWeight += runWeight;
                        }
                    }

                    if (validRunsCount > 0)
                    {
                        double allowableLoss = totalActualRunsWeight - targetWeight;
                        totalProducedWeight = totalCompletedRunsWeight - completedRunsCount * (allowableLoss / validRunsCount);
                        if (totalProducedWeight < 0) totalProducedWeight = 0;
                    }
                    else
                    {
                        totalProducedWeight = 0;
                    }

                    totalTargetWeight = targetWeight;
                    totalRuns = validRunsCount;
                    completedRuns = completedRunsCount;
                }

                if (totalTargetWeight <= 0)
                {
                    totalTargetWeight = targetWeight;
                }

                string targetWeightStr = totalTargetWeight > 0 ? $"{totalTargetWeight.ToString("0.##", CultureInfo.InvariantCulture)} KG" : "-";
                percent = totalTargetWeight > 0 ? ((double)totalProducedWeight / totalTargetWeight * 100) : 0;
                string actualWeightStr = totalTargetWeight > 0 ? $"{totalProducedWeight.ToString("0.##", CultureInfo.InvariantCulture)} KG ({Math.Round(percent)}%)" : "-";

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
                        if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && activeStepCodes.Contains(def.Code))
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
                        string status = "pending";
                        string statusText = "Chưa bắt đầu";

                        string statusVal = stepLogRow["Status"].ToString().Trim();
                        bool isCompleted = statusVal.Equals("Resolved", StringComparison.OrdinalIgnoreCase);
                        bool isAlarm = statusVal.Equals("Alarm", StringComparison.OrdinalIgnoreCase);

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
                                if (accumulatedValues.ContainsKey(def.Alias) && accumulatedValues[def.Alias] > 0)
                                {
                                    totalSeconds = accumulatedValues[def.Alias];
                                }
                                durationStr = $"{(int)Math.Round(totalSeconds)}s";
                            }
                        }
                        else if (isAlarm)
                        {
                            status = "in-progress";
                            statusText = "Đang thực hiện";
                        }
                        else
                        {
                            if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && activeStepCodes.Contains(def.Code))
                            {
                                status = "in-progress";
                                statusText = "Đang thực hiện";
                            }
                            else
                            {
                                status = "pending";
                                statusText = "Chưa bắt đầu";
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
                            bool cdMatchesAny = false;
                            if (!string.IsNullOrEmpty(cd))
                            {
                                foreach (var otherDef in stepDefs)
                                {
                                    bool matchThis = false;
                                    if (cd.Equals(otherDef.TagNo, StringComparison.OrdinalIgnoreCase)) matchThis = true;
                                    else if (cd.Equals(otherDef.Name, StringComparison.OrdinalIgnoreCase)) matchThis = true;
                                    else
                                    {
                                        string cdLower = RemoveSign4VietnameseString(cd).ToLower();
                                        string otherDefNameLower = RemoveSign4VietnameseString(otherDef.Name).ToLower();
                                        if (cdLower == otherDefNameLower) matchThis = true;
                                        else if (otherDef.Code == 1 && (cdLower.Contains("cap lieu") || cdLower.Contains("cấp liệu") || cdLower.Contains("t001"))) matchThis = true;
                                        else if (otherDef.Code == 2 && (cdLower.Contains("tron 1") || cdLower.Contains("trộn 1") || cdLower.Contains("t002"))) matchThis = true;
                                        else if (otherDef.Code == 3 && (cdLower.Contains("xa day") || cdLower.Contains("xả đáy") || cdLower.Contains("t003"))) matchThis = true;
                                        else if (otherDef.Code == 4 && (cdLower.Contains("rung xa day") || cdLower.Contains("rung xả đáy") || cdLower.Contains("t004"))) matchThis = true;
                                        else if (otherDef.Code == 5 && (cdLower.Contains("hut xa day") || cdLower.Contains("hút xả đáy") || cdLower.Contains("t005"))) matchThis = true;
                                        else if (otherDef.Code == 6 && (cdLower.Contains("tron 2") || cdLower.Contains("trộn 2") || cdLower.Contains("t006"))) matchThis = true;
                                        else if (otherDef.Code == 7 && (cdLower.Contains("xa hang") || cdLower.Contains("xả hàng") || cdLower.Contains("t007")) && !cdLower.Contains("rung")) matchThis = true;
                                        else if (otherDef.Code == 8 && (cdLower.Contains("rung xa hang") || cdLower.Contains("rung xả hàng") || cdLower.Contains("t008"))) matchThis = true;
                                    }

                                    if (matchThis)
                                    {
                                        cdMatchesAny = true;
                                        if (otherDef.Code == def.Code)
                                        {
                                            codeMatches = true;
                                        }
                                    }
                                }
                            }

                            return cdMatchesAny ? codeMatches : timeInStep;
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

                        // Auto-scale telemetry values if they are stored as raw integers (e.g. 320 instead of 32.0)
                        if (topTemps.Any(t => t > 150.0))
                        {
                            for (int i = 0; i < topTemps.Count; i++) topTemps[i] /= 10.0;
                        }
                        if (midTemps.Any(t => t > 150.0))
                        {
                            for (int i = 0; i < midTemps.Count; i++) midTemps[i] /= 10.0;
                        }
                        if (botTemps.Any(t => t > 150.0))
                        {
                            for (int i = 0; i < botTemps.Count; i++) botTemps[i] /= 10.0;
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
                                    topTemps.Add(NormalizeTemperature(val));
                                }
                                else if (tagName.IndexOf("NhietDoBonTronGiua", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    midTemps.Add(NormalizeTemperature(val));
                                }
                                else if (tagName.IndexOf("NhietDoBonTronDuoi", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    botTemps.Add(NormalizeTemperature(val));
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
                                    topThreshold = NormalizeTemperature(thresh);
                                }
                                else if (tagName.IndexOf("NhietDoBonTronGiua", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    midThreshold = NormalizeTemperature(thresh);
                                }
                                else if (tagName.IndexOf("NhietDoBonTronDuoi", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    botThreshold = NormalizeTemperature(thresh);
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

                            if (tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                val = NormalizeTemperature(val);
                                threshold = NormalizeTemperature(threshold);
                            }

                            string formatStr = tagName.IndexOf("ApSuat", StringComparison.OrdinalIgnoreCase) >= 0 ? "0.00" : "0.#";
                            string detailMessage = $"Giá trị: {val.ToString(formatStr, CultureInfo.InvariantCulture)} {unit} (ngưỡng: {threshold.ToString(formatStr, CultureInfo.InvariantCulture)} {unit})";

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
                        if (def.Code == 3) continue; // Exclude bottom discharge (Xả đáy)
                        
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
                                double stepSecs = (Convert.ToDateTime(stepLogRow["RestoreTime"]) - startTime).TotalSeconds;
                                if (accumulatedValues.ContainsKey(def.Alias) && accumulatedValues[def.Alias] > 0)
                                {
                                    stepSecs = accumulatedValues[def.Alias];
                                }
                                runningSeconds += stepSecs;
                            }
                            else if (!isCompleted)
                            {
                                double stepSecs = (DateTime.Now - startTime).TotalSeconds;
                                if (accumulatedValues.ContainsKey(def.Alias) && accumulatedValues[def.Alias] > 0)
                                {
                                    stepSecs = accumulatedValues[def.Alias];
                                }
                                runningSeconds += stepSecs;
                            }
                        }
                    }
                }
                if (runningSeconds < 0) runningSeconds = 0;

                // Calculate alarm count excluding INFO severity
                int alarmCount = 0;
                if (resolvedRunId != -1)
                {
                    var dtAlarmCount = connector.ExecuteQuery($"SELECT COUNT(*) FROM realtime_alarms WHERE runId = {resolvedRunId} AND Severity != 'INFO' AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0))");
                    if (dtAlarmCount != null && dtAlarmCount.Rows.Count > 0)
                    {
                        alarmCount = Convert.ToInt32(dtAlarmCount.Rows[0][0]);
                    }
                }
                else if (resolvedBatchId != -1)
                {
                    var dtAlarmCount = connector.ExecuteQuery($"SELECT COUNT(*) FROM realtime_alarms WHERE batchId = {resolvedBatchId} AND Severity != 'INFO' AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0))");
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
                    var dtRuns = connector.ExecuteQuery($"SELECT id, run_number, name, status, is_paused, start_time, end_time FROM runs WHERE batch_id = {resolvedBatchId} ORDER BY run_number ASC");
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
                                is_paused = row["is_paused"] != DBNull.Value ? Convert.ToInt32(row["is_paused"]) : 0,
                                start_time = row["start_time"] != DBNull.Value ? Convert.ToDateTime(row["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "",
                                end_time = row["end_time"] != DBNull.Value ? Convert.ToDateTime(row["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : ""
                            });
                        }
                    }
                }



                var batchInfo = new
                {
                    accumulatedValues = accumulatedValues,
                    batchId = resolvedBatchId,
                    batchName = batchName,
                    batchStatus = batchStatus,
                    machineStatus = runStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) ? "RUNNING" : 
                                    (runStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase) ? "PENDING" : "COMPLETED"),
                    activeStepCode = activeStepCode,
                    activeStepName = activeStepName,
                    headerStepName = headerStepName,
                    activeStepStartTime = activeStepStartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    batchStartTime = !string.IsNullOrEmpty(runStart) ? runStart : batchStart, // fallback to batch start
                    batchTotalSeconds = runningSeconds, // total elapsed seconds of the resolved run
                    headerRunningTime = headerRunningTimeStr,
                    alarmCount = alarmCount,
                    serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    
                    // Batch overview parameters
                    productName = productName,
                    formula = formula,
                    targetWeightStr = targetWeightStr,
                    actualWeightStr = actualWeightStr,
                    batchActualStart = batchActualStart,
                    totalRuns = totalRuns,
                    completedRuns = completedRuns,
                    batchEndTime = batchEnd,
                    
                    // Raw weight properties
                    totalProducedWeight = totalProducedWeight,
                    totalTargetWeight = totalTargetWeight,

                    // Run specific metadata
                    runId = resolvedRunId,
                    runName = runName,
                    runStatus = runStatus,
                    isPaused = isPaused,
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
                var todayStr = DateTime.Today.ToString("yyyy-MM-dd");
                var dtDaily = connector.ExecuteQuery($@"
                    SELECT id, name, status, start_time, end_time, target_weight 
                    FROM batches 
                    WHERE DATE(created_at) = '{todayStr}' 
                       OR DATE(start_time) = '{todayStr}' 
                       OR status = 'Active' 
                       OR id IN (SELECT DISTINCT batch_id FROM runs WHERE DATE(start_time) = '{todayStr}' OR DATE(end_time) = '{todayStr}') 
                    ORDER BY id ASC");

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
                                durationStr = $"{startTimeVal.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}";
                            }
                        }
                        else
                        {
                            if (firstOccurrence.HasValue && lastRestore.HasValue)
                            {
                                durationStr = $"{firstOccurrence.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} - {lastRestore.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}";
                            }
                            else if (row["start_time"] != DBNull.Value && row["end_time"] != DBNull.Value)
                            {
                                DateTime startTimeVal = Convert.ToDateTime(row["start_time"]);
                                DateTime endTimeVal = Convert.ToDateTime(row["end_time"]);
                                durationStr = $"{startTimeVal.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)} - {endTimeVal.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}";
                            }
                        }
                        
                        double bWeight = row["target_weight"] != DBNull.Value ? Convert.ToDouble(row["target_weight"]) : 0;
                        string bWeightStr = bWeight > 0 ? $"{bWeight.ToString("0.##", CultureInfo.InvariantCulture)} KG" : "-";

                        double bProducedWeight = 0;
                        var dtRunsForBatch = connector.ExecuteQuery($"SELECT id, status FROM runs WHERE batch_id = {bId}");
                        if (dtRunsForBatch != null && dtRunsForBatch.Rows.Count > 0)
                        {
                            var runBOMWeights = new Dictionary<int, double>();
                            var dtBOMWeights = connector.ExecuteQuery($@"
                                SELECT ri.run_id, SUM(ri.quantity) as run_weight 
                                FROM run_info ri 
                                JOIN runs r ON ri.run_id = r.id 
                                WHERE r.batch_id = {bId}
                                  AND LOWER(ri.unit) = 'kg'
                                GROUP BY ri.run_id");
                            if (dtBOMWeights != null)
                            {
                                foreach (DataRow rRow in dtBOMWeights.Rows)
                                {
                                    int rId = Convert.ToInt32(rRow["run_id"]);
                                    double w = rRow["run_weight"] != DBNull.Value ? Convert.ToDouble(rRow["run_weight"]) : 0;
                                    runBOMWeights[rId] = w;
                                }
                            }

                            int totalRunsForBatch = dtRunsForBatch.Rows.Count;
                            double averageRunWeightForBatch = bWeight / (totalRunsForBatch > 0 ? totalRunsForBatch : 1);

                            int validRunsCount = 0;
                            int completedRunsCount = 0;
                            double totalRunsWeight = 0;
                            double completedRunsWeight = 0;

                            foreach (DataRow rRow in dtRunsForBatch.Rows)
                            {
                                int rId = Convert.ToInt32(rRow["id"]);
                                string rStatus = rRow["status"] != DBNull.Value ? rRow["status"].ToString().Trim() : "";

                                if (rStatus.Equals("Error", StringComparison.OrdinalIgnoreCase) || 
                                    rStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                validRunsCount++;

                                double runWeight = averageRunWeightForBatch;
                                if (runBOMWeights.ContainsKey(rId) && runBOMWeights[rId] > 0)
                                {
                                    runWeight = runBOMWeights[rId];
                                }

                                totalRunsWeight += runWeight;

                                if (rStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                                {
                                    completedRunsCount++;
                                    completedRunsWeight += runWeight;
                                }
                            }

                            if (validRunsCount > 0)
                            {
                                double allowableLoss = totalRunsWeight - bWeight;
                                bProducedWeight = completedRunsWeight - completedRunsCount * (allowableLoss / validRunsCount);
                                if (bProducedWeight < 0) bProducedWeight = 0;
                            }
                        }
                        else
                        {
                            if (bStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                            {
                                bProducedWeight = bWeight;
                            }
                        }

                        dailyBatchesList.Add(new
                        {
                            id = bId,
                            name = bName,
                            weight = bWeightStr,
                            producedWeight = bProducedWeight,
                            duration = durationStr,
                            status = bStatus
                        });
                    }
                }

                // Check for pending runs of active batches started on a previous day
                string pendingRunNote = "";
                var dtPendingRunsCheck = connector.ExecuteQuery($@"
                    SELECT b.name AS batch_name, b.start_time AS batch_start, r.name AS run_name 
                    FROM runs r 
                    JOIN batches b ON r.batch_id = b.id 
                    WHERE b.status = 'Active' 
                      AND DATE(b.start_time) < '{todayStr}' 
                      AND r.status IN ('Pending', 'Waiting', 'Created') 
                    ORDER BY b.id ASC, r.run_number ASC");

                if (dtPendingRunsCheck != null && dtPendingRunsCheck.Rows.Count > 0)
                {
                    var pendingGroups = dtPendingRunsCheck.AsEnumerable()
                        .GroupBy(row => new { 
                            BatchName = row["batch_name"].ToString(), 
                            Start = Convert.ToDateTime(row["batch_start"]).ToString("yyyy-MM-dd") 
                        });
                    
                    var groupNotes = new List<string>();
                    foreach (var group in pendingGroups)
                    {
                        var runNames = string.Join(", ", group.Select(r => r["run_name"].ToString()));
                        groupNotes.Add($"Batch đang chạy ({group.Key.BatchName}) ngày {group.Key.Start}, mẻ còn thiếu chưa chạy ({runNames})");
                    }
                    pendingRunNote = string.Join(" | ", groupNotes);
                }

                // Fetch BOM (run_info) for all runs in the active/resolved batch
                var bomList = new List<object>();
                if (resolvedBatchId != -1)
                {
                    var dtBOM = connector.ExecuteQuery($"SELECT ri.code, ri.material_code, ri.quantity, ri.value, ri.unit, ri.batch_no, r.run_number, ri.run_id FROM run_info ri JOIN runs r ON ri.run_id = r.id WHERE r.batch_id = {resolvedBatchId} AND r.status != 'Error' ORDER BY r.run_number ASC, ri.id ASC");
                    if (dtBOM != null)
                    {
                        foreach (DataRow row in dtBOM.Rows)
                        {
                            bomList.Add(new
                            {
                                code = row["code"] != DBNull.Value ? row["code"].ToString() : "",
                                material_code = row["material_code"] != DBNull.Value ? row["material_code"].ToString() : "",
                                quantity = row["quantity"] != DBNull.Value ? Convert.ToDouble(row["quantity"]) : (double?)null,
                                value = row["value"] != DBNull.Value ? row["value"].ToString() : "",
                                unit = row["unit"] != DBNull.Value ? row["unit"].ToString() : "",
                                batch_no = row["batch_no"] != DBNull.Value ? row["batch_no"].ToString() : "",
                                run_number = row["run_number"] != DBNull.Value ? Convert.ToInt32(row["run_number"]) : 1,
                                run_id = row["run_id"] != DBNull.Value ? Convert.ToInt32(row["run_id"]) : 0
                            });
                        }
                    }
                }

                return Json(new { steps = stepsList, globalAlarms = sortedGlobalAlarms, batchInfo = batchInfo, dailyBatches = dailyBatchesList, bom = bomList, pendingRunNote = pendingRunNote }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetLightweightStepStatus(int? runId = null)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
                };

                var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, null, runId?.ToString());
                int resolvedBatchId = resolution.BatchId;
                int resolvedRunId = resolution.RunId;

                int isPaused = 0;
                int spCapLieu = 0;
                int spTron1 = 0;
                int spXaDay = 0;
                int spRungXaDay = 0;
                int spHutXaDay = 0;
                int spTron2 = 0;
                int spXaHang = 0;
                int spRungXaHang = 0;

                string runStatus = "";
                string runStart = "";
                string runEnd = "";
                string batchStatus = "";
                string batchStart = "";

                if (resolvedRunId > 0)
                {
                    var dtRun = connector.ExecuteQuery($"SELECT status, is_paused, start_time, end_time, sp_thoi_gian_cap_lieu, sp_thoi_gian_tron1, sp_thoi_gian_xa_day, sp_thoi_gian_rung_xa_day, sp_thoi_gian_hut_xa_day_them, sp_thoi_gian_tron2, sp_thoi_gian_xa_hang, sp_thoi_gian_rung_xa_hang FROM runs WHERE id = {resolvedRunId} LIMIT 1");
                    if (dtRun != null && dtRun.Rows.Count > 0)
                    {
                        runStatus = dtRun.Rows[0]["status"] != DBNull.Value ? dtRun.Rows[0]["status"].ToString() : "";
                        isPaused = dtRun.Rows[0]["is_paused"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["is_paused"]) : 0;
                        runStart = dtRun.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtRun.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                        runEnd = dtRun.Rows[0]["end_time"] != DBNull.Value ? Convert.ToDateTime(dtRun.Rows[0]["end_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";

                        spCapLieu = dtRun.Rows[0]["sp_thoi_gian_cap_lieu"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_cap_lieu"]) : 0;
                        spTron1 = dtRun.Rows[0]["sp_thoi_gian_tron1"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_tron1"]) : 0;
                        spXaDay = dtRun.Rows[0]["sp_thoi_gian_xa_day"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_xa_day"]) : 0;
                        spRungXaDay = dtRun.Rows[0]["sp_thoi_gian_rung_xa_day"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_rung_xa_day"]) : 0;
                        spHutXaDay = dtRun.Rows[0]["sp_thoi_gian_hut_xa_day_them"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_hut_xa_day_them"]) : 0;
                        spTron2 = dtRun.Rows[0]["sp_thoi_gian_tron2"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_tron2"]) : 0;
                        spXaHang = dtRun.Rows[0]["sp_thoi_gian_xa_hang"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_xa_hang"]) : 0;
                        spRungXaHang = dtRun.Rows[0]["sp_thoi_gian_rung_xa_hang"] != DBNull.Value ? Convert.ToInt32(dtRun.Rows[0]["sp_thoi_gian_rung_xa_hang"]) : 0;
                    }
                }

                if (resolvedBatchId > 0)
                {
                    var dtBatch = connector.ExecuteQuery($"SELECT status, start_time FROM batches WHERE id = {resolvedBatchId} LIMIT 1");
                    if (dtBatch != null && dtBatch.Rows.Count > 0)
                    {
                        batchStatus = dtBatch.Rows[0]["status"] != DBNull.Value ? dtBatch.Rows[0]["status"].ToString() : "";
                        batchStart = dtBatch.Rows[0]["start_time"] != DBNull.Value ? Convert.ToDateTime(dtBatch.Rows[0]["start_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    }
                }

                DataTable dtAlarmLog = null;
                if (resolvedRunId != -1)
                {
                    dtAlarmLog = connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE runId = {resolvedRunId}");
                }
                else if (resolvedBatchId != -1)
                {
                    dtAlarmLog = connector.ExecuteQuery($"SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE batchId = {resolvedBatchId}");
                }

                DataTable dtTelemetry = null;
                if (resolvedRunId != -1)
                {
                    dtTelemetry = connector.ExecuteQuery($"SELECT ThoiGianCapLieu, ThoiGianTron1, ThoiGianXaDay, ThoiGianRungXaDay, ThoiGianHutXaDay, ThoiGianTron2, ThoiGianXaHang, ThoiGianRungXaHang FROM alarmreport WHERE runId = {resolvedRunId}");
                }
                else if (resolvedBatchId != -1)
                {
                    dtTelemetry = connector.ExecuteQuery($"SELECT ThoiGianCapLieu, ThoiGianTron1, ThoiGianXaDay, ThoiGianRungXaDay, ThoiGianHutXaDay, ThoiGianTron2, ThoiGianXaHang, ThoiGianRungXaHang FROM alarmreport WHERE batchId = {resolvedBatchId}");
                }

                var accumulatedValues = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    { "ThoiGianCapLieu", 0 },
                    { "ThoiGianTron1", 0 },
                    { "ThoiGianXaDay", 0 },
                    { "ThoiGianRungXaDay", 0 },
                    { "ThoiGianHutXaDay", 0 },
                    { "ThoiGianTron2", 0 },
                    { "ThoiGianXaHang", 0 },
                    { "ThoiGianRungXaHang", 0 }
                };

                if (resolvedRunId != -1)
                {
                    var dtAcc = connector.ExecuteQuery($"SELECT stepCode, accumulatedTime FROM run_step_accumulated_times WHERE runId = {resolvedRunId}");
                    if (dtAcc != null && dtAcc.Rows.Count > 0)
                    {
                        var mapping = new Dictionary<int, string>
                        {
                            { 1, "ThoiGianCapLieu" },
                            { 2, "ThoiGianTron1" },
                            { 3, "ThoiGianXaDay" },
                            { 4, "ThoiGianRungXaDay" },
                            { 5, "ThoiGianHutXaDay" },
                            { 6, "ThoiGianTron2" },
                            { 7, "ThoiGianXaHang" },
                            { 8, "ThoiGianRungXaHang" }
                        };

                        foreach (DataRow row in dtAcc.Rows)
                        {
                            if (row["stepCode"] != DBNull.Value && row["accumulatedTime"] != DBNull.Value)
                            {
                                int code = Convert.ToInt32(row["stepCode"]);
                                double accTime = Convert.ToDouble(row["accumulatedTime"]);
                                if (mapping.ContainsKey(code))
                                {
                                    accumulatedValues[mapping[code]] = accTime;
                                }
                            }
                        }
                    }

                    // Apply telemetry-max calculations to refine/correct accumulated times (recover lost seconds from polling delay)
                    if (dtTelemetry != null && dtTelemetry.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtTelemetry.Rows)
                        {
                            var keys = new List<string>(accumulatedValues.Keys);
                            foreach (var key in keys)
                            {
                                if (row.Table.Columns.Contains(key) && row[key] != DBNull.Value)
                                {
                                    if (double.TryParse(row[key].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                                    {
                                        if (val > accumulatedValues[key])
                                        {
                                            accumulatedValues[key] = val;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                DataTable dtAlarms = null;
                if (resolvedRunId != -1)
                {
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE runId = {resolvedRunId} AND Severity IN ('ALARM', 'WARNING', 'HIGH', 'AVERAGE', 'LOW') AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0)) ORDER BY DateTime ASC, id ASC");
                }
                else if (resolvedBatchId != -1)
                {
                    dtAlarms = connector.ExecuteQuery($"SELECT id, DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE batchId = {resolvedBatchId} AND Severity IN ('ALARM', 'WARNING', 'HIGH', 'AVERAGE', 'LOW') AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0)) ORDER BY DateTime ASC, id ASC");
                }

                var stepDefs = new[]
                {
                    new { Code = 1, TagNo = "T001", Name = "Cấp liệu", Standard = spCapLieu > 0 ? $"{spCapLieu}s" : "0s", Alias = "ThoiGianCapLieu" },
                    new { Code = 2, TagNo = "T002", Name = "Trộn 1", Standard = spTron1 > 0 ? $"{spTron1}s" : "0s", Alias = "ThoiGianTron1" },
                    new { Code = 3, TagNo = "T003", Name = "Xả đáy", Standard = spXaDay > 0 ? $"{spXaDay}s" : "0s", Alias = "ThoiGianXaDay" },
                    new { Code = 4, TagNo = "T004", Name = "Rung xả đáy", Standard = spRungXaDay > 0 ? $"{spRungXaDay}s" : "0s", Alias = "ThoiGianRungXaDay" },
                    new { Code = 5, TagNo = "T005", Name = "Hút xả đáy", Standard = spHutXaDay > 0 ? $"{spHutXaDay}s" : "0s", Alias = "ThoiGianHutXaDay" },
                    new { Code = 6, TagNo = "T006", Name = "Trộn 2", Standard = spTron2 > 0 ? $"{spTron2}s" : "0s", Alias = "ThoiGianTron2" },
                    new { Code = 7, TagNo = "T007", Name = "Xả hàng", Standard = spXaHang > 0 ? $"{spXaHang}s" : "0s", Alias = "ThoiGianXaHang" },
                    new { Code = 8, TagNo = "T008", Name = "Rung xả hàng", Standard = spRungXaHang > 0 ? $"{spRungXaHang}s" : "0s", Alias = "ThoiGianRungXaHang" }
                };

                var stepsList = new List<object>();

                var logRows = dtAlarmLog != null 
                    ? dtAlarmLog.AsEnumerable()
                                 .OrderByDescending(r => r["OccurrenceTime"] != DBNull.Value ? Convert.ToDateTime(r["OccurrenceTime"]) : DateTime.MinValue)
                                 .ToList() 
                    : new List<DataRow>();
                var alarmRows = dtAlarms != null ? dtAlarms.AsEnumerable().ToList() : new List<DataRow>();

                var activeLogRows = logRows.Where(r => r["Status"] != DBNull.Value && r["Status"].ToString().Trim().Equals("Alarm", StringComparison.OrdinalIgnoreCase)).ToList();
                int activeStepCode = 0;
                var activeStepCodes = new List<int>();
                var activeStepNames = new List<string>();
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
                            activeStepCodes.Add(def.Code);
                            activeStepNames.Add(def.Name);
                            var t = Convert.ToDateTime(match["OccurrenceTime"]);
                            if (!activeStepStartTime.HasValue || t < activeStepStartTime.Value)
                            {
                                activeStepStartTime = t;
                            }
                        }
                    }

                    if (activeStepCodes.Count > 0)
                    {
                        activeStepCode = activeStepCodes[0];
                        activeStepName = string.Join(", ", activeStepNames);
                    }
                }

                if (batchStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    activeStepCode = 8;
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
                            bool isT004Resolved = logRows.Any(r => r["TagNo"] != DBNull.Value && r["TagNo"].ToString().Trim().Equals("T004", StringComparison.OrdinalIgnoreCase) && r["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase));
                            bool isT003Resolved = logRows.Any(r => r["TagNo"] != DBNull.Value && r["TagNo"].ToString().Trim().Equals("T003", StringComparison.OrdinalIgnoreCase) && r["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase));
                            if (isT004Resolved) activeStepCode = 5;
                            else if (isT003Resolved) activeStepCode = 4;
                            else activeStepCode = 3;
                        }
                        else if (inferredCd == 4) activeStepCode = 6;
                        else if (inferredCd == 5)
                        {
                            bool isT007Resolved = logRows.Any(r => r["TagNo"] != DBNull.Value && r["TagNo"].ToString().Trim().Equals("T007", StringComparison.OrdinalIgnoreCase) && r["Status"].ToString().Trim().Equals("Resolved", StringComparison.OrdinalIgnoreCase));
                            if (isT007Resolved) activeStepCode = 8;
                            else activeStepCode = 7;
                        }
                    }

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

                if (activeStepCode > 0 && activeStepCodes.Count == 0)
                {
                    activeStepCodes.Add(activeStepCode);
                }

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

                    if (stepLogRow == null)
                    {
                        if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && activeStepCodes.Contains(def.Code))
                        {
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
                                statusText = "Chờ chạy",
                                alerts = new List<object>()
                            });
                        }
                    }
                    else
                    {
                        DateTime startTime = Convert.ToDateTime(stepLogRow["OccurrenceTime"]);
                        string startStr = startTime.ToString("HH:mm:ss");
                        string endStr = "-";
                        string durationStr = "-";
                        string status = "pending";
                        string statusText = "Chờ chạy";

                        string statusVal = stepLogRow["Status"].ToString().Trim();
                        bool isCompleted = statusVal.Equals("Resolved", StringComparison.OrdinalIgnoreCase);
                        bool isAlarm = statusVal.Equals("Alarm", StringComparison.OrdinalIgnoreCase);

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
                                if (accumulatedValues.ContainsKey(def.Alias) && accumulatedValues[def.Alias] > 0)
                                {
                                    totalSeconds = accumulatedValues[def.Alias];
                                }
                                durationStr = $"{(int)Math.Round(totalSeconds)}s";
                            }
                        }
                        else if (isAlarm)
                        {
                            status = "in-progress";
                            statusText = "Đang thực hiện";
                        }
                        else
                        {
                            if (batchStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) && activeStepCodes.Contains(def.Code))
                            {
                                status = "in-progress";
                                statusText = "Đang thực hiện";
                            }
                            else
                            {
                                status = "pending";
                                statusText = "Chờ chạy";
                            }
                        }

                        var stepAlertsList = new List<object>();
                        var stepAlarms = alarmRows.Where(r => {
                            DateTime alarmTime = Convert.ToDateTime(r["DateTime"]);
                            bool timeInStep = endTime.HasValue ? (alarmTime >= startTime && alarmTime <= endTime.Value) : (alarmTime >= startTime);

                            string cd = r["CongDoan"] != DBNull.Value ? r["CongDoan"].ToString().Trim() : "";
                            bool codeMatches = false;
                            bool cdMatchesAny = false;
                            if (!string.IsNullOrEmpty(cd))
                            {
                                foreach (var otherDef in stepDefs)
                                {
                                    bool matchThis = false;
                                    if (cd.Equals(otherDef.TagNo, StringComparison.OrdinalIgnoreCase)) matchThis = true;
                                    else if (cd.Equals(otherDef.Name, StringComparison.OrdinalIgnoreCase)) matchThis = true;
                                    else
                                    {
                                        string cdLower = RemoveSign4VietnameseString(cd).ToLower();
                                        string otherDefNameLower = RemoveSign4VietnameseString(otherDef.Name).ToLower();
                                        if (cdLower == otherDefNameLower) matchThis = true;
                                        else if (otherDef.Code == 1 && (cdLower.Contains("cap lieu") || cdLower.Contains("c\u1ea5p li\u1ec7u") || cdLower.Contains("t001"))) matchThis = true;
                                        else if (otherDef.Code == 2 && (cdLower.Contains("tron 1") || cdLower.Contains("tr\u1ed9n 1") || cdLower.Contains("t002"))) matchThis = true;
                                        else if (otherDef.Code == 3 && (cdLower.Contains("xa day") || cdLower.Contains("x\u1ea3 \u0111\u00e1y") || cdLower.Contains("t003"))) matchThis = true;
                                        else if (otherDef.Code == 4 && (cdLower.Contains("rung xa day") || cdLower.Contains("rung x\u1ea3 \u0111\u00e1y") || cdLower.Contains("t004"))) matchThis = true;
                                        else if (otherDef.Code == 5 && (cdLower.Contains("hut xa day") || cdLower.Contains("h\u00fat x\u1ea3 \u0111\u00e1y") || cdLower.Contains("t005"))) matchThis = true;
                                        else if (otherDef.Code == 6 && (cdLower.Contains("tron 2") || cdLower.Contains("tr\u1ed9n 2") || cdLower.Contains("t006"))) matchThis = true;
                                        else if (otherDef.Code == 7 && (cdLower.Contains("xa hang") || cdLower.Contains("x\u1ea3 h\u00e0ng") || cdLower.Contains("t007")) && !cdLower.Contains("rung")) matchThis = true;
                                        else if (otherDef.Code == 8 && (cdLower.Contains("rung xa hang") || cdLower.Contains("rung x\u1ea3 h\u00e0ng") || cdLower.Contains("t008"))) matchThis = true;
                                    }

                                    if (matchThis)
                                    {
                                        cdMatchesAny = true;
                                        if (otherDef.Code == def.Code)
                                        {
                                            codeMatches = true;
                                        }
                                    }
                                }
                            }

                            return cdMatchesAny ? codeMatches : timeInStep;
                        }).ToList();

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

                            if (tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                val = NormalizeTemperature(val);
                                threshold = NormalizeTemperature(threshold);
                            }

                            string formatStr = tagName.IndexOf("ApSuat", StringComparison.OrdinalIgnoreCase) >= 0 ? "0.00" : "0.#";
                            string detailMessage = $"Giá trị: {val.ToString(formatStr, CultureInfo.InvariantCulture)} {unit} (ngưỡng: {threshold.ToString(formatStr, CultureInfo.InvariantCulture)} {unit})";

                            stepAlertsList.Add(new
                            {
                                id = Convert.ToInt32(row["id"]),
                                time = alarmTime.ToString("HH:mm:ss"),
                                type = severity,
                                title = msg,
                                message = detailMessage
                            });
                        }

                        stepsList.Add(new
                        {
                            process = def.Name,
                            standard = def.Standard,
                            start = startStr,
                            end = endStr,
                            duration = durationStr,
                            tempTop = "-",
                            tempMid = "-",
                            tempBot = "-",
                            status = status,
                            statusText = statusText,
                            alerts = stepAlertsList
                        });
                    }
                }

                return Json(new { activeStepCode = activeStepCode, runStatus = runStatus, isPaused = isPaused, steps = stepsList }, JsonRequestBehavior.AllowGet);
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

                // 1. Get active run via BatchResolver
                var resolution = LongDucProject.Helpers.BatchResolver.Resolve(connector, null, null);
                int runId = resolution.RunId;

                if (runId <= 0)
                {
                    return Json(new List<object>(), JsonRequestBehavior.AllowGet);
                }

                // 2. Fetch realtime_alarms for active run (limit to top 30 to de-duplicate, then take top 5)
                DataTable dtAlarms = connector.ExecuteQuery(
                    $"SELECT id, DateTime, Severity, TagName, Value, Threshold, Message FROM realtime_alarms " +
                    $"WHERE runId = {runId} AND Severity IN ('ALARM', 'WARNING', 'HIGH', 'AVERAGE', 'LOW') " +
                    $"AND NOT ((TagName LIKE '%ThoiGian%' OR Message LIKE '%thời gian%') AND (Value + 0) <= (Threshold + 0)) " +
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

                        if (tagName.IndexOf("NhietDo", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            val = NormalizeTemperature(val);
                            threshold = NormalizeTemperature(threshold);
                        }

                        string formatStr = tagName.IndexOf("ApSuat", StringComparison.OrdinalIgnoreCase) >= 0 ? "0.00" : "0.#";
                        string detailMessage = $"Giá trị: {val.ToString(formatStr, CultureInfo.InvariantCulture)} {unit} (ngưỡng: {threshold.ToString(formatStr, CultureInfo.InvariantCulture)} {unit})";

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
                var dt = connector.ExecuteQuery($"SELECT id, name, status, run_number, is_paused FROM runs WHERE batch_id = {batch_id} ORDER BY id ASC");
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
                            run_number = Convert.ToInt32(row["run_number"]),
                            is_paused = row["is_paused"] != DBNull.Value ? Convert.ToInt32(row["is_paused"]) : 0
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

        [HttpGet]
        public JsonResult GetStandbyBatchesAndRuns()
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
                };

                // Query standby batches (status IN ('Active', 'Pending'))
                var dtBatches = connector.ExecuteQuery("SELECT id, name, status FROM batches WHERE status IN ('Active', 'Pending') ORDER BY id ASC");
                var batchesList = new List<object>();
                int totalBatches = 0;
                int totalRuns = 0;

                if (dtBatches != null)
                {
                    foreach (DataRow batchRow in dtBatches.Rows)
                    {
                        int batchId = Convert.ToInt32(batchRow["id"]);
                        string batchName = batchRow["name"] != DBNull.Value ? batchRow["name"].ToString() : "";
                        string batchStatus = batchRow["status"] != DBNull.Value ? batchRow["status"].ToString() : "";

                        // Query standby runs (status IN ('Pending', 'Waiting', 'Created')) for this batch
                        var dtRuns = connector.ExecuteQuery($"SELECT id, run_number, name, status FROM runs WHERE batch_id = {batchId} AND status IN ('Pending', 'Waiting', 'Created') ORDER BY run_number ASC, id ASC");
                        var runsList = new List<object>();

                        if (dtRuns != null && dtRuns.Rows.Count > 0)
                        {
                            totalBatches++;
                            foreach (DataRow runRow in dtRuns.Rows)
                            {
                                totalRuns++;
                                runsList.Add(new
                                {
                                    id = Convert.ToInt32(runRow["id"]),
                                    run_number = Convert.ToInt32(runRow["run_number"]),
                                    name = runRow["name"] != DBNull.Value ? runRow["name"].ToString() : "",
                                    status = runRow["status"] != DBNull.Value ? runRow["status"].ToString() : ""
                                });
                            }

                            batchesList.Add(new
                            {
                                id = batchId,
                                name = batchName,
                                status = batchStatus,
                                runs = runsList
                            });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    batches = batchesList,
                    total_batches = totalBatches,
                    total_runs = totalRuns
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SelectBatchRun(int batchId, int runId)
        {
            string connStr = "Server=localhost;Database=scada;Uid=root;Pwd=101101;";
            using (var conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    
                    // 1. Check if there is any run in the database with status 'Active'
                    using (var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM runs WHERE status = 'Active'", conn))
                    {
                        long activeCount = Convert.ToInt64(checkCmd.ExecuteScalar());
                        if (activeCount > 0)
                        {
                            return Json(new { success = false, message = "Không thể bắt đầu vì đang có mẻ chạy đang hoạt động!" });
                        }
                    }

                    // Start a transaction
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                             // 1. Query MIN execution_order of other pending runs and MAX execution_order of all runs
                             int minPendingOrder = 0;
                             int maxOrder = 0;
                             using (var minCmd = new MySqlCommand("SELECT COALESCE(MIN(execution_order), 0) FROM runs WHERE status = 'Pending' AND id != @runId", conn, transaction))
                             {
                                 minCmd.Parameters.AddWithValue("@runId", runId);
                                 minPendingOrder = Convert.ToInt32(minCmd.ExecuteScalar());
                             }
                             using (var maxCmd = new MySqlCommand("SELECT COALESCE(MAX(execution_order), 0) FROM runs", conn, transaction))
                             {
                                 maxOrder = Convert.ToInt32(maxCmd.ExecuteScalar());
                             }
 
                             // 2. Update currently 'Active' runs of any batch to 'Pending' to prevent parallel execution
                             using (var updateRunsPendingCmd = new MySqlCommand("UPDATE runs SET status = 'Pending' WHERE status = 'Active'", conn, transaction))
                             {
                                 updateRunsPendingCmd.ExecuteNonQuery();
                             }
 
                             // 3. Update currently 'Active' batches to 'Pending'
                             using (var updateBatchPendingCmd = new MySqlCommand("UPDATE batches SET status = 'Pending' WHERE status = 'Active'", conn, transaction))
                             {
                                 updateBatchPendingCmd.ExecuteNonQuery();
                             }
 
                             // 4. Update selected batch to 'Active'
                             using (var updateBatchActiveCmd = new MySqlCommand("UPDATE batches SET status = 'Active' WHERE id = @batchId", conn, transaction))
                             {
                                 updateBatchActiveCmd.Parameters.AddWithValue("@batchId", batchId);
                                 updateBatchActiveCmd.ExecuteNonQuery();
                             }
 
                             // 5. Update execution orders to make the selected run next in queue
                             if (minPendingOrder > 0)
                             {
                                 // Shift other pending runs up to make room for the selected run
                                 using (var shiftCmd = new MySqlCommand("UPDATE runs SET execution_order = execution_order + 1 WHERE status = 'Pending' AND id != @runId", conn, transaction))
                                 {
                                     shiftCmd.Parameters.AddWithValue("@runId", runId);
                                     shiftCmd.ExecuteNonQuery();
                                 }
 
                                 // Set selected run to have the lowest pending order
                                 using (var updateRunActiveCmd = new MySqlCommand("UPDATE runs SET execution_order = @newOrder WHERE id = @runId", conn, transaction))
                                 {
                                     updateRunActiveCmd.Parameters.AddWithValue("@newOrder", minPendingOrder);
                                     updateRunActiveCmd.Parameters.AddWithValue("@runId", runId);
                                     updateRunActiveCmd.ExecuteNonQuery();
                                 }
                             }
                             else
                             {
                                 // No other pending runs exist, set selected run to maxOrder + 1
                                 using (var updateRunActiveCmd = new MySqlCommand("UPDATE runs SET execution_order = @newOrder WHERE id = @runId", conn, transaction))
                                 {
                                     updateRunActiveCmd.Parameters.AddWithValue("@newOrder", maxOrder + 1);
                                     updateRunActiveCmd.Parameters.AddWithValue("@runId", runId);
                                     updateRunActiveCmd.ExecuteNonQuery();
                                 }
                             }

                            // Commit the transaction
                            transaction.Commit();
                            return Json(new { success = true, message = "Đã kích hoạt mẻ chạy thành công!" });
                        }
                        catch (Exception ex)
                        {
                            // Rollback if any error occurs
                            transaction.Rollback();
                            return Json(new { success = false, message = "Lỗi trong quá trình cập nhật dữ liệu: " + ex.Message });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi kết nối cơ sở dữ liệu: " + ex.Message });
                }
            }
        }

        private static double NormalizeTemperature(double val)
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

        private string FormatTempRange(List<double> temps, double? threshold = null)
        {
            if (temps == null || temps.Count == 0) return "-";

            var normalizedTemps = temps.Select(t => NormalizeTemperature(t)).ToList();
            double min = normalizedTemps.Min();
            double max = normalizedTemps.Max();

            string minStr = Math.Round(min, 1).ToString("0.#", CultureInfo.InvariantCulture);
            string maxStr = Math.Round(max, 1).ToString("0.#", CultureInfo.InvariantCulture);

            double? normThreshold = threshold.HasValue ? NormalizeTemperature(threshold.Value) : (double?)null;

            bool isMinExceeded = normThreshold.HasValue && min >= normThreshold.Value;
            bool isMaxExceeded = normThreshold.HasValue && max >= normThreshold.Value;

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