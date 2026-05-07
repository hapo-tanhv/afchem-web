using Hino.Getdata.Common;
using Hino.Getdata.Project;
using LongDucProjectTest.Controllers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;


namespace LongDucProjectTest.Controllers
{
    public class Project7Controller : Controller
    {
        // GET: Project7
        public ActionResult Inverter1()
        {
            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Inverter1", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Inverter1", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Inverter1", "~/Views/Shared/_LayoutProject7.cshtml");
            }
        }
        public ActionResult Inverter2()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Inverter2", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Inverter2", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Inverter2", "~/Views/Shared/_LayoutProject7.cshtml");
            }
        }
        public ActionResult Inverter3()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Inverter3", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Inverter3", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Inverter3", "~/Views/Shared/_LayoutProject7.cshtml");
            }
        }

        public ActionResult Info()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Info", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Info", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Info", "~/Views/Shared/_LayoutProject7.cshtml");
            }

        }
        public ActionResult Alarm()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Alarm", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Alarm", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Alarm", "~/Views/Shared/_LayoutProject7.cshtml");
            }
        }
        public ActionResult Event()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Event", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Event", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Event", "~/Views/Shared/_LayoutProject7.cshtml");
            }

        }
        public ActionResult Weather()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Weather", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Weather", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Weather", "~/Views/Shared/_LayoutProject7.cshtml");
            }
        }
        public ActionResult Signage7()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Signage7", "~/Views/Shared/_LayoutSignageAdmin.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Signage7", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Signage7", "~/Views/Shared/_LayoutProject7.cshtml");
            }

            //return View("Signage7", "~/Views/Shared/_LayoutProject7.cshtml");
        }

        public ActionResult UserSettings()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("UserSettings", "~/Views/Shared/_LayoutSignageAdmin.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("UserSettings", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("UserSettings", "~/Views/Shared/_LayoutProject7.cshtml");
            }
        }
    }
}