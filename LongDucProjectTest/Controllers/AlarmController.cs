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

        //Xuất dữ liệu ra file Excel
        public ActionResult GetdataExportAlarm(string starttime, string endtime, string filepath)
        {
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
}