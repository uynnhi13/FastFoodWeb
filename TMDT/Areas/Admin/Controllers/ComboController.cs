using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.Admin.Controllers
{
    public class ComboController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: Admin/Combo
        public ActionResult Index()
        {
            return View(db.Combo.ToList());
        }

        // GET: Admin/Combo/Details/5

        public ActionResult Create()
        {
            var lsitemCombo = new List<itemProduct>();
            lsitemCombo = LayCombo();
            return View(lsitemCombo);
        }
        public List<itemProduct> LayCombo()
        {
            List<itemProduct> lstCombo = Session["combo"] as List<itemProduct>;

            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (lstCombo == null) {
                lstCombo = new List<itemProduct>();
                Session["combo"] = lstCombo;
            }
            return lstCombo;
        }
        // POST: Admin/Combo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "nameCombo,sale")] Combo combo, HttpPostedFileBase HinhAnh)
        {
            try {
                if (HinhAnh != null && HinhAnh.ContentLength > 0) {
                    // luu file
                    string Noiluu = Server.MapPath("/Images/Product/");
                    String PathImg = Noiluu + HinhAnh.FileName;
                    HinhAnh.SaveAs(PathImg);

                    // Màu sắc điện thoại

                    string img = "/Images/Product/" + (string)HinhAnh.FileName;

                    combo.image = img;
                    combo.typeCombo = true;
                    combo.comboID = db.Database.SqlQuery<int>("SELECT dbo.GetNewComboID()").SingleOrDefault();

                    var lsitemCombo = new List<itemProduct>();
                    lsitemCombo = LayCombo();
                    var lstComboDetail = new List<ComboDetail>();

                    //tong tien
                    decimal sumPrice = 0;
                    foreach (var item in lsitemCombo) {
                        var product = db.Product.FirstOrDefault(f => f.cateID == item.producID);
                        sumPrice += product.price;
                        lstComboDetail.Add(new ComboDetail(combo.comboID, item.producID, item.quantity, item.upSize));
                    }

                    combo.price = sumPrice;

                    db.Combo.Add(combo);
                    db.SaveChanges();

                    db.ComboDetail.AddRange(lstComboDetail);
                    db.SaveChanges();



                    ViewBag.notification = true;
                    return RedirectToAction("Index","Product",db.Combo);
                }
                else {
                    var lsitemCombo = new List<itemProduct>();
                    lsitemCombo = LayCombo();
                    ViewBag.notification = false;
                    return View("Create",lsitemCombo);
                }
            }
            catch (Exception e) {
                ViewBag.notification = false;
                return View("Create");
            }
        }

        // GET: Admin/Combo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combo combo = db.Combo.Find(id);
            if (combo == null) {
                return HttpNotFound();
            }
            return View(combo);
        }

        // POST: Admin/Combo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "comboID,nameCombo,price,sale,typeCombo,image")] Combo combo)
        {
            if (ModelState.IsValid) {
                db.Entry(combo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(combo);
        }

        // GET: Admin/Combo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Combo combo = db.Combo.Find(id);
            if (combo == null) {
                return HttpNotFound();
            }
            return View(combo);
        }

        // POST: Admin/Combo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Combo combo = db.Combo.Find(id);
            db.Combo.Remove(combo);
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
