using CsvHelper;
using Hino.Getdata.Project;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Windows.Forms;

namespace LongDucProject.Controllers
{

    public class DataProjectController : Controller
    {
        // GET: DataProject

        string unit;
        string time;
        string projectname;

        #region check data để đặt tên file
        public void checktime(int timeUnit, string starttime)
        {
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
        }

        public void checkproject(string ProjectName)
        {
            if (ProjectName.ToLower() == "project1")
            {
                projectname = "Rental Factory 1";
            }
            else if (ProjectName.ToLower() == "project2")
            {
                projectname = "Rental Factory 3";
            }
            else if (ProjectName.ToLower() == "project3")
            {
                projectname = "Kolmar";
            }
            else if (ProjectName.ToLower() == "project4")
            {
                projectname = "KYC";
            }
            else if (ProjectName.ToLower() == "project5")
            {
                projectname = "Nagae";
            }
            else if (ProjectName.ToLower() == "project6")
            {
                projectname = "Settsu Carton";
            }
            else if (ProjectName.ToLower() == "project7")
            {
                projectname = "Pegasus";
            }
        }
        #endregion

        //Get data for Energy (include for chart and export Excel)
        [HttpGet]
        public JsonResult GetProjectSolarEnergy(string ProjectName, int timeUnit, string starttime, string endtime)
        {
            var Project_Energy = new ProjectDataEnergy();
            var list = Project_Energy.GetProjectSolarEnergy(ProjectName, timeUnit, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdataExportProjectEnergy(string ProjectName, int timeUnit, string starttime, string endtime)
        {
            var Project_Energy = new ProjectDataEnergy();
            var listExcel = Project_Energy.GetProjectSolarEnergy(ProjectName, timeUnit, starttime, endtime).ToList();
            var templatepath = "";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                checktime(timeUnit, starttime);
                if (timeUnit == 1)
                {
                    templatepath = $@"C:\Program Files\ATPro\ATSCADA\Reports\DailyEnergy{ProjectName}Report.xlsx";
                }
                else if (timeUnit == 2)
                {
                    templatepath = $@"C:\Program Files\ATPro\ATSCADA\Reports\MonthlyEnergy{ProjectName}Report.xlsx";
                }
                else if (timeUnit == 3)
                {
                    templatepath = $@"C:\Program Files\ATPro\ATSCADA\Reports\YearlyEnergy{ProjectName}Report.xlsx";
                }
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo(templatepath)))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];

