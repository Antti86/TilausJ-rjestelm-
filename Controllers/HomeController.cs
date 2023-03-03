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
            if (Session["UserName"] != null) //Menee tähän kun kirjaudutaan ulos
            {
                Cacher listCache = new Cacher();
                listCache.TyhjennäLista();
                Session["Level"] = null;
                Session.Abandon();
            }
            Session["LogStatus"] = "Kirjaudu sisään";
            return View();
        }
        [HttpPost]
        public ActionResult Authorize(LoginsVM model)
        {

            //Haetaan käyttäjän/Loginin tiedot annetuilla tunnustiedoilla tietokannasta LINQ -kyselyllä
            var LoggedUser = db.Logins.SingleOrDefault(x => x.UserName == model.UserName && x.PassWord == model.PassWord);
            if (LoggedUser != null)
            {
                string statustext = "Kirjaudu ulos (" + LoggedUser.UserName + ")";
                Session["LogStatus"] = statustext;
                Session["UserName"] = LoggedUser.UserName;
                Session["Level"] = Convert.ToString(LoggedUser.Level);

                if (LoggedUser.PassWord == "1234")
                {
                    return RedirectToAction("SalasananVaihto", "KayttajienHallintas"); //Tähän jatko viesti
                }
                return RedirectToAction("Index", "TilausHallinta");
            }
            else
            {
                Session["LogStatus"] = "Kirjaudu sisään";
                model.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Index", model);
            }
        }

        public ActionResult ErrorViesti()
        {
            Session["Denied"] = "True"; //Laitetaan flag layout sivulle joka kutsuu partial viewsiä sen mukaan
            return Redirect(Request.UrlReferrer.ToString()); //Hakee ja palauttaa edellisen sivun
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