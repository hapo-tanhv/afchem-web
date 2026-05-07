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
    }
}