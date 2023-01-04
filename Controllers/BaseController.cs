using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TilausJärjestelmä.Models;

namespace TilausJärjestelmä.Controllers
{
    public class BaseController : Controller    //Kaikki Controllerit perii tästä!!
    {
        public new RedirectToRouteResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }

        protected TilausDBEntities db = new TilausDBEntities();
    }
}