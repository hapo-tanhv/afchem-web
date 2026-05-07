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


namespace LongDucProject.Controllers
{
    public class Project4Controller : Controller
    {
        // GET: Project4
        public ActionResult Inverter1()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

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
                return View("Inverter1", "~/Views/Shared/_LayoutProject4.cshtml");
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
                return View("Inverter2", "~/Views/Shared/_LayoutProject4.cshtml");
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
                return View("Inverter3", "~/Views/Shared/_LayoutProject4.cshtml");
            }
        }
        public ActionResult Inverter4()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Inverter4", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Inverter4", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Inverter4", "~/Views/Shared/_LayoutProject4.cshtml");
            }
        }
        public ActionResult Inverter5()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Inverter5", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Inverter5", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Inverter5", "~/Views/Shared/_LayoutProject4.cshtml");
            }
        }
        public ActionResult Inverter6()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Inverter6", "~/Views/Shared/_LayoutMain.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Inverter6", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Inverter6", "~/Views/Shared/_LayoutProject4.cshtml");
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
                return View("Info", "~/Views/Shared/_LayoutProject4.cshtml");
            }
            //return View("Info", "~/Views/Shared/_LayoutProject4.cshtml");
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
                return View("Alarm", "~/Views/Shared/_LayoutProject4.cshtml");
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
                return View("Event", "~/Views/Shared/_LayoutProject4.cshtml");
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
                return View("Weather", "~/Views/Shared/_LayoutProject4.cshtml");
            }

            //return View("Weather", "~/Views/Shared/_LayoutProject4.cshtml");
        }
        public ActionResult Signage4()
        {
            if (Session["Role"] is null) return RedirectToAction("Login", "Home");

            if ((int)Session["Role"] == (int)Role.Admin || (int)Session["Role"] == (int)Role.SolEnergy || (int)Session["Role"] == (int)Role.JGC || (int)Session["Role"] == (int)Role.Hino)
            {
                return View("Signage4", "~/Views/Shared/_LayoutSignageAdmin.cshtml");
            }
            else if ((int)Session["Role"] == (int)Role.LDIP)
            {
                return View("Signage4", "~/Views/Shared/_LayoutLDIP.cshtml");
            }
            else
            {
                return View("Signage4", "~/Views/Shared/_LayoutProject4.cshtml");
            }
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
                return View("UserSettings", "~/Views/Shared/_LayoutProject4.cshtml");
            }
        }
    }
}