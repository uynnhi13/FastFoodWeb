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

        //Start THÔNG TIN USER//
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
        //End THÔNG TIN USER//
      

        //Start địa chỉ//
        public ActionResult LocaList()
        {
            var user = (User)Session["TaiKhoan"];
            var userPhone = user.numberPhone;
            var userAddresses = db.Address.Where(a => a.userID.Equals(userPhone)).ToList();
            if (userAddresses.Any()) {
                return View(userAddresses); // Trả về danh sách địa chỉ nếu có
            }
            else {
                // Xử lý trường hợp không có địa chỉ nào được tìm thấy
                TempData["Message"] = "Bạn chưa có địa chỉ. Vui lòng thêm địa chỉ.";
                TempData.Keep("Message"); // Giữ lại TempData cho request tiếp theo
                return RedirectToAction("LocaAdd"); // Chuyển hướng đến action thêm địa chỉ
            }

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
                var addres = db.Address.FirstOrDefault();

                if (ad.priority) {
                    var lsAad = db.Address.Where(w => w.userID == user.numberPhone);

                    foreach (var item in lsAad) {
                        if (item.priority) {
                            addres = db.Address.FirstOrDefault(f => f.addressID == item.addressID);
                            addres.priority = false;
                            
                        }
                    }

                }

                db.SaveChanges();



                ad.userID = user.numberPhone;
                db.Address.Add(ad);
                db.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("LocaList", "NguoiDung");
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
                var user = (User)Session["TaiKhoan"];

                var addres = db.Address.FirstOrDefault();

                if (ad.priority) {
                    var lsAad = db.Address.Where(w=>w.userID == user.numberPhone);

                    foreach(var item in lsAad) {
                        if (item.priority) {
                            addres = db.Address.FirstOrDefault(f => f.addressID == item.addressID);
                            addres.priority = false;
                      
                        } 
                    }
                    
                }

                db.SaveChanges();

                var aad = db.Address.FirstOrDefault(s => s.addressID == ad.addressID);

                aad.firstName = ad.firstName;
                aad.lastName = ad.lastName;
                aad.numberPhone = ad.numberPhone;
                aad.note = ad.note;
                aad.priority = ad.priority;
                aad.address1 = ad.address1;

                db.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("LocaList");
        }
        public ActionResult DeleteAddress(int id)

        {
       
            var address = db.Address.FirstOrDefault(a => a.addressID == id);

            if (address != null) {
                db.Address.Remove(address);
                db.SaveChanges();
              
            }
         
            
            return RedirectToAction("LocaList");
        }
        //Start Order//
        public ActionResult OrderList()
        {
        
            var user = (User)Session["TaiKhoan"];
            var userPhone = user.numberPhone;
            var order = db.Order.Where(a => a.numberPhone.Equals(userPhone)).ToList();
            return View(order);
         
        }
        public ActionResult OrderDetail()
        {
          
            var user = (User)Session["TaiKhoan"];

            var ordt = db.Order.FirstOrDefault(u => u.numberPhone == user.numberPhone);

            return View(ordt);
        }
        //End Order//

    }
}