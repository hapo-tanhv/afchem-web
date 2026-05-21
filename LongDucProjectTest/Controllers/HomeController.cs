using Hino.Getdata.Common;
using LongDucProjectTest.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hino.DatabaseConnector;
using System.Data;
using System.Globalization;
using LongDucProjectTest.Service;

namespace LongDucProject.Controllers
{
    public class LoginParam
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class UserParam
    {
        public string UserName { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }



    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginParam param)
        {
            if (!ModelState.IsValid) return View();

            var login = new Authentication();
            var role = login.CheckUser(param.UserName, param.Password);
            if (role == Role.None)
            {
                ModelState.AddModelError(string.Empty, "The UserName or Password is incorrect!");
                return View();
            }

            Session["Role"] = (int)role;
            switch (role)
            {
                case Role.Admin:
                    return RedirectToAction("Overview", "Home");
                case Role.SolEnergy:
                    return RedirectToAction("Overview", "Home");
                case Role.JGC:
                    return RedirectToAction("Overview", "Home");
                case Role.Hino:
                    return RedirectToAction("Overview", "Home");
                case Role.LDIP:
                    return RedirectToAction("Home", "LDIP");
                case Role.Project1:
                    return RedirectToAction("Info", "Project1");
                case Role.Project2:
                    return RedirectToAction("Info", "Project2");
                case Role.Project3:
                    return RedirectToAction("Info", "Project3");
                case Role.Project4:
                    return RedirectToAction("Info", "Project4");
                case Role.Project5:
                    return RedirectToAction("Info", "Project5");
                case Role.Project6:
                    return RedirectToAction("Info", "Project6");
                case Role.Project7:
                    return RedirectToAction("Info", "Project7");
                default:
                    return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public JsonResult ChangePassword(UserParam param)
        {
            var status = false;
            var message = string.Empty;

            if (param.NewPassword != param.ConfirmPassword)
            {
                message = "Password & confirm pass is not same";
            }
            else
            {
                var login = new Authentication();
                status = login.ChangePassword(
                    param.UserName,
                    param.OldPassword,
                    param.NewPassword);
                message = status ?
                    "Change password successfully" :
                    "Please check the wrong user or password";
            }

            return Json(new { Status = status, Message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Home()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else
            {
                ViewBag.DisplayAdmin = "none";
            }
            return View();
        }

        public ActionResult Report()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            return View();
        }

        [HttpPost]
        public JsonResult GetReportData(string starttime, string endtime, string batchId, int draw, int start, int length)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
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

                string baseQuery = "FROM alarmreport a INNER JOIN batches b ON a.batchId = b.id WHERE 1=1";
                string filterQuery = $" AND a.DateTime >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND a.DateTime <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}'";

                if (hasBatchFilter)
                {
                    filterQuery += $" AND a.batchId = {parsedBatchId}";
                }

                string searchValue = Request.Form["search[value]"];
                if (!string.IsNullOrEmpty(searchValue))
                {
                    filterQuery += $" AND (b.name LIKE '%{searchValue}%' OR a.ApSuat LIKE '%{searchValue}%' OR a.NhietDoMoiTruong LIKE '%{searchValue}%' OR a.NhietDoBonTronTren LIKE '%{searchValue}%')";
                }

                string countQuery = $"SELECT COUNT(*) {baseQuery} {filterQuery}";
                int recordsFiltered = Convert.ToInt32(connector.ExecuteScalarQuery(countQuery));
                int recordsTotal = recordsFiltered;

                string dataQuery = $"SELECT a.DateTime, a.QuyTrinh, a.CongDoanMay, a.ThoiGianCapLieu, a.ThoiGianTron1, a.ThoiGianXaDay, a.ThoiGianRungXaDay, a.ThoiGianHutXaDay, a.ThoiGianTron2, a.ThoiGianXaHang, a.ThoiGianRungXaHang, a.ApSuat, a.NhietDoMoiTruong, a.DoAmMoiTruong, a.NhietDoBonTronTren, a.NhietDoBonTronGiua, a.NhietDoBonTronDuoi {baseQuery} {filterQuery} ORDER BY a.DateTime DESC, a.ID DESC LIMIT {length} OFFSET {start}";

                var data = new List<object>();
                var dt = connector.ExecuteQuery(dataQuery);
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var dateTimeVal = row["DateTime"] != DBNull.Value ? Convert.ToDateTime(row["DateTime"]) : DateTime.MinValue;
                        string dateStr = dateTimeVal != DateTime.MinValue ? dateTimeVal.ToString("yyyy/MM/dd") : "-";
                        string timeStr = dateTimeVal != DateTime.MinValue ? dateTimeVal.ToString("HH:mm:ss") : "-";

                        double tgCapLieu = TryGetDouble(row["ThoiGianCapLieu"]);
                        double tgTron1 = TryGetDouble(row["ThoiGianTron1"]);
                        double tgXaDay = TryGetDouble(row["ThoiGianXaDay"]);
                        double tgRungXaDay = TryGetDouble(row["ThoiGianRungXaDay"]);
                        double tgHutXaDay = TryGetDouble(row["ThoiGianHutXaDay"]);
                        double tgTron2 = TryGetDouble(row["ThoiGianTron2"]);
                        double tgXaHang = TryGetDouble(row["ThoiGianXaHang"]);
                        double tgRungXaHang = TryGetDouble(row["ThoiGianRungXaHang"]);

                        double tongTgTron = tgCapLieu + tgTron1 + tgXaDay + tgRungXaDay + tgHutXaDay + tgTron2 + tgXaHang + tgRungXaHang;

                        data.Add(new
                        {
                            Date = dateStr,
                            Time = timeStr,
                            QuyTrinh = row["QuyTrinh"] != DBNull.Value ? Convert.ToInt32(row["QuyTrinh"]) : 0,
                            CongDoan = row["CongDoanMay"] != DBNull.Value ? Convert.ToInt32(row["CongDoanMay"]) : 0,
                            TgCapLieu = tgCapLieu,
                            TgTron1 = tgTron1,
                            TgXaDay = tgXaDay,
                            TgRungXaDay = tgRungXaDay,
                            TgHutXaDay = tgHutXaDay,
                            TgTron2 = tgTron2,
                            TgXaHang = tgXaHang,
                            TgRungXaHang = tgRungXaHang,
                            TongTgTron = tongTgTron,
                            ApSuat = TryGetDouble(row["ApSuat"]),
                            NhietDoMT = TryGetDouble(row["NhietDoMoiTruong"]),
                            DoAmMT = TryGetDouble(row["DoAmMoiTruong"]),
                            NhietNapBon = TryGetDouble(row["NhietDoBonTronTren"]),
                            NhietGiuaBon = TryGetDouble(row["NhietDoBonTronGiua"]),
                            NhietDayBon = TryGetDouble(row["NhietDoBonTronDuoi"])
                        });
                    }
                }

                return Json(new
                {
                    draw = draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = data
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

        [HttpGet]
        public FileResult ExportReportExcel(string starttime, string endtime, string batchId, string searchValue)
        {
            var connector = new MySQLConnect()
            {
                ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
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

            string baseQuery = "FROM alarmreport a INNER JOIN batches b ON a.batchId = b.id WHERE 1=1";
            string filterQuery = $" AND a.DateTime >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND a.DateTime <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (hasBatchFilter)
            {
                filterQuery += $" AND a.batchId = {parsedBatchId}";
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                filterQuery += $" AND (b.name LIKE '%{searchValue}%' OR a.ApSuat LIKE '%{searchValue}%' OR a.NhietDoMoiTruong LIKE '%{searchValue}%' OR a.NhietDoBonTronTren LIKE '%{searchValue}%')";
            }

            string query = $"SELECT a.DateTime, a.QuyTrinh, a.CongDoanMay, a.ThoiGianCapLieu, a.ThoiGianTron1, a.ThoiGianXaDay, a.ThoiGianRungXaDay, a.ThoiGianHutXaDay, a.ThoiGianTron2, a.ThoiGianXaHang, a.ThoiGianRungXaHang, a.ApSuat, a.NhietDoMoiTruong, a.DoAmMoiTruong, a.NhietDoBonTronTren, a.NhietDoBonTronGiua, a.NhietDoBonTronDuoi {baseQuery} {filterQuery} ORDER BY a.DateTime DESC, a.ID DESC";

            var list = new List<ReportExportDto>();
            var dt = connector.ExecuteQuery(query);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var dateTimeVal = row["DateTime"] != DBNull.Value ? Convert.ToDateTime(row["DateTime"]) : DateTime.MinValue;
                    string dateStr = dateTimeVal != DateTime.MinValue ? dateTimeVal.ToString("yyyy/MM/dd") : "-";
                    string timeStr = dateTimeVal != DateTime.MinValue ? dateTimeVal.ToString("HH:mm:ss") : "-";

                    double tgCapLieu = TryGetDouble(row["ThoiGianCapLieu"]);
                    double tgTron1 = TryGetDouble(row["ThoiGianTron1"]);
                    double tgXaDay = TryGetDouble(row["ThoiGianXaDay"]);
                    double tgRungXaDay = TryGetDouble(row["ThoiGianRungXaDay"]);
                    double tgHutXaDay = TryGetDouble(row["ThoiGianHutXaDay"]);
                    double tgTron2 = TryGetDouble(row["ThoiGianTron2"]);
                    double tgXaHang = TryGetDouble(row["ThoiGianXaHang"]);
                    double tgRungXaHang = TryGetDouble(row["ThoiGianRungXaHang"]);

                    double tongTgTron = tgCapLieu + tgTron1 + tgXaDay + tgRungXaDay + tgHutXaDay + tgTron2 + tgXaHang + tgRungXaHang;

                    list.Add(new ReportExportDto
                    {
                        Ngay = dateStr,
                        Gio = timeStr,
                        QuyTrinh = row["QuyTrinh"] != DBNull.Value ? Convert.ToInt32(row["QuyTrinh"]) : 0,
                        CongDoan = row["CongDoanMay"] != DBNull.Value ? Convert.ToInt32(row["CongDoanMay"]) : 0,
                        TgCapLieu = tgCapLieu,
                        TgTron1 = tgTron1,
                        TgXaDay = tgXaDay,
                        TgRungXaDay = tgRungXaDay,
                        TgHutXaDay = tgHutXaDay,
                        TgTron2 = tgTron2,
                        TgXaHang = tgXaHang,
                        TgRungXaHang = tgRungXaHang,
                        TongTgTron = tongTgTron,
                        ApSuat = TryGetDouble(row["ApSuat"]),
                        NhietDoMT = TryGetDouble(row["NhietDoMoiTruong"]),
                        DoAmMT = TryGetDouble(row["DoAmMoiTruong"]),
                        NhietNapBon = TryGetDouble(row["NhietDoBonTronTren"]),
                        NhietGiuaBon = TryGetDouble(row["NhietDoBonTronGiua"]),
                        NhietDayBon = TryGetDouble(row["NhietDoBonTronDuoi"])
                    });
                }
            }

            byte[] fileBytes = ExportUtility.ExportToExcel(list, "Report");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        public FileResult ExportReportCsv(string starttime, string endtime, string batchId, string searchValue)
        {
            var connector = new MySQLConnect()
            {
                ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;"
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

            string baseQuery = "FROM alarmreport a INNER JOIN batches b ON a.batchId = b.id WHERE 1=1";
            string filterQuery = $" AND a.DateTime >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND a.DateTime <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}'";

            if (hasBatchFilter)
            {
                filterQuery += $" AND a.batchId = {parsedBatchId}";
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                filterQuery += $" AND (b.name LIKE '%{searchValue}%' OR a.ApSuat LIKE '%{searchValue}%' OR a.NhietDoMoiTruong LIKE '%{searchValue}%' OR a.NhietDoBonTronTren LIKE '%{searchValue}%')";
            }

            string query = $"SELECT a.DateTime, a.QuyTrinh, a.CongDoanMay, a.ThoiGianCapLieu, a.ThoiGianTron1, a.ThoiGianXaDay, a.ThoiGianRungXaDay, a.ThoiGianHutXaDay, a.ThoiGianTron2, a.ThoiGianXaHang, a.ThoiGianRungXaHang, a.ApSuat, a.NhietDoMoiTruong, a.DoAmMoiTruong, a.NhietDoBonTronTren, a.NhietDoBonTronGiua, a.NhietDoBonTronDuoi {baseQuery} {filterQuery} ORDER BY a.DateTime DESC, a.ID DESC";

            var list = new List<ReportExportDto>();
            var dt = connector.ExecuteQuery(query);
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var dateTimeVal = row["DateTime"] != DBNull.Value ? Convert.ToDateTime(row["DateTime"]) : DateTime.MinValue;
                    string dateStr = dateTimeVal != DateTime.MinValue ? dateTimeVal.ToString("yyyy/MM/dd") : "-";
                    string timeStr = dateTimeVal != DateTime.MinValue ? dateTimeVal.ToString("HH:mm:ss") : "-";

                    double tgCapLieu = TryGetDouble(row["ThoiGianCapLieu"]);
                    double tgTron1 = TryGetDouble(row["ThoiGianTron1"]);
                    double tgXaDay = TryGetDouble(row["ThoiGianXaDay"]);
                    double tgRungXaDay = TryGetDouble(row["ThoiGianRungXaDay"]);
                    double tgHutXaDay = TryGetDouble(row["ThoiGianHutXaDay"]);
                    double tgTron2 = TryGetDouble(row["ThoiGianTron2"]);
                    double tgXaHang = TryGetDouble(row["ThoiGianXaHang"]);
                    double tgRungXaHang = TryGetDouble(row["ThoiGianRungXaHang"]);

                    double tongTgTron = tgCapLieu + tgTron1 + tgXaDay + tgRungXaDay + tgHutXaDay + tgTron2 + tgXaHang + tgRungXaHang;

                    list.Add(new ReportExportDto
                    {
                        Ngay = dateStr,
                        Gio = timeStr,
                        QuyTrinh = row["QuyTrinh"] != DBNull.Value ? Convert.ToInt32(row["QuyTrinh"]) : 0,
                        CongDoan = row["CongDoanMay"] != DBNull.Value ? Convert.ToInt32(row["CongDoanMay"]) : 0,
                        TgCapLieu = tgCapLieu,
                        TgTron1 = tgTron1,
                        TgXaDay = tgXaDay,
                        TgRungXaDay = tgRungXaDay,
                        TgHutXaDay = tgHutXaDay,
                        TgTron2 = tgTron2,
                        TgXaHang = tgXaHang,
                        TgRungXaHang = tgRungXaHang,
                        TongTgTron = tongTgTron,
                        ApSuat = TryGetDouble(row["ApSuat"]),
                        NhietDoMT = TryGetDouble(row["NhietDoMoiTruong"]),
                        DoAmMT = TryGetDouble(row["DoAmMoiTruong"]),
                        NhietNapBon = TryGetDouble(row["NhietDoBonTronTren"]),
                        NhietGiuaBon = TryGetDouble(row["NhietDoBonTronGiua"]),
                        NhietDayBon = TryGetDouble(row["NhietDoBonTronDuoi"])
                    });
                }
            }

            byte[] fileBytes = ExportUtility.ExportToCsv(list);
            return File(fileBytes, "text/csv", $"Report_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        public ActionResult Overview()
        {
            ViewBag.ButtonHome = "active";
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else
            {
                ViewBag.DisplayAdmin = "none";
            }
            return View();
        }

        public ActionResult OverviewSignage()
        {
            ViewBag.ButtonHome = "active";
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");
            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else
            {
                ViewBag.DisplayAdmin = "none";
            }
            return View("OverviewSignage", "~/Views/Shared/_LayoutSignageAdmin.cshtml");
        }

        public ActionResult Alarm()
        {
            ViewBag.ButtonAlarm = "active";
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else
            {
                ViewBag.DisplayAdmin = "none";
            }
            return View();
        }
        public ActionResult Event()
        {
            ViewBag.ButtonEvent = "active";
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else
            {
                ViewBag.DisplayAdmin = "none";
            }
            return View();
        }

        public ActionResult UserSetting()
        {
            ViewBag.ButtonUser = "active";
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                ViewBag.DisplayAdmin = "block";
            }
            else
            {
                ViewBag.DisplayAdmin = "block";
            }

            return View();
        }

        private static double TryGetDouble(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            double res;
            if (double.TryParse(value.ToString(), out res)) return res;
            return 0;
        }
    }

    public class ReportExportDto
    {
        public string Ngay { get; set; }
        public string Gio { get; set; }
        public int QuyTrinh { get; set; }
        public int CongDoan { get; set; }
        public double TgCapLieu { get; set; }
        public double TgTron1 { get; set; }
        public double TgXaDay { get; set; }
        public double TgRungXaDay { get; set; }
        public double TgHutXaDay { get; set; }
        public double TgTron2 { get; set; }
        public double TgXaHang { get; set; }
        public double TgRungXaHang { get; set; }
        public double TongTgTron { get; set; }
        public double ApSuat { get; set; }
        public double NhietDoMT { get; set; }
        public double DoAmMT { get; set; }
        public double NhietNapBon { get; set; }
        public double NhietGiuaBon { get; set; }
        public double NhietDayBon { get; set; }
    }
}