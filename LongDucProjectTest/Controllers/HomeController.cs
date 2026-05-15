using Hino.Getdata.Common;
using LongDucProjectTest.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
            // TODO: Replace with actual query to `alarmreport` or respective report table
            var Alarm = new Hino.Getdata.Common.AlarmCommon();
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
        public FileResult ExportReportExcel(string starttime, string endtime, string batchId, string searchValue)
        {
            var Alarm = new Hino.Getdata.Common.AlarmCommon();
            var resultList = Alarm.GetAlarmLog(starttime, endtime);
            var list = resultList != null ? resultList.ToList() : new List<Hino.Parameter.Common.AlarmParameter>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.TagNo != null && x.TagNo.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToExcel(list, "Report");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpGet]
        public FileResult ExportReportCsv(string starttime, string endtime, string batchId, string searchValue)
        {
            var Alarm = new Hino.Getdata.Common.AlarmCommon();
            var resultList = Alarm.GetAlarmLog(starttime, endtime);
            var list = resultList != null ? resultList.ToList() : new List<Hino.Parameter.Common.AlarmParameter>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => 
                    (x.Description != null && x.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) || 
                    (x.TagNo != null && x.TagNo.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            byte[] fileBytes = LongDucProjectTest.Service.ExportUtility.ExportToCsv(list);
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
    }
}