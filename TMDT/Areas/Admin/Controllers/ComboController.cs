using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using TMDT.MauThietKe;

namespace TMDT.Areas.Admin.Controllers
{
    public class ComboController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        private ComboSingleton comboSingleton = ComboSingleton.instance;

        // GET: Admin/Combo
        /*public ActionResult Index()
        {

            return View(db.Combo.ToList());
        }*/

        public ActionResult Index()
        {
            comboSingleton.Init(db);

            var lsCombo = comboSingleton.lsCombo;

            return View(lsCombo);
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
                        if (item.upSize == false) sumPrice += product.price * item.quantity;
                        else sumPrice += (product.price + product.priceUp) * item.quantity;
                        lstComboDetail.Add(new ComboDetail{comboID = combo.comboID, cateID = item.producID, quantity = item.quantity, sizeUP = item.upSize });
                    }

                    combo.price = sumPrice * (100-combo.sale)/100;

                    db.Combo.Add(combo);
                    db.SaveChanges();

                    db.ComboDetail.AddRange(lstComboDetail);
                    db.SaveChanges();

                    TempData["result"] = true;
                    TempData["notification"] = "Tạo thành công";

                    Session["combo"] = null;

                    return RedirectToAction("Index","Product");
                }
                else {
                    TempData["result"] = false;
                    TempData["notification"] = "Tạo không thành công";

                    Session["combo"] = null;

                    return RedirectToAction("Index", "Product");
                }
            }
            catch (Exception e) {
                TempData["result"] = false;
                TempData["notification"] = "Tạo không thành công";

                Session["combo"] = null;

                return RedirectToAction("Index", "Product");
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
