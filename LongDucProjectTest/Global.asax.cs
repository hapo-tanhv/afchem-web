using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LongDucProjectTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Start backend timer accumulator service
            LongDucProjectTest.Service.BackendTimerAccumulator.Instance.Start();
        }

        protected void Application_End()
        {
            // Stop backend timer accumulator service
            LongDucProjectTest.Service.BackendTimerAccumulator.Instance.Stop();
        }
    }
}
