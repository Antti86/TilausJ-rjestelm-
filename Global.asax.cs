﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TilausJärjestelmä
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fi-FI");
            Application_BeginRequest();
        }
        protected void Application_BeginRequest()
        {
            var currentCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = ".";
            currentCulture.NumberFormat.NumberGroupSeparator = " ";
            currentCulture.NumberFormat.CurrencyDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }
}
