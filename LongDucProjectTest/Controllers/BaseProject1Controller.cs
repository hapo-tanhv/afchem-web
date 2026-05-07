using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LongDucProjectTest.Controllers
{
    public class BaseProject1Controller : Controller
    {
        // GET: BaseProject1
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (string)Session["user"];
            if (session == "1")
            {
                TempData["NotificationMessage"] = "Notification message";
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}