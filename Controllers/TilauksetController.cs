using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TilausDBMVC.Models;
using TilausDBMVC.ViewModels;

namespace TilausDBMVC.Controllers
{
    public class TilauksetController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string culture = filterContext.RouteData.Values["culture"]?.ToString()?? "fi";
            // Set the action parameter just in case we didn't get one
            // from the route.
            filterContext.ActionParameters["culture"] = culture;

            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            // Because we've overwritten the ActionParameters, we
            // make sure we provide the override to the 
            // base implementation.
            base.OnActionExecuting(filterContext);
        }
        public ActionResult RedirectToLocalized()
        {
            return RedirectPermanent("/fi");
        }


        private TilausDBEntities db = new TilausDBEntities();

        // GET: Tilaukset
        public ActionResult Index()
        {
            //CultureInfo fiFi = new CultureInfo("fi-FI");

            if (Session["UserName"] == null)
            {
                ViewBag.LoggedStatus = "Out";
                return RedirectToAction("login", "home");
            }
            else
            {
                var tilaukset = db.Tilaukset.Include(t => t.Asiakkaat).Include(t => t.Postitoimipaikat);
                ViewBag.LoggedStatus = "In";
          
                return View(tilaukset.ToList());
            }
        }

        public ActionResult TilausTiedot()
        {
            if (Session["UserName"] == null)
            {
                ViewBag.LoggedStatus = "Out";
                return RedirectToAction("login", "home");
            }
            else
            {
                List<TilausTiedot> model = new List<TilausTiedot>();

                try
                {
                    List<Tilaukset> tilaukset = db.Tilaukset.OrderByDescending(Tilaukset => Tilaukset.Tilauspvm).ToList();

                    CultureInfo fiFi = new CultureInfo("fi-FI");
                    RedirectToLocalized();


                    foreach (Tilaukset tilaus in tilaukset)
                    {
                        TilausTiedot til = new TilausTiedot();
                        til.TilausID = tilaus.TilausID;
                        til.AsiakasID = tilaus.AsiakasID;
                        til.Nimi = tilaus.Asiakkaat.Nimi;

                        til.PostiID = tilaus.PostiID;
                        til.Toimitusosoite = tilaus.Toimitusosoite;
                        til.Tilauspvm = tilaus.Tilauspvm.GetValueOrDefault();
                        //til.Tilauspvm = tilaus.Tilauspvm?.Value.ToString();
                        //til.Tilauspvm = tilaus.Tilauspvm?.Value.ToString("dd-MM-yyyy");
                        til.Toimituspvm = tilaus.Toimituspvm.GetValueOrDefault();
                        til.Postinumero = tilaus.Postinumero;

                        //til.Postitoimipaikka = tilaus.Postitoimipaikat?.Postinumero;
                        til.Postitoimipaikka = tilaus.Postitoimipaikat?.Postitoimipaikka;

                        model.Add(til);
                    }
                }
                finally
                {
                    ViewBag.LoggedStatus = "In";
                    db.Dispose();
                }

                return View(model);
            }
        }
    

        // GET: Tilaukset/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null)
            {
                return HttpNotFound();
            }
            return View(tilaukset);
        }

        // GET: Tilaukset/Create
        public ActionResult Create()
        {
            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };

            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Nimi");
            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postitoimipaikka");
            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text");
            //ViewBag.Postiosoite = new SelectList(db.Postitoimipaikat, "Postinumero", "PostiPaikka");
            return View();
        }

        // POST: Tilaukset/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TilausID,AsiakasID,PostiID,Toimitusosoite,Tilauspvm,Toimituspvm,Postinumero,Postitoimipaikka")] Tilaukset tilaukset)
        {
            if (ModelState.IsValid)
            {
                db.Tilaukset.Add(tilaukset);
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

            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Nimi", tilaukset.AsiakasID);
            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postitoimipaikka", tilaukset.PostiID);
            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text", tilaukset.PostiID);
            //ViewBag.Postiosoite = new SelectList(db.Postitoimipaikat, "Postinumero", "PostiPaikka", tilaukset.PostiID);
            return View(tilaukset);
        }

        // GET: Tilaukset/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null)
            {
                return HttpNotFound();
            }

            TilausTiedot til = new TilausTiedot();
            til.TilausID = tilaukset.TilausID;
            til.AsiakasID = tilaukset.AsiakasID;
            til.PostiID = tilaukset.PostiID;
            til.Nimi = tilaukset.Asiakkaat.Nimi;

            //til.Postinumero = tilaukset.Postinumero;
            til.Postitoimipaikka = tilaukset.Postitoimipaikat?.Postitoimipaikka;

            til.Toimitusosoite = tilaukset.Toimitusosoite;         
            til.Tilauspvm = tilaukset.Tilauspvm.GetValueOrDefault();
            til.Toimituspvm = tilaukset.Toimituspvm.Value;

            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };

            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Nimi", tilaukset.AsiakasID);
            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postitoimipaikka", tilaukset.PostiID);
            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text", tilaukset.PostiID);
            return View(til);
        }

        // POST: Tilaukset/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TilausID,AsiakasID,PostiID,Toimitusosoite,Tilauspvm,Toimituspvm,Postinumero,Postitoimipaikka")] Tilaukset tilaukset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tilaukset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            TilausTiedot til = new TilausTiedot();
            til.TilausID = tilaukset.TilausID;
            til.AsiakasID = tilaukset.AsiakasID;
            til.PostiID = tilaukset.PostiID;
            til.Nimi = tilaukset.Asiakkaat.Nimi;

            //til.Postinumero = tilaukset.Postinumero;
            til.Postitoimipaikka = tilaukset.Postitoimipaikat?.Postitoimipaikka;

            til.Toimitusosoite = tilaukset.Toimitusosoite;
            til.Tilauspvm = tilaukset.Tilauspvm.GetValueOrDefault();
            til.Toimituspvm = tilaukset.Toimituspvm.GetValueOrDefault();


            var posti = db.Postitoimipaikat;
            IEnumerable<SelectListItem> selectPostiList = from p in posti
                                                          select new SelectListItem
                                                          {
                                                              Value = p.PostiID.ToString(),
                                                              Text = p.Postinumero + " " + p.Postitoimipaikka.ToString()
                                                          };

            ViewBag.AsiakasID = new SelectList(db.Asiakkaat, "AsiakasID", "Nimi", tilaukset.AsiakasID);
            //ViewBag.PostiID = new SelectList(db.Postitoimipaikat, "PostiID", "Postitoimipaikka", tilaukset.PostiID);
            ViewBag.PostiID = new SelectList(selectPostiList, "Value", "Text", tilaukset.PostiID);

            return View(til);
        }

        // GET: Tilaukset/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            if (tilaukset == null)
            {
                return HttpNotFound();
            }
            return View(tilaukset);
        }

        // POST: Tilaukset/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tilaukset tilaukset = db.Tilaukset.Find(id);
            db.Tilaukset.Remove(tilaukset);
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