                    int rowstart = 3;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.Value;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.GridValue;
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    //pck.Save();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_Energy_{unit}_{time}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void GetdataExportCSVProjectEnergy(string ProjectName, int timeUnit, string starttime, string endtime)
        {
            var Project_Energy = new ProjectDataEnergy();
            var listExcel = Project_Energy.GetProjectSolarEnergy(ProjectName, timeUnit, starttime, endtime).ToList();

            try
            {
                checkproject(ProjectName);
                checktime(timeUnit, starttime);

                string unit = GetTimeUnitString(timeUnit);
                string formattedProjectName = FormatProjectName(ProjectName);

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
                        string formattedSolarValue = Convert.ToInt32(item.Value).ToString("N0"); // Add thousands separator
                        string formattedGridValue = Convert.ToInt32(item.GridValue).ToString("N0"); // Add thousands separator
                        csv.WriteField(formattedSolarValue);
                        csv.WriteField(formattedGridValue);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_Energy_{unit}_{time}_Report.csv";

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string GetTimeUnitString(int timeUnit)
        {
            if (timeUnit == 1)
            {
                return "Day";
            }
            else if (timeUnit == 2)
            {
                return "Month";
            }
            else if (timeUnit == 3)
            {
                return "Year";
            }

            return "";
        }

        private string FormatProjectName(string projectName)
        {
            return projectName.Replace(" ", "_");
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



        public ActionResult DownloadEnergyProject()
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


        //Get data for Power (include for chart and export Excel)
        [HttpGet]
        public JsonResult GetProjectSolarPower(string ProjectName, string datetime)
        {
            var Project_electrical = new ProjectDataEnergy();
            var list = Project_electrical.GetProjectSolarPower(datetime, ProjectName);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdataExportProjectPower(string datetime, string filepath, string ProjectName)
        {
            var Project_power = new ProjectDataEnergy();
            var listExcel = Project_power.GetProjectSolarPower(datetime, ProjectName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\ElectricalPowerSolar{ProjectName}Report.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.Value;
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_Power_{datetime}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Xuất dữ liệu Power CSV
        public void GetdataExportCSVProjectPower(string datetime, string filepath, string ProjectName)
        {
            var Project_power = new ProjectDataEnergy();
            var listExcel = Project_power.GetProjectSolarPower(datetime, ProjectName).ToList();
            try
            {
                checkproject(ProjectName);
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
                        csv.WriteField(item.Value);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_Power_{datetime}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadPowerProject()
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
        //Get data for Weather (include for chart and export Excel)
        [HttpGet]
        public JsonResult GetProjectWeather(string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var list = Project_weather.GetProjectWeather(ProjectName, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //Get data for Temperature export Excel
        public JsonResult GetdataExportProjectTemp(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\DailyAmbientAirTemperatureReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.AmbientTemperature;
                        ws.Cells[string.Format("J{0}", 4)].Value = $@"{ ProjectName}";

                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_AmbientTemperature_{starttime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get data for Temperature export CSV
        public void GetdataExportCSVProjectTemp(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();
            try
            {
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Ambient Temperature");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.AmbientTemperature);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_AmbientTemperature_{starttime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadAmbientTemperature()
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

        //Get data for Irradian export Excel
        public JsonResult GetdataExportProjectIrradian(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\DailyIrradianceReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.Irradiation;
                        ws.Cells[string.Format("J{0}", 4)].Value = $@"{ ProjectName}";
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_Irradiance_{starttime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get data for Irradian export CSV
        public void GetdataExportCSVProjectIrradian(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();
            try
            {
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Irradiance");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.Irradiation);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_Irradiance_{starttime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadIrradiation()
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

        //Get data for Cell 1 Temperature export Excel
        public JsonResult GetdataExportProjectCell1(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\DailyModuleTemperature1Report.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("D{0}", rowstart)].Value = item.Cell1Temperature;
                        ws.Cells[string.Format("J{0}", 4)].Value = $@"{ ProjectName}";
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_Module Temperature 1_{starttime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get data for Cell 1 Temperature export CSV
        public void GetdataExportCSVProjectCell1(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();
            try
            {
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Module Temperature 1");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.Cell1Temperature);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_Module Temperature 1_{starttime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadCell1Temperature()
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

        //Get data for Cell 2 Temperature export Excel
        public JsonResult GetdataExportProjectCell2(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();
            try
            {
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\DailyWeatherCell2TemperatureReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("E{0}", rowstart)].Value = item.Cell2Temperature;
                        ws.Cells[string.Format("J{0}", 4)].Value = $@"{ ProjectName}";
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_Module Temperature 2_{starttime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get data for Cell 2 Temperature export CSV
        public void GetdataExportCSVProjectCell2(string filepath, string ProjectName, string starttime, string endtime)
        {
            var Project_weather = new ProjectDataWeather();
            var listExcel = Project_weather.GetProjectWeather(ProjectName, starttime, endtime).ToList();
            try
            {
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Module Temperature 2");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.Cell2Temperature);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_Module Temperature 2_{starttime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadCell2Temperature()
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

        // get data for Project Electrical excel
        public JsonResult GetdataExportProjectElectrical(string datetime, string filepath, string ProjectName)
        {
            var Project_electrical = new ProjectDataEnergy();
            //var list = Project_electrical.GetProjectSolarElectrical(datetime, ProjectName);
            var listExcel = Project_electrical.GetProjectSolarElectrical(datetime, ProjectName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\ElectricalCommon{ProjectName}Report.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.VoltageA;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.VoltageB;
                        ws.Cells[string.Format("D{0}", rowstart)].Value = item.VoltageC;
                        ws.Cells[string.Format("E{0}", rowstart)].Value = item.CurrentA;
                        ws.Cells[string.Format("F{0}", rowstart)].Value = item.CurrentB;
                        ws.Cells[string.Format("G{0}", rowstart)].Value = item.CurrentC;
                        ws.Cells[string.Format("H{0}", rowstart)].Value = item.Frequency;
                        ws.Cells[string.Format("I{0}", rowstart)].Value = item.ActivePower;
                        ws.Cells[string.Format("J{0}", rowstart)].Value = item.ReactivePower;
                        ws.Cells[string.Format("K{0}", rowstart)].Value = item.PowerFactor;
                        //ws.Cells[string.Format("E{0}", rowstart)].Value = item.Frequency;           
                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_Electrical_{datetime}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // get data for Project Electrical CSV
        public void GetdataExportCSVProjectElectrical(string datetime, string filepath, string ProjectName)
        {
            var Project_electrical = new ProjectDataEnergy();
            var listExcel = Project_electrical.GetProjectSolarElectrical(datetime, ProjectName).ToList();
            try
            {
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Voltage A(V)");
                    csv.WriteField("Voltage B(V)");
                    csv.WriteField("Voltage C(V)");
                    csv.WriteField("Current A(A)");
                    csv.WriteField("Current B(A)");
                    csv.WriteField("Current C(A)");
                    csv.WriteField("Frequency");
                    csv.WriteField("Active Power");
                    csv.WriteField("Reactive Power");
                    csv.WriteField("Power Factor");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.VoltageA);
                        csv.WriteField(item.VoltageB);
                        csv.WriteField(item.VoltageC);
                        csv.WriteField(item.CurrentA);
                        csv.WriteField(item.CurrentB);
                        csv.WriteField(item.CurrentC);
                        csv.WriteField(item.Frequency);
                        csv.WriteField(item.ActivePower);
                        csv.WriteField(item.ReactivePower);
                        csv.WriteField(item.PowerFactor);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_Electrical_{datetime}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadElectricalProject()
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


        // Xuất dữ liệu và báo cáo cho Alarm của Project
        public JsonResult GetAlarmProject(string starttime, string endtime, string ProjectName, string invertername)
        {
            var Alarm = new ProjectAlarm();
            var list = Alarm.GetAlarmProject(starttime, endtime, ProjectName, invertername).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Xuất dữ liệu ra file Excel
        public ActionResult GetdataExportAlarm(string starttime, string endtime, string filepath, string ProjectName, string invertername)
        {
            var Alarm = new ProjectAlarm();
            var listExcel = Alarm.GetAlarmProject(starttime, endtime, ProjectName, invertername).ToList();
            try
            {
                checkproject(ProjectName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\AlarmReport.xlsx")))
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
                    Session["ReportName"] = $@"{projectname}_Alarm_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Xuất dữ liệu ra file CSV
        public void GetdataCSVExportAlarm(string starttime, string endtime, string ProjectName, string invertername)
        {
            var Alarm = new ProjectAlarm();
            var listExcel = Alarm.GetAlarmProject(starttime, endtime, ProjectName, invertername).ToList();
            try
            {
                checkproject(ProjectName);
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
                Session["ReportName"] = $@"{projectname}_Alarm_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.csv";
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


        //Xuất dữ liệu và báo cáo cho Event của Project

        public JsonResult GetEventProject(string starttime, string endtime, string projectname, string invertername)
        {
            var Event = new ProjectEvent();
            var list = Event.GetEventProject(starttime, endtime, projectname, invertername).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetdataExportEvent(string starttime, string endtime, string filepath, string ProjectName, string invertername)
        {
            var Event = new ProjectEvent();
            var listExcel = Event.GetEventProject(starttime, endtime, ProjectName, invertername).ToList();
            try
            {
                checkproject(ProjectName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\EventReport.xlsx")))
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
                    Session["ReportName"] = $@"{projectname}_Events_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get data for export event CSV
        public void GetdataCSVExportEvent(string starttime, string endtime, string ProjectName, string invertername)
        {
            var Event = new ProjectEvent();
            var listExcel = Event.GetEventProject(starttime, endtime, ProjectName, invertername).ToList();
            try
            {
                checkproject(ProjectName);
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
                Session["ReportName"] = $@"{projectname}_Events_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult DownloadEvent()
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

        //Phần xuất dữ liệu cho Common report
        public JsonResult GetdataExportProjectCommon(string starttime, string endtime, string filepath, string ProjectName)
        {
            var Project_common = new ProjectDataCommon();
            var listExcel = Project_common.GetProjectDataCommon(starttime, endtime, ProjectName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //try
            //{
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\{ProjectName}CommonReport.xlsx")))
                {
                    checkproject(ProjectName);
                    ExcelWorksheet ws = pck.Workbook.Worksheets["Report"];
                    int rowstart = 11;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.TotalEnergyFromINV;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.TotalEnergy;
                        ws.Cells[string.Format("D{0}", rowstart)].Value = item.SurplusPower;
                        ws.Cells[string.Format("E{0}", rowstart)].Value = item.PurchasedPower;
                        ws.Cells[string.Format("F{0}", rowstart)].Value = item.VoltageA;
                        ws.Cells[string.Format("G{0}", rowstart)].Value = item.VoltageB;
                        ws.Cells[string.Format("H{0}", rowstart)].Value = item.VoltageC;
                        ws.Cells[string.Format("I{0}", rowstart)].Value = item.CurrentA;
                        ws.Cells[string.Format("J{0}", rowstart)].Value = item.CurrentB;
                        ws.Cells[string.Format("K{0}", rowstart)].Value = item.CurrentC;
                        ws.Cells[string.Format("L{0}", rowstart)].Value = item.Frequency;
                        ws.Cells[string.Format("M{0}", rowstart)].Value = item.ActivePower;
                        ws.Cells[string.Format("N{0}", rowstart)].Value = item.ReactivePower;
                        ws.Cells[string.Format("O{0}", rowstart)].Value = item.PowerFactor;
                        ws.Cells[string.Format("P{0}", rowstart)].Value = item.AmbientAirTemperature;
                        ws.Cells[string.Format("Q{0}", rowstart)].Value = item.Irradiance;
                        ws.Cells[string.Format("R{0}", rowstart)].Value = item.BackOfModuleTemperature1;
                        ws.Cells[string.Format("S{0}", rowstart)].Value = item.BackOfModuleTemperature2;
                        ws.Cells[string.Format("T{0}", rowstart)].Value = item.SolarPeak;
                        ws.Cells[string.Format("U{0}", rowstart)].Value = item.SolarOffPeak;
                        ws.Cells[string.Format("V{0}", rowstart)].Value = item.SolarNormal;
                        ws.Cells[string.Format("W{0}", rowstart)].Value = item.GridPeakIM;
                        ws.Cells[string.Format("X{0}", rowstart)].Value = item.GridOffPeakIM;
                        ws.Cells[string.Format("Y{0}", rowstart)].Value = item.GridNormalIM;
                        ws.Cells[string.Format("Z{0}", rowstart)].Value = item.GridPeakEX;
                        ws.Cells[string.Format("AA{0}", rowstart)].Value = item.GridOffPeakEX;
                        ws.Cells[string.Format("AB{0}", rowstart)].Value = item.GridNormalEX;
                    rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();
                    ws.Column(1).Style.Numberformat.Format = "dd/mm/yyyy hh:mm";
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_DataCommon_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }

        //Phần xuất dữ liệu cho Common report CSV
        public void GetdataExportCSVProjectCommon(string starttime, string endtime, string ProjectName)
        {
            var Project_common = new ProjectDataCommon();
            var listExcel = Project_common.GetProjectDataCommon(starttime, endtime, ProjectName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("PV Power Generation (from Inverter)(kWh)");
                    csv.WriteField("PV Power Generation (from Power Meter)(kWh)");
                    csv.WriteField("Excess Energy(kWh)");
                    csv.WriteField("Purchased Energy(kWh)");
                    csv.WriteField("Voltage A(kV)");
                    csv.WriteField("Voltage B(kV)");
                    csv.WriteField("Voltage C(kV)");
                    csv.WriteField("Current A(A)");
                    csv.WriteField("Current B(A)");
                    csv.WriteField("Current C(A)");
                    csv.WriteField("Frequency(Hz)");
                    csv.WriteField("Active Power(kW)");
                    csv.WriteField("Reactive Power(kVAr)");
                    csv.WriteField("PowerFactor");
                    csv.WriteField("Ambient Temperature(°C)");
                    csv.WriteField("Irradiance(W/m2)");
                    csv.WriteField("Temperature Module1(°C)");
                    csv.WriteField("Temperature Module2(°C)");
                    csv.WriteField("SolarPeak(kWh)");
                    csv.WriteField("SolarOffPeak(kWh)");
                    csv.WriteField("SolarNormal(kWh)");
                    csv.WriteField("GridBuyPeak(kWh)");
                    csv.WriteField("GridBuyOffPeak(kWh)");
                    csv.WriteField("GridBuyNormal(kWh)");
                    csv.WriteField("GridExcessPeak(kWh)");
                    csv.WriteField("GridExcessOffPeak(kWh)");
                    csv.WriteField("GridExcessNormal(kWh)");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.TotalEnergyFromINV);
                        csv.WriteField(item.TotalEnergy);
                        csv.WriteField(item.SurplusPower);
                        csv.WriteField(item.PurchasedPower);
                        csv.WriteField(item.VoltageA);
                        csv.WriteField(item.VoltageB);
                        csv.WriteField(item.VoltageC);
                        csv.WriteField(item.CurrentA);
                        csv.WriteField(item.CurrentB);
                        csv.WriteField(item.CurrentC);
                        csv.WriteField(item.Frequency);
                        csv.WriteField(item.ActivePower);
                        csv.WriteField(item.ReactivePower);
                        csv.WriteField(item.PowerFactor);
                        csv.WriteField(item.AmbientAirTemperature);
                        csv.WriteField(item.Irradiance);
                        csv.WriteField(item.BackOfModuleTemperature1);
                        csv.WriteField(item.BackOfModuleTemperature2);
                        csv.WriteField(item.SolarPeak);
                        csv.WriteField(item.SolarOffPeak);
                        csv.WriteField(item.SolarNormal);
                        csv.WriteField(item.GridPeakIM);
                        csv.WriteField(item.GridOffPeakIM);
                        csv.WriteField(item.GridNormalIM);
                        csv.WriteField(item.GridPeakEX);
                        csv.WriteField(item.GridOffPeakEX);
                        csv.WriteField(item.GridNormalEX);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_DataCommon_From_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadProjectDataCommon()
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


        //Phần xuất dữ liệu cho CO2 report
        public JsonResult GetdataExportProjectCO2(string ProjectName, int TimeUnit, string starttime, string endtime, string filepath)
        {
            var Project_CO2 = new ProjectDataCO2();
            var projectform = "ProjectDailyCO2Report";
            var listExcel = Project_CO2.GetProjectCO2(ProjectName, TimeUnit, starttime, endtime).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            switch (TimeUnit)
            {
                case 1:
                    projectform = "ProjectDailyCO2Report";
                    break;
                case 2:
                    projectform = "ProjectMonthlyCO2Report";
                    break;
                case 3:
                    projectform = "ProjectYearlyCO2Report";
                    break;
            }
            try
            {
                checktime(TimeUnit, starttime);
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlsx"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\{projectform}.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 3;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.Value;

                        rowstart++;
                    }
                    ws.Cells["A:AZ"].AutoFitColumns();

                    ws.Cells["H1"].Value = projectname;

                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_CO2 {unit} Reduce_{time}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Phần xuất dữ liệu cho CO2 report CSV
        public void GetdataExportCSVProjectCO2(string ProjectName, int TimeUnit, string starttime, string endtime)
        {
            var Project_CO2 = new ProjectDataCO2();
            var listExcel = Project_CO2.GetProjectCO2(ProjectName, TimeUnit, starttime, endtime).ToList();

            try
            {
                checktime(TimeUnit, starttime);
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("CO2 Value");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.Value);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_CO2 {unit} Reduce_{time}_Report.csv";

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadProjectDataCO2()
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