using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TilausJärjestelmä.Models;

namespace TilausJärjestelmä.Controllers
{

    public class HomeController : BaseController
    {

        
        public ActionResult Index()
        {
            if (Session["UserName"] != null)
            {
                Session.Abandon();
            }
            Session["LogStatus"] = "Kirjaudu sisään";
            return View();
        }
        [HttpPost]
        public ActionResult Authorize(Logins model)
        {

            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Logins.SingleOrDefault(x => x.UserName == model.UserName && x.PassWord == model.PassWord);
            if (LoggedUser != null)
            {
                string statustext = "Kirjaudu ulos (" + model.UserName + ")";
                Session["LogStatus"] = statustext;
                Session["UserName"] = LoggedUser.UserName;
                return RedirectToAction("Index", "TilausHallinta");
            }
            else
            {
                Session["LogStatus"] = "Kirjaudu sisään";
                model.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Index", model);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            Session["LogStatus"] = "Kirjaudu sisään";
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}