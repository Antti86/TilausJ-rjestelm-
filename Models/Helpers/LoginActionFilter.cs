using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Routing;
using TilausJärjestelmä.Controllers;

namespace TilausJärjestelmä.Models
{
    public class LoginActionFilter : ActionFilterAttribute  //Luokka varmistaa että ilman kirjautumista ei voi käyttää ohjelmaa
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (HttpContext.Current.Session["UserName"] == null)
            {
                var controller = (BaseController)filterContext.Controller;
                filterContext.Result = controller.RedirectToAction("index", "home");
            }

        }
    }
}

