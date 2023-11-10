using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.KhachHang.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: KhachHang/NguoiDung
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(User user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.fullName))
                    ModelState.AddModelError(string.Empty, "Họ tên không được để trống");
                if (string.IsNullOrEmpty(user.numberPhone))
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống");
                if (string.IsNullOrEmpty(user.gmail))
                    ModelState.AddModelError(string.Empty, "Gmail không được để trống");
                if (string.IsNullOrEmpty(user.password))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");

                var kiemTraUser = db.User.FirstOrDefault(k => k.numberPhone == user.numberPhone);
                if (kiemTraUser != null)
                    ModelState.AddModelError(string.Empty, "Số điện thoại này đã được sử dụng");
                if (ModelState.IsValid)
                {
                    db.User.Add(user);
                    db.SaveChanges();
                }
                else
                    return View();
            }
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(User user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.numberPhone))
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống");
                if (string.IsNullOrEmpty(user.password))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (ModelState.IsValid)
                {
                    //tìm khách hàng có sđt và password hợp lệ trong csdl
                    var kiemtra = db.User.FirstOrDefault(k => k.numberPhone == user.numberPhone && k.password == user.password);
                    if (kiemtra != null)
                    {
                        //Lưu vào session
                        Session["TaiKhoan"] = user;
                        ViewBag.TaiKhoan = Session["TaiKhoan"];
                    }
                    else {
                        ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không hợp lệ.";
                        // Chuyển hướng người dùng trở lại trang đăng nhập
                        return View();
                    }
                        
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UserInfo()
        {
            var user = (User)Session["TaiKhoan"];
            var usi =db.User.FirstOrDefault(u => u.numberPhone == user.numberPhone);
            return View(usi);
        }

      

        public ActionResult UserEdit(string id)
        {
            var us = db.User.FirstOrDefault(u => u.numberPhone == id);
            if(us==null){
                return HttpNotFound();
            }
            return View(us);
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(User u)
        {
            if (ModelState.IsValid) {
                db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("UserInfo" ,"NguoiDung");
        }
        public ActionResult OrderList()
        {
            var dh = db.Order.ToList();
            return View(dh);
        }
        public ActionResult OrderDetail(int id )
        {
            var dt = db.OrderDetail.FirstOrDefault(u => u.orderID == id);
            if( dt ==null ){
                Response.StatusCode = 404;
                return null;

            }
            return View(dt);
        }
        public ActionResult LocaDetail()
        {
            var user = (User)Session["TaiKhoan"];

            var loc = db.Address.FirstOrDefault(u => u.userID == user.numberPhone);
            if (loc == null) {
                // Nếu địa chỉ là null, hiển thị thông báo yêu cầu thêm địa chỉ
                TempData["Message"] = "Bạn chưa có địa chỉ. Vui lòng thêm địa chỉ.";
                TempData.Keep("Message"); // Giữ lại TempData cho request tiếp theo
                return RedirectToAction("LocaAdd"); // Chuyển hướng đến action thêm địa chỉ
            }

            return View(loc);

        }
        public ActionResult LocaAdd()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocaAdd(Address ad)
        {
            if (ModelState.IsValid) {

                var user = (User)Session["TaiKhoan"];

                ad.userID = user.numberPhone;

                db.Address.Add(ad);
                db.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("LocaDetail", "NguoiDung");
        }

        public ActionResult LocaEdit(int id)
        {
            var lc = db.Address.FirstOrDefault(u => u.addressID == id);
            if (lc == null) {
                return HttpNotFound();
            }
            return View(lc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocaEdit(Address ad)
        {
            if (ModelState.IsValid) {

                var aad = db.Address.FirstOrDefault();
                aad.firstName = ad.firstName;
                aad.lastName = ad.lastName;
                aad.numberPhone = ad.numberPhone;
                aad.note = ad.note;
                aad.priority = ad.priority;
                aad.address1 = ad.address1;
                db.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("LocaDetail");
        }


    }
}