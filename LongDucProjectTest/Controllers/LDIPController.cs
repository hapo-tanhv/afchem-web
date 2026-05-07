using Hino.Getdata.Common;
using LongDucProjectTest.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LongDucProjectTest.Controllers
{

    public class LDIPController : Controller
    {
        public ActionResult Home()
        {
            return View("Home", "~/Views/Shared/_LayoutLDIP.cshtml");
        }
        public ActionResult Overview()
        {
            return View("Overview", "~/Views/Shared/_LayoutLDIP.cshtml");
        }
        public ActionResult Alarm()
        {
            return View("Alarm", "~/Views/Shared/_LayoutLDIP.cshtml");
        }
        public ActionResult Event()
        {
            return View("Event", "~/Views/Shared/_LayoutLDIP.cshtml");
        }
       
    }
}