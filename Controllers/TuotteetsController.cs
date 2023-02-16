using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TilausJärjestelmä.Models;

namespace TilausJärjestelmä.Controllers
{
    //[LoginActionFilter]
    public class TuotteetsController : BaseController
    {
        

        // GET: Tuotteets
        public ActionResult Index(string sortOrder, string searchString, int page = 1)
        {
            var tuotteet = from t in db.Tuotteet
                           select t;
            ViewBag.AllItems = tuotteet.Count().ToString();
            ViewBag.Search = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                tuotteet = tuotteet.Where(s => s.Nimi.Contains(searchString));
            }

            ViewBag.Sort = sortOrder;
            switch (sortOrder)
            {
                case "A-Ö":
                    tuotteet = tuotteet.OrderBy(t => t.Nimi);
                    break;
                case "Ö-A":
                    tuotteet = tuotteet.OrderByDescending(t => t.Nimi);
                    break;
                case "Kallein ensin":
                    tuotteet = tuotteet.OrderByDescending(t => t.Ahinta);
                    break;
                case "Halvin ensin":
                    tuotteet = tuotteet.OrderBy(t => t.Ahinta);
                    break;
                default:
                    tuotteet = tuotteet.OrderBy(t => t.Nimi);
                    ViewBag.Sort = "A-Ö";
                    break;
            }

            //SIVUTTAMIS RUTIINIT

            const int pElements = 30;       //Määrittää montako elementtiä yhdellä sivulla
            int begin = pElements * page - pElements;   //Laskee alku indexin mistä aloitetaan lisäämään elementtejä sivulle
            int numOfPages = tuotteet.Count() / pElements + 1;   //Laskee montako sivua on
            ViewBag.nPages = numOfPages;
            ViewBag.CurrentPage = page;
            tuotteet = tuotteet.Skip(begin).Take(pElements);  //Suodattaa sivunäkymän sivu numeron ja elementti määrän mukaan
            return View(tuotteet.ToList());
        }



        // GET: Tuotteets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null)
            {
                return HttpNotFound();
            }
            return View(tuotteet);
        }

        // GET: Tuotteets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tuotteets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TuoteID,Nimi,Ahinta,Kuva")] Tuotteet tuotteet)
        {
            if (ModelState.IsValid)
            {
                if (tuotteet.Nimi == null || tuotteet.Nimi == "")
                {
                    ModelState.AddModelError("Nimi", "Nimi ei voi olla tyhjä!");
                    return View(tuotteet);
                }
                db.Tuotteet.Add(tuotteet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tuotteet);
        }

        // GET: Tuotteets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null)
            {
                return HttpNotFound();
            }
            return View(tuotteet);
        }

        // POST: Tuotteets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TuoteID,Nimi,Ahinta,Kuva")] Tuotteet tuotteet)
        {
            if (ModelState.IsValid)
            {
                if (tuotteet.Nimi == null || tuotteet.Nimi == "")
                {
                    ModelState.AddModelError("Nimi", "Nimi ei voi olla tyhjä!");
                    return View(tuotteet);
                }
                db.Entry(tuotteet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tuotteet);
        }

        // GET: Tuotteets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            if (tuotteet == null)
            {
                return HttpNotFound();
            }
            return View(tuotteet);
        }

        // POST: Tuotteets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tuotteet tuotteet = db.Tuotteet.Find(id);
            db.Tuotteet.Remove(tuotteet);
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
