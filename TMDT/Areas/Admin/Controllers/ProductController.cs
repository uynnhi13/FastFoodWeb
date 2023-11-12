using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: Admin/Product
        public ActionResult Index()
        {
            var product = db.Product.Include(p => p.Category);
            return View(product.ToList());
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null) {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.typeID = new SelectList(db.Category, "typeID", "nameType");
            ViewBag.lstCategory = db.Category;
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for     
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cateID,name,price,image,typeID,priceUp")] Product product, HttpPostedFileBase HinhAnh)
        {
            try {
                if ((HinhAnh != null && HinhAnh.ContentLength > 0) && ModelState.IsValid) {
                    // luu file
                    string Noiluu = Server.MapPath("/Images/Product/");
                    String PathImg = Noiluu + HinhAnh.FileName;
                    HinhAnh.SaveAs(PathImg);

                    // Màu sắc điện thoại

                    string img = "/Images/Product/" + (string)HinhAnh.FileName;

                    db.AddProductAndCombo(product.name, product.price, img, product.typeID, product.priceUp);
                    db.SaveChanges();

                    ViewBag.notification = true;
                    return View("Create");
                }
                else {
                    ViewBag.notification = false;
                    return View("Create");
                }
            }
            catch (Exception e) {
                ViewBag.notification = false;
                return View("Create");
            }

        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null) {
                return HttpNotFound();
            }
            ViewBag.typeID = new SelectList(db.Category, "typeID", "nameType", product.typeID);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cateID,name,price,image,typeID,priceUp")] Product product)
        {
            if (ModelState.IsValid) {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.typeID = new SelectList(db.Category, "typeID", "nameType", product.typeID);
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null) {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
