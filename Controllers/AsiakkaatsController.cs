using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TilausJärjestelmä.Models;

namespace TilausJärjestelmä.Controllers
{
    public class AsiakkaatsController : Controller
    {
        private TilausDBEntities db = new TilausDBEntities();

        // GET: Asiakkaats
        public ActionResult Index(string sortOrder, string searchString, int page = 1)
        {
            var asiakkaat = from a in db.Asiakkaat
                           select a;
            asiakkaat = db.Asiakkaat.Include(a => a.Postitoimipaikat);
            ViewBag.AllItems = asiakkaat.Count().ToString();
            ViewBag.Search = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                asiakkaat = asiakkaat.Where(s => s.Nimi.Contains(searchString));
            }
            ViewBag.Sort = sortOrder;
            switch (sortOrder)
            {
                case "A-Ö":
                    asiakkaat = asiakkaat.OrderBy(t => t.Nimi);
                    break;
                case "Ö-A":
                    asiakkaat = asiakkaat.OrderByDescending(t => t.Nimi);
                    break;
                case "Osoitteen mukaan":
                    asiakkaat = asiakkaat.OrderBy(t => t.Osoite);
                    break;
                case "Postitoimipaikan mukaan":
                    asiakkaat = asiakkaat.OrderBy(t => t.Postitoimipaikat.Postitoimipaikka);
                    break;
                default:
                    asiakkaat = asiakkaat.OrderBy(t => t.Nimi);
                    ViewBag.Sort = "A-Ö";
                    break;
            }

            //SIVUTTAMIS RUTIINIT

            const int pElements = 30;       //Määrittää montako elementtiä yhdellä sivulla
            int begin = pElements * page - pElements;   //Laskee alku indexin mistä aloitetaan lisäämään elementtejä sivulle
            int numOfPages = asiakkaat.Count() / pElements;   //Laskee montako sivua on
            ViewBag.nPages = numOfPages;
            ViewBag.CurrentPage = page;
            asiakkaat = asiakkaat.Skip(begin).Take(pElements);  //Suodattaa sivunäkymän sivu numeron ja elementti määrän mukaan
            return View(asiakkaat.ToList());
        }

        // GET: Asiakkaats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            return View(asiakkaat);
        }

        // GET: Asiakkaats/Create
        public ActionResult Create()
        {
            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka");
            return View();
        }

        // POST: Asiakkaats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Asiakkaat.Add(asiakkaat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        // GET: Asiakkaats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        // POST: Asiakkaats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsiakasID,Nimi,Osoite,Postinumero")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakkaat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Postinumero = new SelectList(db.Postitoimipaikat, "Postinumero", "Postitoimipaikka", asiakkaat.Postinumero);
            return View(asiakkaat);
        }

        // GET: Asiakkaats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            if (asiakkaat == null)
            {
                return HttpNotFound();
            }
            return View(asiakkaat);
        }

        // POST: Asiakkaats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Asiakkaat asiakkaat = db.Asiakkaat.Find(id);
            db.Asiakkaat.Remove(asiakkaat);
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
