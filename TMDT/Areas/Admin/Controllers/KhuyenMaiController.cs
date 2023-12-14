using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.Admin.Controllers
{
    public class KhuyenMaiController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: Admin/KhuyenMai
        public ActionResult Index()
        {
            return View(db.KhuyenMai.ToList());
        }

        // GET: Admin/KhuyenMai/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhuyenMai khuyenMai = db.KhuyenMai.Find(id);
            if (khuyenMai == null)
            {
                return HttpNotFound();
            }
            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMai/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhuyenMai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TenQuanLy,TenHienThi,MaKhuyenMai,LoaiKhuyenMai,GiaTriKhuyenMai,GiaTriGiamToiDa,SoTienGiam,DieuKienApDung_DonHangToiThieu,DieuKienApDung_SoLuongToiThieu,GioiHanSoLanMaGiamGia,GioiHanSoLanMaGiamGiaMoiKhachHang,ThoiGianBatDau,ThoiGianKetThuc")] KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
                db.KhuyenMai.Add(khuyenMai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMai/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhuyenMai khuyenMai = db.KhuyenMai.Find(id);

            if (khuyenMai == null)
            {
                return HttpNotFound();
            }
            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TenQuanLy,TenHienThi,MaKhuyenMai,LoaiKhuyenMai,GiaTriKhuyenMai,GiaTriGiamToiDa,SoTienGiam,DieuKienApDung_DonHangToiThieu,DieuKienApDung_SanPhamToiThieu,DieuKienApDung_SoLuongToiThieu,GioiHanSoLanMaGiamGia,GioiHanSoLanMaGiamGiaMoiKhachHang,ThoiGianBatDau,ThoiGianKetThuc")] KhuyenMai khuyenMai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khuyenMai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khuyenMai);
        }

        // GET: Admin/KhuyenMai/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhuyenMai khuyenMai = db.KhuyenMai.Find(id);
            if (khuyenMai == null)
            {
                return HttpNotFound();
            }
            return View(khuyenMai);
        }

        // POST: Admin/KhuyenMai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhuyenMai khuyenMai = db.KhuyenMai.Find(id);
            db.KhuyenMai.Remove(khuyenMai);
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

        public ActionResult Duplicat(int id)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var duplicate = db.KhuyenMai.FirstOrDefault(s => s.ID == id);
            //db.KhuyenMai.Add(duplicate);
            //db.SaveChanges();

            var khuyenMaiclone = duplicate.clone();
            db.KhuyenMai.Add((KhuyenMai)khuyenMaiclone);
            db.SaveChanges();

            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;

            // In thời gian ra Output hoặc ghi vào log
            ViewBag.HienThi = elapsedTime.Milliseconds;
            return RedirectToAction("Index");

        }

        public ActionResult Duplicate(int id)
        {
            var duplicate = db.KhuyenMai.FirstOrDefault(s => s.ID == id);
            var khuyenMaiclone = duplicate.clone();
            db.KhuyenMai.Add((KhuyenMai)khuyenMaiclone);
            db.SaveChanges();

            return RedirectToAction("Index");

        }
    }
}
