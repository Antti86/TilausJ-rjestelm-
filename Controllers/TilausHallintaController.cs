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
    
    [AuthFilter(RequiredLevel = 1)]
    public class TilausHallintaController : BaseController
    {
        // GET: TilausHallinta
        public ActionResult Index()
        {
            return View(TilausViewModel.GetViewModelList());
        }

        public ActionResult Tilausrivit(int id)
        {
            return View(TilausRiviVM.GetViewModelList(id));
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tilaukset = TilausViewModel.GetViewModelList();
            var t = tilaukset.Find(x => x.TilausID== id);
            if (t == null)
            {
                return HttpNotFound();
            }
            var rivit = TilausRiviVM.GetViewModelList((int)id);
            ViewData["Rivit"] = rivit;
            return View(t);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Haetaan ensin kaikki tilausrivit mitkä on poistettavan tilauksen alla
            var tilausrivit = from i in db.Tilausrivit
                              where i.TilausID == id
                              select i;
            //Poistetaan ensin kaikki rivit ennen kun voidaan poistaa tilaus
            foreach (var i in tilausrivit)
            {
                db.Tilausrivit.Remove(i);
            }
            //Haetaan tilaus ja poistetaan se
            Tilaukset tilaus = db.Tilaukset.Find(id);
            db.Tilaukset.Remove(tilaus);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult TilauksenLuonti(TilausLuontiVM model)   //Tilauksen luonti sivulla kaikki data liikkuu tämän kautta
        {
            //Huom. Asiakashaku toiminto ei toimi kunnolla jos on useampi saman niminen asiakas
            //Etsintä tehdään nimellä eikä asiakas_id:llä!! Sama myös Tuotehaulla!!

            TilausLuontiVM.HaeAsiakkaatJaTuotteet(model, db);

            //Muutetaan generic listaks, jotta saadaan find metohdi käyttöön
            List<Asiakkaat> asiakaslista = model.asiakkaat.ToList();
            List<Tuotteet> tuotelista = model.tuotteet.ToList();

            //Haetaan tiedot jos ne on client puolelta valittu
            Tuotteet tuote = tuotelista.Find(a => a.Nimi == model.valittuTuote);
            Asiakkaat asia = asiakaslista.Find(a => a.Nimi == model.valittuAsiakas);

            //Asiakas Tiedot!
            //Jos on valittu asiakas client puolelta niin haetaan oikeat tiedot
            if (asia != null)
            {
                model.AsiakasID = asia.AsiakasID;
                model.Toimitusosoite = asia.Osoite.ToString();
                model.TilausID = TilausLuontiVM.HaeTilausnumero(db);
                model.Postitoimipaikka = asia.Postitoimipaikat.Postitoimipaikka.ToString();
                model.Postinumero = asia.Postinumero.ToString();
            }
            else
            {
                model.TyhjennäAsiakasTiedot();
            }


            //Tuote ja Tilausrivi tiedot
            //Jos on valittu tuote client puolelta niin haetaan oikeat tiedot
            if (tuote != null)
            {
                model.Tuotenmr = tuote.TuoteID;
                model.Hinta = tuote.Ahinta;
                decimal? summa = model.Hinta * model.maara;
                model.Rivisumma = summa;
            }
            else
            {
                model.TyhjennäTuoteTeidot();
            }

            //Liitä kalenteri ominaisuus jossain vaiheessa?
            //Päivämäärä hallinta
            model.ToimitusPvmLista = new List<string>();
            model.Tilauspvm = DateTime.Now.Date;
            if (model.Toimituspvm != null)
            {
                model.ToimitusAika = model.Toimituspvm.Value.Day - model.Tilauspvm.Value.Day;
            }
            //Luodaan Cache objekti, tällä pidetään kirjaa lisätyistä tilausriveistä
            Cacher listCache = new Cacher(); 
            if (model.uusirivi && model.maara > 0 && tuote != null && asia != null)
            {
                int riviId = TilausLuontiVM.HaeTilausRivi(db) + listCache.CacheLista.Count();
                listCache.CacheLista.Add(new TilausRiviVM(model.valittuTuote, model.maara, model.Rivisumma, riviId, model.Tuotenmr));
                model.uusirivi = false;
            }
            if (model.rivinpoisto)
            {
                listCache.PoistaRivi((int)model.PoistoIndexi);
                model.rivinpoisto = false;
            }
            if (model.tyhjennäKaikkiRivit)
            {
                listCache.TyhjennäLista();
                model.tyhjennäKaikkiRivit = false;
            }
            //Liitetään aktiiviset rivit listaan web memory cachen tiedot
            model.aktiivisetRivit = listCache.CacheLista;


            model.KokonaisSumma = model.aktiivisetRivit.Sum(x => x.Ahinta);

            //Katsotaan onko action linjastosta tullut error flag
            if (model.error)
            {
                ModelState.AddModelError("Error", "Tilauksen tallentaminen ei onnistunut, ei tarpeeksi tietoja");
                model.error = false;
            }

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
        [HttpPost]
        public ActionResult MaaranMuutos(TilausLuontiVM model, string maarax) //Jos muutetaan käsin rivin tuotemäärää ni tän actionin kautta
        {
            if (Checker.IsNumeric(maarax))
            {
                model.maara = int.Parse(maarax);
            }
            else
            {
                model.maara = 0;
            }
            return RedirectToAction("TilauksenLuonti", model);
        }
        public ActionResult LisaaRivi(TilausLuontiVM model) //Hoitaa lisää rivi buttonin toiminnon
        {
            model.uusirivi = true;
            return RedirectToAction("TilauksenLuonti", model);
        }

        public ActionResult PoistaRivi(TilausLuontiVM model) //Hoitaa poistarivi buttonin toiminnon
        {
            model.rivinpoisto = true;
            return RedirectToAction("TilauksenLuonti", model);
        }

        public ActionResult TyhjennäRivit(TilausLuontiVM model) //Hoitaa tyhjennä rivit buttonin toiminnon
        {
            model.tyhjennäKaikkiRivit = true;
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
        [HttpPost]
        public ActionResult ToimitusPvmValinta(TilausLuontiVM model) //Kiertää tätä kautta Toimituspäivä dropdowboxin valinnan yhteydessä
        {
            model.Toimituspvm = DateTime.Parse(model.ToimitusPaivastring);
            return RedirectToAction("TilauksenLuonti", model);
        }
        public ActionResult TallennaTilaus(TilausLuontiVM model)
        {
            Cacher listCache = new Cacher();
            if (String.IsNullOrEmpty(model.valittuAsiakas) || String.IsNullOrEmpty(model.valittuTuote) || listCache.CacheLista.Count() == 0
                || model.Toimituspvm == null)
            {
                //Heittää takaisin TilauksenLuonti actioniin ja merkkaa errorin
                model.error = true;
                return RedirectToAction("TilauksenLuonti", model);
            }
            Tilaukset uusitilaus = new Tilaukset()
            {
                TilausID = (int)model.TilausID,
                AsiakasID = model.AsiakasID,
                Toimitusosoite = model.Toimitusosoite,
                Postinumero = model.Postinumero,
                Tilauspvm = model.Tilauspvm,
                Toimituspvm = model.Toimituspvm
            };

            db.Tilaukset.Add(uusitilaus);

            foreach (var i in listCache.CacheLista)
            {
                Tilausrivit rivi = new Tilausrivit()
                {
                    TilausriviID = (int)i.TilausriviID,
                    TilausID = model.TilausID,
                    TuoteID = i.TuoteID,
                    Maara = i.maara,
                    Ahinta = i.Ahinta
                };
                db.Tilausrivit.Add(rivi);
            }
            db.SaveChanges();
            listCache.TyhjennäLista();
            return RedirectToAction("Index");
        }

        public ActionResult TyhjennäTiedot(TilausLuontiVM model)
        {
            Cacher listCache = new Cacher();
            listCache.TyhjennäLista();
            model.valittuAsiakas = null;
            model.valittuTuote = null;
            model.Toimituspvm = null;
            model.ToimitusPaivastring = null;
            return RedirectToAction("TilauksenLuonti", model);
        }

        public ActionResult Takaisin(TilausLuontiVM model)
        {
            Cacher listCache = new Cacher();
            listCache.TyhjennäLista();
            model.valittuAsiakas = null;
            model.valittuTuote = null;
            model.Toimituspvm = null;
            model.ToimitusPaivastring = null;
            return RedirectToAction("Index");
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