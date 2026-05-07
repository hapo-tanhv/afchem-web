using Hino.Parameter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LongDucProjectTest.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string sessionUser = (string)Session["user"];
            string sessionRole = (string)Session["role"];
            if (sessionUser != "1")
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Login", id = UrlParameter.Optional }));
            }  
            else
            {
                if(sessionRole!="Admin")
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "Sorry, your account does not have permission to access this website. Please use an Admin account to access.");
                }    
            }    
            base.OnActionExecuting(filterContext);
        }

    }
}