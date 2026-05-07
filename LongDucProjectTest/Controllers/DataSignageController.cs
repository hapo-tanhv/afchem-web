using Hino.Getdata.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LongDucProjectTest.Controllers
{
    public class DataSignageController : Controller
    {
        // GET: DataSignage
        public ActionResult Index()
        {
            return View();
        }
        //Phần data để vẽ biểu đồ total cho màn hình signage
        [HttpGet]
        public JsonResult GetProjectDataForSignage(string ProjectName, int timeUnit, string starttime, string endtime)
        {
            var Project_Energy = new ProjectDataEnergy();
            var list = Project_Energy.GetProjectSignageData(ProjectName, timeUnit, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Phần data để vẽ biểu đồ daily self consumption cho màn hình signage
        //[HttpGet]
        //public JsonResult GetProjectDailySelfConsumption(string ProjectName, int selectProc, string starttime, string endtime)
        //{
        //    var SelfDaily = new ProjectDataEnergy();
        //    var list = SelfDaily.GetProjectSelfConsumption(ProjectName, selectProc, starttime, endtime);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        //Phần data để vẽ biểu đồ Monthly self consumption cho màn hình signage
        [HttpGet]
        public JsonResult GetProjectMonthlySelfConsumption(string ProjectName, int selectProc, string starttime, string endtime)
        {
            var SelfMonthly = new ProjectDataEnergy();
            var list = SelfMonthly.GetProjectSelfConsumption(ProjectName, selectProc, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Phần data để vẽ biểu đồ Monthly self consumption cho màn hình signage
        [HttpGet]
        public JsonResult GetProjectYearlySelfConsumption(string ProjectName, int selectProc, string starttime, string endtime)
        {
            var SelfYearly = new ProjectDataEnergy();
            var list = SelfYearly.GetProjectSelfConsumption(ProjectName, selectProc, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //Phần data để vẽ biểu đồ Monthly self consumption cho màn hình signage
        [HttpGet]
        public JsonResult GetProjectTenYearSelfConsumption(string ProjectName, int selectProc, string starttime, string endtime)
        {
            var SelfYearly = new ProjectDataEnergy();
            var list = SelfYearly.GetProjectSelfConsumption(ProjectName, selectProc, starttime, endtime);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}