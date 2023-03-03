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
    [AuthFilter(RequiredLevel = 3)]
    public class KayttajienHallintasController : BaseController
    {

        // GET: KayttajienHallintas
        public ActionResult Index()
        {
            return View(LoginsVM.GetUserList(Convert.ToString(Session["UserName"])));
        }

        // GET: KayttajienHallintas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KayttajienHallintas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,PassWord,Level")] Logins logins)
        {
            logins.LoginId = LoginsVM.GetLoginId(db);
            var log = from i in db.Logins
                      where i.UserName == logins.UserName
                      select i;

            if (log.Any())
            {
                ModelState.AddModelError("UserName", "Käyttäjätunnus on jo olemassa");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    db.Logins.Add(logins);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(logins);
        }

        // GET: KayttajienHallintas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logins logins = db.Logins.Find(id);
            if (logins == null)
            {
                return HttpNotFound();
            }
            return View(logins);
        }

        // POST: KayttajienHallintas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoginId,UserName,PassWord,Level")] Logins logins)
        {


            if (ModelState.IsValid)
            {
                db.Entry(logins).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(logins);
        }

        // GET: KayttajienHallintas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logins logins = db.Logins.Find(id);
            if (logins == null)
            {
                return HttpNotFound();
            }
            return View(logins);
        }

        // POST: KayttajienHallintas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Logins logins = db.Logins.Find(id);
            db.Logins.Remove(logins);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AuthFilter(RequiredLevel = 1)]
        public ActionResult SalasananVaihto(string eka, string toka)
        {
            if ((string.IsNullOrEmpty(eka) || string.IsNullOrEmpty(toka)) || eka != toka)
            {
                return View();
            }
            string test = Convert.ToString(Session["UserName"]);

            var kayttajat = from i in db.Logins
                            select i;

            var k = kayttajat.ToList();

            Logins l = k.Find(x => x.UserName == test);
            l.PassWord = eka;

            db.Entry(l).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "TilausHallinta");
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
