using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
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
        public ActionResult TilauksenLuonti(TilausLuontiVM model)
        {
            //Huom. Asiakashaku toiminto ei toimi kunnolla jos on useampi saman niminen asiakas
            //Etsintä tehdään nimellä eikä asiakas_id:llä!! Sama myös Tuotehaulla!!

            model.asiakkaat = from a in db.Asiakkaat
                              select a;
            model.asiakkaat = model.asiakkaat.OrderBy(t => t.Nimi);

            model.tuotteet = from t in db.Tuotteet
                             select t;

            model.postitoimipaikat = from p in db.Postitoimipaikat
                                     select p;

            //Muutetaangeneric listaks, jotta saadaan find metohdi käyttöön
            List<Asiakkaat> asiakaslista = model.asiakkaat.ToList();
            List<Tuotteet> tuotelista = model.tuotteet.ToList();

            //Haetaan tiedot jos ne on client puolelta valittu
            Tuotteet tuote = tuotelista.Find(a => a.Nimi == model.selectedTuote);
            Asiakkaat asia = asiakaslista.Find(a => a.Nimi == model.selectedAsiakas);

            //Tilaus viewbagit... Muuta viewbagit modelin ominaisuuksi
            //Jos on valittu asiakas client puolelta niin haetaan oikeat tiedot
            if (asia != null)
            {
                ViewBag.AsiakasId = asia.AsiakasID.ToString();
                ViewBag.Toimitusosoite = asia.Osoite.ToString();
                ViewBag.Tilausnumero = TilausLuontiVM.HaeTilausnumero(db).ToString();
                ViewBag.Postitoimipaikka = asia.Postitoimipaikat.Postitoimipaikka.ToString();
                ViewBag.Postinumero = asia.Postinumero.ToString();
                DateTime time = DateTime.Now;
                ViewBag.TilausPvm = time;
                ViewBag.ToimitusPvm = time.AddDays(3);
            }


            //Tilausrivi viewbagit.. Muuta viewbagit modelin ominaisuuksi
            //Jos on valittu tuote client puolelta niin haetaan oikeat tiedot
            if (tuote != null)
            {
                ViewBag.Tuotenmr = tuote.TuoteID.ToString();
                model.hinta = (decimal)tuote.Ahinta;
                var summa = tuote.Ahinta * model.maara;
                ViewBag.Summa = summa.ToString();
            }
            //Luodaan Cache objekti, tällä pidetään kirjaa lisätyistä tilausriveistä
            var listCache = new Cacher(); 
            if (model.uusirivi == true && model.maara > 0 && tuote != null && asia != null)
            {
                listCache.CacheLista.Add(new TilausRiviVM(model.selectedTuote, model.maara, model.hinta, TilausLuontiVM.HaeTilausRivi(db)));
                
            }
            //Liitetään aktiiviset rivit listaan web memory cachen tiedot
            model.aktiivisetRivit = listCache.CacheLista;
            model.uusirivi = false;

            return View(model);
        }

        public ActionResult Plus(TilausLuontiVM model) //Jos painetaan Plus näppäintä niin dataflow tän actionin kautta
        {
            model.maara += 1;
            return RedirectToAction("TilauksenLuonti", model);
        }
        public ActionResult Minus(TilausLuontiVM model) //Jos painetaan Miinus näppäintä niin dataflow tän actionin kautta
        {
            if (model.maara > 0)
            {
                model.maara -= 1;
            }
            return RedirectToAction("TilauksenLuonti", model);
        }

        public ActionResult LisaaRivi(TilausLuontiVM model) //Hoitaa lisää rivi buttonin toiminnon
        {
            model.uusirivi = true;
            return RedirectToAction("TilauksenLuonti", model);
        }

        [HttpPost]
        public ActionResult AsiakaValinta(TilausLuontiVM model) //Kiertää tätä kautta asiakkaan dropdowboxin valinnan yhteydessä
        {

            return RedirectToAction("TilauksenLuonti", model);
        }
        [HttpPost]
        public ActionResult TuoteValinta(TilausLuontiVM model) //Kiertää tätä kautta tuotteen dropdowboxin valinnan yhteydessä
        {
            return RedirectToAction("TilauksenLuonti", model);
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