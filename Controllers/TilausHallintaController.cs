using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TilausJärjestelmä.Models;

namespace TilausJärjestelmä.Controllers
{
    public class TilausHallintaController : Controller
    {
        private TilausDBEntities db = new TilausDBEntities();
        // GET: TilausHallinta
        public ActionResult Index()
        {
            return View(TilausViewModel.GetViewModelList());
        }

        public ActionResult Tilausrivit(int id)
        {
            return View(TilausRiviVM.GetViewModelList(id));
        }

        public ActionResult TilauksenLuonti()
        {
            TilausLuontiVM model = new TilausLuontiVM();
            model.asiakkaat = from a in db.Asiakkaat
                              select a;
            model.asiakkaat = model.asiakkaat.OrderBy(t => t.Nimi);

            model.tuotteet = from t in db.Tuotteet
                             select t;

            model.postitoimipaikat = from p in db.Postitoimipaikat
                                     select p;
            return View(model);
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