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
using System.Windows.Forms;

namespace LongDucProject.Controllers
{
    public class EventController : Controller
    {
        [HttpGet]
        // GET: Event
        public JsonResult GetEventLog(string starttime, string endtime)
        {
            var Event = new EventCommon();
            var list = Event.GetEventLog(starttime, endtime).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEventData(string starttime, string endtime, string batchId, int draw, int start, int length)
        {
            var Event = new EventCommon();
            var resultList = Event.GetEventLog(starttime, endtime);
            var list = resultList != null ? resultList.ToList() : new List<Hino.Parameter.Common.EventParameter>();

            var searchValue = Request.Form["search[value]"];
            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.Location != null && x.Location.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Status != null && x.Status.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
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
        public FileResult ExportEventExcel(string starttime, string endtime, string batchId, string searchValue)
        {
            var Event = new EventCommon();
            var resultList = Event.GetEventLog(starttime, endtime);
            var list = resultList != null ? resultList.ToList() : new List<Hino.Parameter.Common.EventParameter>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.Location != null && x.Location.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Status != null && x.Status.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToExcel(list, "Events");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Events_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        public FileResult ExportEventCsv(string starttime, string endtime, string batchId, string searchValue)
        {
            var Event = new EventCommon();
            var resultList = Event.GetEventLog(starttime, endtime);
            var list = resultList != null ? resultList.ToList() : new List<Hino.Parameter.Common.EventParameter>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.Location != null && x.Location.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.Status != null && x.Status.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToCsv(list);
            return File(fileBytes, "text/csv", $"Events_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        public ActionResult GetdataExportEvent(string starttime, string endtime, string filepath)
        {
            var Event = new EventCommon();
            var list = Event.GetEventLog(starttime, endtime);
            var x = filepath;
            var listExcel = Event.GetEventLog(starttime, endtime).ToList();
            try
            {

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlxs"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\EventReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["Event"];

                    int rowstart = 10;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.count;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.OccurrenceTime;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.Location;
                        ws.Cells[string.Format("D{0}", rowstart)].Value = item.Description;
                        ws.Cells[string.Format("E{0}", rowstart)].Value = item.Status;

                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    //pck.Save();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"Event_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public void GetdataCSVExportEvent(string starttime, string endtime, string filepath)
        {
            var Event = new EventCommon();
            var listExcel = Event.GetEventLog(starttime, endtime).ToList();
            try
            {

                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("No");
                    csv.WriteField("OccurrenceTime");
                    csv.WriteField("Plant Name");
                    csv.WriteField("Device");
                    csv.WriteField("Status");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.count);
                        csv.WriteField(item.OccurrenceTime);
                        csv.WriteField(item.Location);
                        csv.WriteField(item.Description);
                        csv.WriteField(item.Status);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"Events_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.csv";     
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
                return File(data, "application/octet-stream", $@"{Session["ReportName"]}");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}
