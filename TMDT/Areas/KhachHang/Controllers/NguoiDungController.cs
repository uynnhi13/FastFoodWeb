using System;
using System.Linq;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;

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
            if (ModelState.IsValid) {
                if (string.IsNullOrEmpty(user.fullName))
                    ModelState.AddModelError(string.Empty, "Họ tên không được để trống");
                if (string.IsNullOrEmpty(user.numberPhone))
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống");
                if (string.IsNullOrEmpty(user.gmail))
                    ModelState.AddModelError(string.Empty, "Gmail không được để trống");
                if (string.IsNullOrEmpty(user.password))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                var kiemTraUser = db.User.FirstOrDefault(k => k.numberPhone == user.numberPhone);
                if (kiemTraUser != null && kiemTraUser.password != null)
                    ModelState.AddModelError(string.Empty, "Số điện thoại này đã được sử dụng");

                if (ModelState.IsValid) {

                    if (kiemTraUser != null && kiemTraUser.password == null) {
                        kiemTraUser.gmail = user.gmail;
                        kiemTraUser.fullName = user.fullName;
                        kiemTraUser.password = user.password;
                        db.SaveChanges();
                    }
                    else {
                        user.password = user.password;
                        db.User.Add(user);
                        db.SaveChanges();
                    }
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
            if (ModelState.IsValid) {
                if (string.IsNullOrEmpty(user.numberPhone))
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống");
                if (string.IsNullOrEmpty(user.password))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (ModelState.IsValid) {
                    //tìm khách hàng có sđt và password hợp lệ trong csdl
                    var kiemtra = db.User.FirstOrDefault(k => k.numberPhone == user.numberPhone);
                    if (kiemtra != null && user.password == kiemtra.password) {
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
            var usi = db.User.FirstOrDefault(u => u.numberPhone == user.numberPhone);
            return View(usi);
        }



        public ActionResult UserEdit(string id)
        {
            var us = db.User.FirstOrDefault(u => u.numberPhone == id);
            if (us == null) {
                return HttpNotFound();
            }
            return View(us);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(User u)
        {
            if (string.IsNullOrEmpty(u.fullName))
                ModelState.AddModelError(string.Empty, "Họ tên không được để trống");


            if (ModelState.IsValid) {
                db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("UserInfo", "NguoiDung");
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
                    var lsAad = db.Address.Where(w => w.userID == user.numberPhone);

                    foreach (var item in lsAad) {
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

        [HttpGet]
        public ActionResult OrderList(DateTime? startdate, DateTime? enddate, int? page, Condition cd, string searchstring = "")
        {
            ViewBag.conditionID = new SelectList(db.Condition.ToList(), "conditionID", "nameCon");

            var user = (User)Session["TaiKhoan"];
            var orderQuery = db.Order.Where(o => o.numberPhone == user.numberPhone);
            // hàm tìm kím 
            if (!string.IsNullOrEmpty(searchstring)) {
                orderQuery = orderQuery.Where(o => o.orderID.ToString().Contains(searchstring) && o.orderID.ToString().Length == searchstring.Length);
            }
            // hàm dropdown theo condition 
            if (cd != null && cd.conditionID != 0) {
                orderQuery = orderQuery.Where(o => o.conditionID == cd.conditionID);
            }

            // hàm hiển thị khi đơn hàng đã giao 
            bool showButton = cd.conditionID == 3; // Kiểm tra điều kiện để xác định có hiển thị button hay không
            ViewBag.ShowReviewButton = showButton;

            if (showButton) {
                orderQuery = orderQuery.Where(o => o.conditionID == cd.conditionID);
            }

            //
            if (startdate != null && enddate != null) {
                enddate = enddate.Value.AddDays(1).AddTicks(-1);
                orderQuery = orderQuery.Where(o => o.datetime >= startdate && o.datetime <= enddate /*&& o.conditionID == 2*/);
                return View(orderQuery.ToList());
            }


            // hiển thị tổng
            var hienthi = orderQuery.ToList();

            int pagesize = 5;
            int pagenum = (page ?? 1);



            return View(hienthi.ToPagedList(pagenum, pagesize));
        }

        public ActionResult Review(int id)
        {
            var user = (User)Session["TaiKhoan"];

            var review = db.Order.FirstOrDefault(u => u.numberPhone == user.numberPhone && u.orderID == id);

            return View(review);
        }
        [HttpPost]
        public ActionResult SubmitReview(Order o)
        {

            var user = (User)Session["TaiKhoan"];
            // Lấy đơn hàng từ cơ sở dữ liệu
            var order = db.Order.FirstOrDefault(u => u.orderID == o.orderID);
            order.star = o.star;
            order.comment = o.comment;

            // Lưu đánh giá vào cơ sở dữ liệu
            db.SaveChanges();
            // Chuyển hướng người dùng đến trang xem đánh giá sau khi đã đánh giá
            return RedirectToAction("OrderList", "NguoiDung");
        }
        public ActionResult ReviewView()
        {
            var revi = db.Order.ToList();
            return PartialView(revi);

        }

        public ActionResult OrderDetail(int id)
        {
            var order = db.Order.FirstOrDefault(s => s.orderID == id);
            return View(order);
        }

        //End Order//
        public ActionResult OrderFinvl()
        {
            return View();
        }
        [HttpGet]
        public ActionResult OrderFinvl(string searchdh = " ", string searchsdt = " ")
        {
                var orderQuery = db.Order.AsQueryable();
                // Kiểm tra xem sdt
                if (!string.IsNullOrEmpty(searchsdt)) {
                    // gán đt
                    orderQuery = orderQuery.Where(o => o.numberPhone == searchsdt);
                    if (orderQuery.Any()) {
                        // Kiểm tra mã đơn
                        if (!string.IsNullOrEmpty(searchdh)) {
                            // thêm dữ liệu
                            orderQuery = orderQuery.Where(o => o.orderID.ToString().Contains(searchdh) && o.orderID.ToString().Length == searchdh.Length);
                            var order = orderQuery.FirstOrDefault(); // Lấy đơn hàng thỏa mãn cả hai điều kiện

                            if (order != null) {
                                return RedirectToAction("TimKiemVL", new { orderId = order.orderID });
                            }
                        }
                        else {
                            // hiển thị sp cuối cùng 
                            var lastItemOrder = orderQuery.OrderByDescending(o => o.orderID).First();

                            return View("TimKiemVL", lastItemOrder);
                        }
                    }
                }
                ViewBag.ErrorMessage = " Xác nhận mật khẩu không đúng  ";
            
            return View();
        }
       //Tìm kiếm đơn hàng cho khách vãng lai
      
    

        public ActionResult TimKiemVL(int orderId)
        {
            // Lấy thông tin chi tiết đơn hàng dựa trên orderId và hiển thị trang chi tiết
            var orderDetail = db.Order.FirstOrDefault(o => o.orderID == orderId);

            if (orderDetail == null) {
                // Xử lý nếu không tìm thấy đơn hàng
                return RedirectToAction("NotFound");
            }

            return View(orderDetail);
        }
        // đổi mật khẩu 
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var u = Session["TaiKhoan"] as User; // Giả sử User là đối tượng chứa thông tin tài khoản của người dùng
            var n = db.User.FirstOrDefault(o => o.numberPhone == u.numberPhone);
            if (u != null) {
                if (currentPassword == u.password) // Kiểm tra mật khẩu hiện tại của người dùng
                {
                    if (newPassword == confirmPassword) // Kiểm tra mật khẩu mới và mật khẩu xác nhận có trùng khớp không
                    {
                     

                        n.password = newPassword; // Cập nhật mật khẩu mới

                        db.Entry(n).State = EntityState.Modified;
                      
                        db.SaveChanges();
                        ViewBag.ErrorMessage = "Thay đổi thành công ";
                        // Redirect về trang chủ hoặc trang thông báo thành công
                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        ViewBag.ErrorMessage = " Xác nhận mật khẩu không đúng  ";
                    }
                }
                else {
                    ViewBag.ErrorMessage = "Nhập sai mật khẩu hiện tại";
                }
            }
            else {
                return RedirectToAction("Login", "NguoiDung"); // Nếu không có thông tin tài khoản trong Session, chuyển hướng đến trang đăng nhập
            }

            // Nếu có lỗi, hiển thị lại form thay đổi mật khẩu với thông báo lỗi
            return View();
        }
    }
}

