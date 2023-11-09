using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMDT.Models;


namespace TMDT.Areas.Admin.Controllers
{
    public class IngredientsController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: Admin/Ingredients
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = 1;

            var ingredient = db.Ingredient.Include(i => i.Unit);
            return View(ingredient);
        }

        // GET: Admin/Ingredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // GET: Admin/Ingredients/Create
        public ActionResult Create()
        {
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU");
            return View();
        }

        // POST: Admin/Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ingID,ingName,quantity,unitID,quantityMin,arrivalDate")] Ingredient ingredient)
        {
            if (ingredient is null) {
                throw new ArgumentNullException(nameof(ingredient));
            }

            if (ModelState.IsValid)
            {
                if( ingredient.quantity == null ) ingredient.quantity = 0;
                if (ingredient.quantityMin == null) ingredient.quantityMin = 0;
                ingredient.arrivalDate = DateTime.Now;

                Ingredient ingre = new Ingredient();
                ingre = ingredient;

                db.Ingredient.Add(ingre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU", ingredient.unitID);
            return View(ingredient);
        }

        // GET: Admin/Ingredients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU", ingredient.unitID);
            return View(ingredient);
        }

        // POST: Admin/Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ingID,ingName,quantity,unitID,quantityMin,arrivalDate")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingredient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU", ingredient.unitID);
            return View(ingredient);
        }

        // GET: Admin/Ingredients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null)
            {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // POST: Admin/Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingredient = db.Ingredient.Find(id);
            db.Ingredient.Remove(ingredient);
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
