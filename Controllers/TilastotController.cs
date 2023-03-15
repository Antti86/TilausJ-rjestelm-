using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TilausJärjestelmä.Models;
using TilausJärjestelmä.Models.Tilastot;

namespace TilausJärjestelmä.Controllers
{
    [AuthFilter(RequiredLevel = 1)]
    public class TilastotController : Controller
    {
        // GET: Tilastot
        public ActionResult Index()
        {
            return View(new TilastoKokoelmaVM());
        }

    }
}