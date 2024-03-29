﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TodoApplication.DAL;
using WebMatrix.WebData;

namespace TodoApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

         //  if (!WebSecurity.Initialized)
             //   WebSecurity.InitializeDatabaseConnection("DefaultConnection",
              //   "AspNetUsers", "Id", "UserName", autoCreateTables: true);

        }
    }
  

}
