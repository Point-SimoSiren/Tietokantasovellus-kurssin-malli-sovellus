using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TilausDBMVC.Models;
using TilausDBMVC.ViewModels;

namespace TilausDBMVC.Controllers
{
    public class AsiakkaatController : Controller
    {
        private TilausDBEntities db = new TilausDBEntities();

        // GET: Asiakkaat
        public ActionResult Index()
        {
            if (Session["UserName"] == null)
            {
                ViewBag.LoggedStatus = "Out";
                return RedirectToAction("login", "home");
            }
            else
            {
                var asiakkaat = db.Asiakkaat.Include(a => a.Postitoimipaikat);
                ViewBag.LoggedStatus = "In";
                return View(asiakkaat.ToList());
            }
        }

        public ActionResult AsiakasTieto()
        {
            if (Session["UserName"] == null)
            {
                ViewBag.LoggedStatus = "Out";
                return RedirectToAction("login", "home");
            }
            else
            {
                List<AsiakasTiedot> model = new List<AsiakasTiedot>();

                try
                {
                    List<Asiakkaat> asiakkaat = db.Asiakkaat.OrderByDescending(Asiakkaat => Asiakkaat.Sukunimi).ToList();

                    foreach (Asiakkaat asiakas in asiakkaat)
                    {
                        AsiakasTiedot asi = new AsiakasTiedot();
                        asi.AsiakasID = asiakas.AsiakasID;
                        asi.PostiID = asiakas.PostiID;
                        asi.Nimi = asiakas.Nimi;
                        asi.Etunimi = asiakas.Etunimi;
                        asi.Sukunimi = asiakas.Sukunimi;
                        asi.Osoite = asiakas.Osoite;

                        asi.Postinumero = asiakas.Postitoimipaikat?.Postinumero;

                        asi.Postitoimipaikka = asiakas.Postitoimipaikat?.Postitoimipaikka;

                        asi.Postnro = asiakas.Postnro;
                        asi.Postplace = asiakas.Post?.Postplace;

                        asi.EtuNimiAsiakas = asiakas.Etunimi;
                        asi.SukuNimiAsiakas = asiakas.Sukunimi;

                        model.Add(asi);
                    }
                }
                finally
                {
                    db.Dispose();
                }

                return View(model);
            }
        }


        // GET: Asiakkaat/Details/5
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

        // GET: Asiakkaat/Create
        public ActionResult Create()
        {
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };


            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postinumero", "Postitoimipaikka");
            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text");

            return View();
        }

        // POST: Asiakkaat/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsiakasID,PostiID,Nimi,Etunimi,Sukunimi,Osoite,Postinumero,Postitoimipaikka,Postnro,Postplace")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Asiakkaat.Add(asiakkaat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };

            ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postinumero", "Postitoimipaikka", asiakkaat.PostiID);
            ViewBag.Postilista = new SelectList(selectPostiList, "Value", "Text", asiakkaat.PostiID);


            return View(asiakkaat);
        }

        // GET: Asiakkaat/Edit/5
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

            var asiakas = db.Asiakkaat;
            IEnumerable<SelectListItem> selectList = from s in asiakas
                                                     select new SelectListItem
                                                     {
                                                         Value = s.AsiakasID.ToString(),
                                                         Text = s.Etunimi + " " + s.Sukunimi.ToString()
                                                     };

            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };
            var post = db.Post;
            IEnumerable<SelectListItem> selectPostList = from p in post
                                                         select new SelectListItem
                                                         {
                                                             Value = p.Postnro,
                                                             Text = p.Postnro + " " + p.Postplace
                                                         };

            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postinumero", "Postitoimipaikka", asiakkaat.PostiID);

            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text", asiakkaat.PostiID);
            
            ViewBag.Postnro = new SelectList(selectPostList, "Value", "Text", asiakkaat.Postnro);

            ViewBag.AsiakasID = new SelectList(selectList, "Value", "Text", asiakkaat.AsiakasID);
            
            return View(asiakkaat);
        }



        // POST: Asiakkaat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsiakasID,PostiID,Nimi,Etunimi,Sukunimi,Osoite,Postinumero,Postitoimipaikka,Postnro,Postplace")] Asiakkaat asiakkaat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asiakkaat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var asiakas = db.Asiakkaat;
            IEnumerable<SelectListItem> selectList = from s in asiakas
                                                     select new SelectListItem
                                                     {
                                                         Value = s.AsiakasID.ToString(),
                                                         Text = s.Etunimi + "" + s.Sukunimi.ToString()
                                                     };
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + "" + p.Postitoimipaikka.ToString()
                                                          };

            var post = db.Post;
            IEnumerable<SelectListItem> selectPostList = from p in post
                                                         select new SelectListItem
                                                         {
                                                             Value = p.Postnro,
                                                             Text = p.Postnro + " " + p.Postplace
                                                         };

            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postinumero", "Postitoimipaikka", asiakkaat.PostiID);
            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text", asiakkaat.PostiID);

            ViewBag.Postnro = new SelectList(selectPostList, "Value", "Text", asiakkaat.Postnro);
            
            ViewBag.AsiakasID = new SelectList(selectList, "Value", "Text", asiakkaat.AsiakasID);
            return View(asiakkaat);
        }

        // GET: Asiakkaat/Delete/5
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

        // POST: Asiakkaat/Delete/5
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
