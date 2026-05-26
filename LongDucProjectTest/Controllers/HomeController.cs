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
            Session["UserName"] = param.UserName;
            switch (role)
            {
                case Role.Admin:
                    return RedirectToAction("Overview", "Home");
                case Role.Operator:
                    return RedirectToAction("Overview", "Home");
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

            if ((int)Session["Role"] == (int)Role.Admin)
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

        [HttpGet]
        public JsonResult GetBatches(string starttime, string endtime)
        {
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };

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

                string query = $"SELECT id, name, status FROM batches WHERE start_time >= '{startDate.ToString("yyyy-MM-dd HH:mm:ss")}' AND start_time <= '{endDate.ToString("yyyy-MM-dd HH:mm:ss")}' ORDER BY id DESC";
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

        [HttpPost]
        public JsonResult GetReportData(string starttime, string endtime, string batchId, int draw, int start, int length, bool? isInitialLoad)
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
                    var dtCompleted = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Completed' ORDER BY id DESC LIMIT 1");
                    if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                    {
                        resolvedBatchId = Convert.ToInt32(dtCompleted.Rows[0]["id"]);
                    }
                    else
                    {
                        var dtActive = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Active' LIMIT 1");
                        if (dtActive != null && dtActive.Rows.Count > 0)
                        {
                            resolvedBatchId = Convert.ToInt32(dtActive.Rows[0]["id"]);
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

                string baseQuery = "FROM alarmreport a INNER JOIN batches b ON a.batchId = b.id WHERE 1=1";
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

        [HttpGet]
        public FileResult ExportReportExcel(string starttime, string endtime, string batchId, string searchValue)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

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
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                throw new HttpException(403, "Bạn không có quyền thực hiện hành động này.");
            }

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

            if ((int)Session["Role"] == (int)Role.Admin)
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
            if ((int)Session["Role"] == (int)Role.Admin)
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

            if ((int)Session["Role"] == (int)Role.Admin)
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

            if ((int)Session["Role"] == (int)Role.Admin)
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

            if ((int)Session["Role"] != (int)Role.Admin)
            {
                return RedirectToAction("Overview", "Home");
            }

            ViewBag.DisplayAdmin = "block";
            return View();
        }

        private static double TryGetDouble(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            double res;
            if (double.TryParse(value.ToString(), out res)) return res;
            return 0;
        }
        [HttpGet]
        public JsonResult GetAccounts()
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                return Json(new { Status = false, Message = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

            var list = new List<object>();
            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };
                var dt = connector.ExecuteQuery("SELECT ID, UserName, Role FROM account");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        list.Add(new
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            UserName = row["UserName"].ToString(),
                            Role = Convert.ToInt32(row["Role"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = true, Data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateAccount(CreateAccountParam param)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                return Json(new { Status = false, Message = "Unauthorized" });
            }

            if (string.IsNullOrEmpty(param.UserName) || string.IsNullOrEmpty(param.Password))
            {
                return Json(new { Status = false, Message = "UserName and Password cannot be empty!" });
            }

            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };
                
                // Check duplicate
                var duplicateCheck = connector.ExecuteScalarQuery($"SELECT COUNT(*) FROM account WHERE UserName = '{param.UserName}'");
                if (Convert.ToInt32(duplicateCheck) > 0)
                {
                    return Json(new { Status = false, Message = "Tên đăng nhập đã tồn tại!" });
                }

                // Insert
                string query = $"INSERT INTO account (UserName, Password, Role) VALUES ('{param.UserName}', '{param.Password}', {param.Role})";
                int result = connector.ExecuteNonQuery(query);
                if (result > 0)
                {
                    return Json(new { Status = true, Message = "Tạo tài khoản thành công!" });
                }
                return Json(new { Status = false, Message = "Không thể tạo tài khoản. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ResetUserPassword(ResetUserPasswordParam param)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                return Json(new { Status = false, Message = "Unauthorized" });
            }

            if (param.NewPassword != param.ConfirmPassword)
            {
                return Json(new { Status = false, Message = "Mật khẩu xác nhận không khớp!" });
            }

            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };
                
                string query = $"UPDATE account SET Password = '{param.NewPassword}' WHERE UserName = '{param.UserName}'";
                int result = connector.ExecuteNonQuery(query);
                if (result > 0)
                {
                    return Json(new { Status = true, Message = "Thay đổi mật khẩu thành công!" });
                }
                return Json(new { Status = false, Message = "Không thể cập nhật mật khẩu. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateUserRole(UpdateRoleParam param)
        {
            if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
            {
                return Json(new { Status = false, Message = "Unauthorized" });
            }

            try
            {
                var connector = new MySQLConnect()
                {
                    ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
                };

                // Edge case validation: Minimum 1 Admin rule
                if (param.NewRole == (int)Role.Operator)
                {
                    var currentRoleCheck = connector.ExecuteScalarQuery($"SELECT Role FROM account WHERE UserName = '{param.UserName}'");
                    if (currentRoleCheck != null && Convert.ToInt32(currentRoleCheck) == (int)Role.Admin)
                    {
                        var adminCount = connector.ExecuteScalarQuery($"SELECT COUNT(*) FROM account WHERE Role = {(int)Role.Admin}");
                        if (Convert.ToInt32(adminCount) <= 1)
                        {
                            return Json(new { Status = false, Message = "Hệ thống phải có ít nhất một tài khoản Admin!" });
                        }
                    }
                }

                string query = $"UPDATE account SET Role = {param.NewRole} WHERE UserName = '{param.UserName}'";
                int result = connector.ExecuteNonQuery(query);
                if (result > 0)
                {
                    return Json(new { Status = true, Message = "Cập nhật quyền thành công!" });
                }
                return Json(new { Status = false, Message = "Không thể cập nhật quyền. Vui lòng thử lại." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

    }


    public class CreateAccountParam
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }

    public class ResetUserPasswordParam
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UpdateRoleParam
    {
        public string UserName { get; set; }
        public int NewRole { get; set; }
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