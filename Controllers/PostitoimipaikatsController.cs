using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TilausJärjestelmä.Models;

namespace TilausJärjestelmä.Controllers
{
    //[LoginActionFilter]
    [AuthFilter]
    public class PostitoimipaikatsController : BaseController
    {

        
        // GET: Postitoimipaikats
        public ActionResult Index(string sortOrder, string searchString, int page = 1)
        {
            var postipaikat = from a in db.Postitoimipaikat     //Hakee kaikki postitoimipaikat tietokannasta
                            select a;
            ViewBag.AllItems = postipaikat.Count().ToString();

            //ETSINTÄ RUTIINI
            ViewBag.Search = searchString;  //Päivittää etsintäboxin tekstin
            if (!String.IsNullOrEmpty(searchString)) 
            {
                //Jos etsintä teksti ei ole tyhjä niin suodattaa tietokannasta tulokset hakukriteereillä
                postipaikat = postipaikat.Where(s => s.Postitoimipaikka.Contains(searchString) || s.Postinumero.Contains(searchString));
            }
            ViewBag.Sort = sortOrder;   //Tallentaa järjestely perusteet
            int test = postipaikat.Count();            
                        //JÄRJESTELY RUTIINI
            switch (sortOrder)  //Järjestellään tietokanta näkymä sen mukaan mikä on järjestely peruste
                //Oletuksena ja ohjelman käynnistyessä järjestely peruste on "A-Ö" nouseva
            {
                case "A-Ö":
                    postipaikat = postipaikat.OrderBy(t => t.Postitoimipaikka);
                    break;
                case "Ö-A":
                    postipaikat = postipaikat.OrderByDescending(t => t.Postitoimipaikka);
                    break;
                case "Postinumero laskeva":
                    postipaikat = postipaikat.OrderByDescending(t => t.Postinumero);
                    break;
                case "Postinumero nouseva":
                    postipaikat = postipaikat.OrderBy(t => t.Postinumero);
                    break;
                default:
                    postipaikat = postipaikat.OrderBy(t => t.Postitoimipaikka);
                    ViewBag.Sort = "A-Ö";
                    break;
            }
            int test2 = postipaikat.Count();
            //SIVUTTAMIS RUTIINIT

            const int pElements = 30;       //Määrittää montako elementtiä yhdellä sivulla
            int begin = pElements * page - pElements;   //Laskee alku indexin mistä aloitetaan lisäämään elementtejä sivulle
            int numOfPages = postipaikat.Count() / pElements + 1;   //Laskee montako sivua on, ei ole testattu jos numero on jaollinen 30
            ViewBag.nPages = numOfPages;    
            ViewBag.CurrentPage = page;
            postipaikat = postipaikat.Skip(begin).Take(pElements);  //Suodattaa sivunäkymän sivu numeron ja elementti määrän mukaan
            int test3 = postipaikat.Count();
            return View(postipaikat);
        }

        // GET: Postitoimipaikats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Postitoimipaikats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Postinumero,Postitoimipaikka")] Postitoimipaikat postitoimipaikat)
        {
            if (ModelState.IsValid)
            {
                var check = from i in db.Postitoimipaikat
                            select i.Postinumero;
                if (check.Contains(postitoimipaikat.Postinumero))
                {
                    ModelState.AddModelError("postinumero", "Postinumero on jo käytössä");
                    return View(postitoimipaikat);
                }
                if (!Checker.IsNumeric(postitoimipaikat.Postinumero))
                {
                    ModelState.AddModelError("postinumero", "Postinumero pitää olla numeroita");
                    return View(postitoimipaikat);
                }
                db.Postitoimipaikat.Add(postitoimipaikat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(postitoimipaikat);
        }

        // GET: Postitoimipaikats/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Postitoimipaikat postitoimipaikat = db.Postitoimipaikat.Find(id);
            if (postitoimipaikat == null)
            {
                return HttpNotFound();
            }
            return View(postitoimipaikat);
        }

        // POST: Postitoimipaikats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Postinumero,Postitoimipaikka")] Postitoimipaikat postitoimipaikat)
        {
            if (ModelState.IsValid)
            {
                var check = from i in db.Postitoimipaikat
                            select i.Postinumero;
                if (check.Contains(postitoimipaikat.Postinumero))
                {
                    ModelState.AddModelError("postinumero", "Postinumero on jo käytössä");
                    return View(postitoimipaikat);
                }
                if (!Checker.IsNumeric(postitoimipaikat.Postinumero))
                {
                    ModelState.AddModelError("postinumero", "Postinumero pitää olla numeroita");
                    return View(postitoimipaikat);
                }
                db.Entry(postitoimipaikat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(postitoimipaikat);
        }

        // GET: Postitoimipaikats/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Postitoimipaikat postitoimipaikat = db.Postitoimipaikat.Find(id);
            if (postitoimipaikat == null)
            {
                return HttpNotFound();
            }
            return View(postitoimipaikat);
        }

        // POST: Postitoimipaikats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Postitoimipaikat postitoimipaikat = db.Postitoimipaikat.Find(id);
            db.Postitoimipaikat.Remove(postitoimipaikat);
            db.SaveChanges();
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
