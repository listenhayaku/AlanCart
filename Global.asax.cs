using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

//¬°¤Fresx¥Î
using System.Globalization;
using System.Threading;

namespace AlanCart
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-TW");
            //Thread.CurrentThread.CurrentUICulture= new CultureInfo("zh-TW");

            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //Thread.CurrentThread.CurrentUICulture= new CultureInfo("en-US");
        }

        protected void Application_BeginRequest()
        {

        }
    }
}
