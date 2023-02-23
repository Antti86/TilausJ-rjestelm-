using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace TilausJärjestelmä.Models
{
    public class AuthFilter : ActionFilterAttribute,
    IAuthenticationFilter   //Customoitu AuthenticationFilter luokka mikä valvoo ja hoitaa että ennen jokaista actionia kirjautuminen on
                            // tehty ja että kirjautumis taso on vaadittu
    {
        public int VaadittuLvl { get; set; } //Tämä asetetaan ennen controllerin actionia
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //Katsotaan onko session tiedoissa kirjautuminen, jos ei niin annetaan resultiks "HttpUnauthorizedResult"
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["UserName"])))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //Tarkistetaan resultti, jos ei ollut hyväksytty niin ohjataan Home index actioniin
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                     { "controller", "Home" },
                     { "action", "Index" }
                });
            }
            else
            {
                //Tarkistetaan oliko yritettyyn actioiniin vaadittu kirjautumis taso,
                //jos ei niin ohjataan Home ErrorViesti actioniin, missä ohjataan takaisin edelliselle sivulle ja annetaan virhe viesti

                string Lvl = Convert.ToString(filterContext.HttpContext.Session["Level"]);
                int Level = 0;
                if (Checker.IsNumeric(Lvl))
                {
                    Level = int.Parse(Lvl);
                }

                if (Level < VaadittuLvl)
                {

                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Home" },
                        { "action", "ErrorViesti" }
                    });
                }
            }


        }
    }
}