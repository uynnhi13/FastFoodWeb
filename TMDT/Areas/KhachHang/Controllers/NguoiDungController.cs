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
                        ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                        //Lưu vào session
                        Session["TaiKhoan"] = user;
                        ViewBag.TaiKhoan = Session["TaiKhoan"];
                    }
                    else
                        ViewBag.ThongBao = "Số điện thoại hoặc mật khẩu không đúng";
                }
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}