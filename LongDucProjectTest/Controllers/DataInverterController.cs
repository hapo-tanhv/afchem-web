using CsvHelper;
using Hino.Getdata.Project;
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
    public class DataInverterController : Controller
    {
        string unit;
        string time;
        string projectname;
        string invertername;
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

        public void checkinverter(string InverterName)
        {
            if (InverterName == "INV1")
                invertername = "Inverter 1";
            else if (InverterName == "INV2")
                invertername = "Inverter 2";
            else if (InverterName == "INV3")
                invertername = "Inverter 3";
            else if (InverterName == "INV4")
                invertername = "Inverter 4";
            else if (InverterName == "INV5")
                invertername = "Inverter 5";
            else if (InverterName == "INV6")
                invertername = "Inverter 6";
            else if (InverterName == "INV7")
                invertername = "Inverter 7";
            else if (InverterName == "INV8")
                invertername = "Inverter 8";
            else if (InverterName == "INV9")
                invertername = "Inverter 9";
            else if (InverterName == "INV10")
                invertername = "Inverter 10";
        }

        #endregion
        // GET: DataInverter
        [HttpGet]
        public JsonResult GetInverterSolarEnergy(string ProjectName, string InverterName, int TimeUnit, string starttime, string endtime)
        {
            var Inverter_Energy = new ProjectDataInverter();
            var list = Inverter_Energy.GetInverterSolarEnergy(ProjectName, InverterName, TimeUnit, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Xuất report excel inverter energy
        public JsonResult GetdataExportInverterEnergy(string ProjectName, string InverterName, int timeUnit, string starttime, string endtime, string filepath)
        {
            var Inverter_Energy = new ProjectDataInverter();
            var listExcel = Inverter_Energy.GetInverterSolarEnergy(ProjectName, InverterName, timeUnit, starttime, endtime).ToList();
            var templatepath = "";
            try
            {
                checktime(timeUnit, starttime);
                checkinverter(InverterName);
                checkproject(ProjectName);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (timeUnit == 1)
                {
                    templatepath = $@"C:\Program Files\ATPro\ATSCADA\Reports\DailyEnergyInverterReport.xlsx";
                }
                else if (timeUnit == 2)
                {
                    templatepath = $@"C:\Program Files\ATPro\ATSCADA\Reports\MonthlyEnergyInverterReport.xlsx";
                }
                else if (timeUnit == 3)
                {
                    templatepath = $@"C:\Program Files\ATPro\ATSCADA\Reports\YearlyEnergyInverterReport.xlsx";
                }


                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlxs"), new FileInfo(templatepath)))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];

                    int rowstart = 3;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.Value;
                        rowstart++;
                    }
                    ws.Cells[string.Format("M{0}", 1)].Value = invertername;
                    ws.Cells["A:AZ"].AutoFitColumns();
                    //pck.Save();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_{invertername}_Energy_{unit}_{time}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Xuất report CSV inverter energy
        //public void GetdataExportCSVInverterEnergy(string ProjectName, string InverterName, int timeUnit, string starttime, string endtime, string filepath)
        //{
        //    var Inverter_Energy = new ProjectDataInverter();
        //    var listExcel = Inverter_Energy.GetInverterSolarEnergy(ProjectName, InverterName, timeUnit, starttime, endtime).ToList();
        //    try
        //    {
        //        checktime(timeUnit, starttime);
        //        checkinverter(InverterName);
        //        checkproject(ProjectName);
        //        var stream = new MemoryStream();
        //        using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
        //        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //        {
        //            csv.WriteField("DateTime");
        //            csv.WriteField("Inverter Solar Energy");
        //            csv.NextRecord();
        //            foreach (var item in listExcel)
        //            {
        //                csv.WriteField(item.DateTime);
        //                csv.WriteField(item.Value);
        //                csv.NextRecord();
        //            }
        //        }
        //        Session["DownloadExcel_FileManager"] = stream.ToArray();
        //        Session["ReportName"] = $@"{projectname}_{invertername}_Energy_{unit}_{time}_Report.csv";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        public void GetdataExportCSVInverterEnergy(string ProjectName, string InverterName, int timeUnit, string starttime, string endtime, string filepath)
        {
            var Inverter_Energy = new ProjectDataInverter();
            var listExcel = Inverter_Energy.GetInverterSolarEnergy(ProjectName, InverterName, timeUnit, starttime, endtime).ToList();

            try
            {
                checktime(timeUnit, starttime);
                checkinverter(InverterName);
                checkproject(ProjectName);

                string unit = GetTimeUnitString(timeUnit);
                string formattedProjectName = FormatProjectName(ProjectName);
                string formattedInverterName = FormatInverterName(InverterName);

                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");

                    if (timeUnit == 1)
                    {
                        csv.WriteField("Inverter Solar Energy");
                    }
                    else
                    {
                        csv.WriteField($"Inverter Solar Energy ({unit})");
                    }

                    csv.NextRecord();

                    foreach (var item in listExcel)
                    {
                        //string formattedDateTime = GetFormattedDateTime(item.DateTime, timeUnit);
                        //csv.WriteField(formattedDateTime);
                        //csv.WriteField(item.Value);
                        //csv.NextRecord();

                        string formattedDateTime = GetFormattedDateTime(item.DateTime, timeUnit);
                        csv.WriteField(formattedDateTime);
                        string formattedValue = Convert.ToInt32(item.Value).ToString("N0"); // Add thousands separator
                        csv.WriteField(formattedValue);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_{invertername}_Energy_{unit}_{time}_Report.csv";
                //Session["DownloadExcel_FileManager"] = stream.ToArray();
                //Session["ReportName"] = $@"{formattedProjectName}_{formattedInverterName}_Energy_{unit}_{starttime}_{endtime}_Report.csv";
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
            // Implement your logic to format project name if needed
            // For example, you can remove spaces or special characters
            return projectName.Replace(" ", "_");
        }

        private string FormatInverterName(string inverterName)
        {
            // Implement your logic to format inverter name if needed
            // For example, you can remove spaces or special characters
            return inverterName.Replace(" ", "_");
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

        public ActionResult DownloadEnergyInverter()
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
        public JsonResult GetInverterSolarPower(string ProjectName, string datetime, string InverterName)
        {
            var Inverter_electrical = new ProjectDataInverter();
            var list = Inverter_electrical.GetInverterSolarPower(datetime, ProjectName, InverterName);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //xuất data inverter power excel
        public JsonResult GetdataExportInverterPower(string datetime, string filepath, string ProjectName, string InverterName)
        {
            var Inverter_power = new ProjectDataInverter();
            var listExcel = Inverter_power.GetInverterSolarPower(datetime, ProjectName, InverterName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkinverter(InverterName);
                checkproject(ProjectName);

                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlxs"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\ElectricalPowerInverterReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.Value;
                        ws.Cells[string.Format("D{0}", 3)].Value = $@"{ProjectName} {InverterName} Report";
                        rowstart++;
                    }
                    ws.Cells[string.Format("M{0}", 1)].Value = invertername;
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_{invertername}_Power_{time}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //xuất data inverter power CSV
        public void GetdataExportCSVInverterPower(string datetime, string ProjectName, string InverterName)
        {
            var Inverter_power = new ProjectDataInverter();
            var listExcel = Inverter_power.GetInverterSolarPower(datetime, ProjectName, InverterName).ToList();
            try
            {
                checkinverter(InverterName);
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Inverter Solar Power");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.Value);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_{invertername}_Power_{datetime}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult DownloadPowerInverter()
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

        // Get data for Electrical inverter export excel
        public JsonResult GetdataExportInverterElectrical(string starttime, string endtime, string filepath, string ProjectName, string InverterName)
        {
            var Inverter_electrical = new ProjectDataInverter();
            var listExcel = Inverter_electrical.GetInverterSolarElectrical(starttime, endtime, ProjectName, InverterName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkinverter(InverterName);
                checkproject(ProjectName);
                using (ExcelPackage pck = new ExcelPackage(new FileInfo("report.xlxs"), new FileInfo($@"C:\Program Files\ATPro\ATSCADA\Reports\ElectricalCommonInverterReport.xlsx")))
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets["data"];
                    int rowstart = 2;
                    foreach (var item in listExcel)
                    {
                        ws.Cells[string.Format("A{0}", rowstart)].Value = item.DateTime;
                        ws.Cells[string.Format("B{0}", rowstart)].Value = item.DailyEnergy;
                        ws.Cells[string.Format("C{0}", rowstart)].Value = item.TotalEnergy;
                        ws.Cells[string.Format("D{0}", rowstart)].Value = item.GridVoltagePhaseA;
                        ws.Cells[string.Format("E{0}", rowstart)].Value = item.GridVoltagePhaseB;
                        ws.Cells[string.Format("F{0}", rowstart)].Value = item.GridVoltagePhaseC;
                        ws.Cells[string.Format("G{0}", rowstart)].Value = item.GridCurrentPhaseA;
                        ws.Cells[string.Format("H{0}", rowstart)].Value = item.GridCurrentPhaseB;
                        ws.Cells[string.Format("I{0}", rowstart)].Value = item.GridCurrentPhaseC;
                        ws.Cells[string.Format("J{0}", rowstart)].Value = item.OutputActivePower;
                        ws.Cells[string.Format("K{0}", rowstart)].Value = item.OutputReactivePower;
                        ws.Cells[string.Format("L{0}", rowstart)].Value = item.PowerFactor;
                        ws.Cells[string.Format("M{0}", rowstart)].Value = item.GridFrequency;
                        ws.Cells[string.Format("N{0}", rowstart)].Value = item.Temperature;

                        ws.Cells[string.Format("O{0}", rowstart)].Value = item.MPPT1Voltage;
                        ws.Cells[string.Format("P{0}", rowstart)].Value = item.MPPT2Voltage;
                        ws.Cells[string.Format("Q{0}", rowstart)].Value = item.MPPT3Voltage;
                        ws.Cells[string.Format("R{0}", rowstart)].Value = item.MPPT4Voltage;
                        ws.Cells[string.Format("S{0}", rowstart)].Value = item.MPPT5Voltage;
                        ws.Cells[string.Format("T{0}", rowstart)].Value = item.MPPT6Voltage;
                        ws.Cells[string.Format("U{0}", rowstart)].Value = item.MPPT7Voltage;
                        ws.Cells[string.Format("V{0}", rowstart)].Value = item.MPPT8Voltage;
                        ws.Cells[string.Format("W{0}", rowstart)].Value = item.MPPT9Voltage;
                        ws.Cells[string.Format("X{0}", rowstart)].Value = item.MPPT10Voltage;
                        ws.Cells[string.Format("Y{0}", rowstart)].Value = item.MPPT11Voltage;
                        ws.Cells[string.Format("Z{0}", rowstart)].Value = item.MPPT12Voltage;

                        ws.Cells[string.Format("AA{0}", rowstart)].Value = item.MPPT1Current;
                        ws.Cells[string.Format("AB{0}", rowstart)].Value = item.MPPT2Current;
                        ws.Cells[string.Format("AC{0}", rowstart)].Value = item.MPPT3Current;
                        ws.Cells[string.Format("AD{0}", rowstart)].Value = item.MPPT4Current;
                        ws.Cells[string.Format("AE{0}", rowstart)].Value = item.MPPT5Current;
                        ws.Cells[string.Format("AF{0}", rowstart)].Value = item.MPPT6Current;
                        ws.Cells[string.Format("AG{0}", rowstart)].Value = item.MPPT7Current;
                        ws.Cells[string.Format("AH{0}", rowstart)].Value = item.MPPT8Current;
                        ws.Cells[string.Format("AI{0}", rowstart)].Value = item.MPPT9Current;
                        ws.Cells[string.Format("AJ{0}", rowstart)].Value = item.MPPT10Current;
                        ws.Cells[string.Format("AK{0}", rowstart)].Value = item.MPPT11Current;
                        ws.Cells[string.Format("AL{0}", rowstart)].Value = item.MPPT12Current;
                        rowstart++;
                    }
                    ws.Cells[string.Format("AM{0}", 1)].Value = invertername;
                    ws.Cells["A:AZ"].AutoFitColumns();
                    Session["DownloadExcel_FileManager"] = pck.GetAsByteArray();
                    Session["ReportName"] = $@"{projectname}_{invertername}_Electrical_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.xlsx";
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // Get data for Electrical inverter export CSV
        public void GetdataExportCSVInverterElectrical(string starttime, string endtime, string ProjectName, string InverterName)
        {
            var Inverter_electrical = new ProjectDataInverter();
            var listExcel = Inverter_electrical.GetInverterSolarElectrical(starttime, endtime, ProjectName, InverterName).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                checkinverter(InverterName);
                checkproject(ProjectName);
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("DateTime");
                    csv.WriteField("Daily Energy");
                    csv.WriteField("Total Energy");
                    csv.WriteField("Grid Voltage Phase A");
                    csv.WriteField("Grid Voltage Phase B");
                    csv.WriteField("Grid Voltage Phase C");
                    csv.WriteField("Grid Current Phase A");
                    csv.WriteField("Grid Current Phase B");
                    csv.WriteField("Grid Current Phase C");
                    csv.WriteField("Output Active Power");
                    csv.WriteField("Output Reactive Power");
                    csv.WriteField("Power Factor");
                    csv.WriteField("Grid Frequency");
                    csv.WriteField("Temperature");

                    csv.WriteField("MPPT1 Voltage");
                    csv.WriteField("MPPT2 Voltage");
                    csv.WriteField("MPPT3 Voltage");
                    csv.WriteField("MPPT4 Voltage");
                    csv.WriteField("MPPT5 Voltage");
                    csv.WriteField("MPPT6 Voltage");
                    csv.WriteField("MPPT7 Voltage");
                    csv.WriteField("MPPT8 Voltage");
                    csv.WriteField("MPPT9 Voltage");
                    csv.WriteField("MPPT10 Voltage");
                    csv.WriteField("MPPT11 Voltage");
                    csv.WriteField("MPPT12 Voltage");

                    csv.WriteField("MPPT1 Current");
                    csv.WriteField("MPPT2 Current");
                    csv.WriteField("MPPT3 Current");
                    csv.WriteField("MPPT4 Current");
                    csv.WriteField("MPPT5 Current");
                    csv.WriteField("MPPT6 Current");
                    csv.WriteField("MPPT7 Current");
                    csv.WriteField("MPPT8 Current");
                    csv.WriteField("MPPT9 Current");
                    csv.WriteField("MPPT10 Current");
                    csv.WriteField("MPPT11 Current");
                    csv.WriteField("MPPT12 Current");
                    csv.NextRecord();
                    foreach (var item in listExcel)
                    {
                        csv.WriteField(item.DateTime);
                        csv.WriteField(item.DailyEnergy);
                        csv.WriteField(item.TotalEnergy);
                        csv.WriteField(item.GridVoltagePhaseA);
                        csv.WriteField(item.GridVoltagePhaseB);
                        csv.WriteField(item.GridVoltagePhaseC);
                        csv.WriteField(item.GridCurrentPhaseA);
                        csv.WriteField(item.GridCurrentPhaseB);
                        csv.WriteField(item.GridCurrentPhaseC);
                        csv.WriteField(item.OutputActivePower);
                        csv.WriteField(item.OutputReactivePower);
                        csv.WriteField(item.PowerFactor);
                        csv.WriteField(item.GridFrequency);
                        csv.WriteField(item.Temperature);

                        csv.WriteField(item.MPPT1Voltage);
                        csv.WriteField(item.MPPT2Voltage);
                        csv.WriteField(item.MPPT3Voltage);
                        csv.WriteField(item.MPPT4Voltage);
                        csv.WriteField(item.MPPT5Voltage);
                        csv.WriteField(item.MPPT6Voltage);
                        csv.WriteField(item.MPPT7Voltage);
                        csv.WriteField(item.MPPT8Voltage);
                        csv.WriteField(item.MPPT9Voltage);
                        csv.WriteField(item.MPPT10Voltage);
                        csv.WriteField(item.MPPT11Voltage);
                        csv.WriteField(item.MPPT12Voltage);

                        csv.WriteField(item.MPPT1Current);
                        csv.WriteField(item.MPPT2Current);
                        csv.WriteField(item.MPPT3Current);
                        csv.WriteField(item.MPPT4Current);

                        csv.WriteField(item.MPPT5Current);
                        csv.WriteField(item.MPPT6Current);
                        csv.WriteField(item.MPPT7Current);
                        csv.WriteField(item.MPPT8Current);
                        csv.WriteField(item.MPPT9Current);
                        csv.WriteField(item.MPPT10Current);
                        csv.WriteField(item.MPPT11Current);
                        csv.WriteField(item.MPPT12Current);
                        csv.NextRecord();
                    }
                }
                Session["DownloadExcel_FileManager"] = stream.ToArray();
                Session["ReportName"] = $@"{projectname}_{invertername}_Electrical_{starttime.Substring(0, 10)}_To_{endtime.Substring(0, 10)}_Report.csv";
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult DownloadElectricalInverter()
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