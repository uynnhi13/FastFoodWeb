using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Facebook;
using System.Collections.Generic;
using TMDT.Models;
using TMDT.Mtk;


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
                        user.IsActive = true;
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
                if (string.IsNullOrEmpty(user.numberPhone) && string.IsNullOrEmpty(user.gmail))
                    ModelState.AddModelError(string.Empty, "Số điện thoại hoặc gmail không được để trống");
                if (string.IsNullOrEmpty(user.password))
                    ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống");
                if (ModelState.IsValid) {
                    //tìm khách hàng có sđt và password hợp lệ trong csdl
                    var kiemtra = db.User.FirstOrDefault(k => k.numberPhone == user.numberPhone && k.password == user.password);

                    if (kiemtra != null) {
                        if (kiemtra.IsActive == true) {
                            //Lưu vào session
                            Session["TaiKhoan"] = user;
                            ViewBag.TaiKhoan = Session["TaiKhoan"];

                        }

                    }
                    else {
                        var tk = db.Employees.Where(s => s.Email == user.gmail && s.password == user.password).FirstOrDefault();

                        if (tk == null) {
                            ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không hợp lệ.";
                            return View();
                        }
                        else {
                            Session["user"] = tk;
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });

                        }
                        //ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không hợp lệ.";
                        //// Chuyển hướng người dùng trở lại trang đăng nhập
                        //return View();
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
            var user = db.User.FirstOrDefault(u => u.numberPhone == id);
            if (user == null) {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(User updatedUser, HttpPostedFileBase HinhAnh)
        {
            var user = db.User.FirstOrDefault(u => u.numberPhone == updatedUser.numberPhone);
            if (ModelState.IsValid) {
                if (HinhAnh != null && HinhAnh.ContentLength > 0) {
                    // Kiểm tra loại file hình ảnh
                    if (HinhAnh.ContentType == "image/jpeg" || HinhAnh.ContentType == "image/png") {
                        // Lưu hình ảnh
                        string folderPath = Server.MapPath("~/Images/User/");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(HinhAnh.FileName);
                        string imagePath = Path.Combine(folderPath, uniqueFileName);
                        HinhAnh.SaveAs(imagePath);
                        user.userpic = "~/Images/User/" + uniqueFileName;
                        db.SaveChanges(); // Lưu thay đổi
                    }
                    else {
                        ModelState.AddModelError("", "Chỉ chấp nhận file hình ảnh (jpg, png).");
                        return View(); // Hiển thị view với thông báo lỗi
                    }
                }

                user.fullName = updatedUser.fullName; // Cập nhật thông tin người dùng
                user.gmail = updatedUser.gmail;
                user.numberPhone = updatedUser.numberPhone;
                user.bDay = updatedUser.bDay;
                user.password = updatedUser.password;
                user.gender = updatedUser.gender;



                // Cập nhật các trường thông tin người dùng khác tương tự ở đây

                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();// Lưu thay đổi
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
                    /*
                                        foreach (var item in lsAad) {
                                            if (item.priority) {
                                                addres = db.Address.FirstOrDefault(f => f.addressID == item.addressID);
                                                addres.priority = false;

                                            }
                                        }*/
                    Iteratorr iteratorr = new Iterator(lsAad);
                    for (var item = iteratorr.First();
                        !iteratorr.IsDone; item = iteratorr.Next()) {
                        addres = db.Address.FirstOrDefault(f => f.addressID == item.addressID);
                        addres.priority = false;
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
                aad.district = ad.district;

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
            //var revi = db.Order.Where(o => o.star != null && !string.IsNullOrEmpty(o.comment)).ToList();
            //if (revi != null) {
            //    var s = revi.Where(o => o.star > 3);
            //        return PartialView(s);
            //}
            var revi = db.Order.Where(o => o.star != null && !string.IsNullOrEmpty(o.comment) && o.star >= 3).ToList();

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
        public ActionResult SendResetLink()
        {
            return View();
        }

        // POST: /ForgotPassword/SendResetLink
        [HttpPost]
        public ActionResult SendResetLink(string email)
        {
            var user = db.User.FirstOrDefault(u => u.gmail == email);

            if (user != null) {
                // Gọi hàm để gửi email chứa liên kết đổi mật khẩu
                bool emailSent = SendResetPasswordEmail(email);

                if (emailSent) {

                    Session["Mailvl"] = email;
                    return RedirectToAction("Noficication");
                }
                else {
                    ViewBag.ErrorMessage = "Không gửi đc email ";
                }
            }
            else {
                ViewBag.ErrorMessage = "Không tìm thấy Email của ban";
            }
            return View();
        }
        string smtpServer = ConfigurationManager.AppSettings["smtpServer"];
        bool enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);
        int smtpPort = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
        string smtpUser = ConfigurationManager.AppSettings["smtpUser"];
        string smtpPass = ConfigurationManager.AppSettings["smtpPass"];
        string adminEmail = ConfigurationManager.AppSettings["adminEmail"];

        private bool SendResetPasswordEmail(string emailAddress)
        {
            string mailBody = "Link to reset password:  ";


            MailMessage message = new MailMessage(smtpUser, emailAddress);
            message.Subject = "Reset Your Password";
            message.Body = mailBody + "https://localhost:44322/KhachHang/NguoiDung/ChangePasswordvl";

            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Send(message);
            return true; // Email đã được gửi thành công


        }
        public ActionResult Noficication()
        {

            return View();
        }

        //End Order//
        public ActionResult FacebookLogin()
        {
            var fb = new FacebookClient();
            var loginParameters = new Dictionary<string, object>
            {
        { "client_id", "1002499694175469" },
        { "client_secret", "b9022ebcfa20c3197c53f2f484b2a4b6" },
        { "redirect_uri", "https://localhost:44322/Home/FacebookCallback" },
        { "response_type", "code" },
        { "scope", "public_profile,email" } // Thêm các quyền bạn muốn yêu cầu từ người dùng
    };

            // Xây dựng URL đăng nhập Facebook
            var loginUrl = fb.GetLoginUrl(loginParameters);
            return Redirect(loginUrl.AbsoluteUri);
            /*return Redirect(loginUrl.AbsoluteUri);*/
        }

        public ActionResult FacebookCallback(string code)
        {
            try {
                var fb = new FacebookClient();

                // Truy cập mã truy cập và lấy thông tin người dùng
                dynamic result = fb.Post("oauth/access_token", new {
                    client_id = "1002499694175469",
                    client_secret = "b9022ebcfa20c3197c53f2f484b2a4b6",
                    redirect_uri = "https://localhost:44322/Home/FacebookCallback",
                    code = code
                });

                fb.AccessToken = result.access_token;

                // Lấy thông tin người dùng từ Facebook
                dynamic fbUser = fb.Get("/me?fields=name,email");

                string name = fbUser.name;
                string email = fbUser.email;

                // Thêm lệnh ghi log để kiểm tra giá trị
                System.Diagnostics.Debug.WriteLine($"Name: {name}, Email: {email}");

                return RedirectToAction("DangNhap", "NguoiDung");
            }
            catch (Exception ex) {
                // Ghi log nếu có lỗi
                System.Diagnostics.Debug.WriteLine($"Exception in FacebookCallback: {ex.Message}");
                throw; // hoặc xử lý lỗi theo nhu cầu
            }
        }

        // đổi mật khẩu 
        public ActionResult ChangePasswordvl()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePasswordvl(string currentPassword, string newPassword, string confirmPassword)
        {
            var email = Session["Mailvl"] as string; // Lấy email từ session

            if (email != null) {
                var user = db.User.FirstOrDefault(u => u.gmail == email);


                if (user != null) {
                    if (newPassword == confirmPassword) {
                        user.password = newPassword;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.ErrorMessage = "Thay đổi mật khẩu thành công ";
                        return RedirectToAction("Login", "NguoiDung");
                    }
                    else {
                        ViewBag.ErrorMessage = "Xác nhận mật khẩu không đúng";
                    }
                }
                else {
                    return RedirectToAction("Login", "NguoiDung");
                }
            }
            else {
                // Xử lý khi không tìm thấy email trong session
            }

            return View();
        }


        [HttpGet]
        public ActionResult LienHe()
        {
            return View();

        }

        [HttpPost]
        public ActionResult LienHe(Models.Mail _objModelMail)
        {
            if (ModelState.IsValid) {
                MailMessage mail = new MailMessage();
                mail.To.Add("tranmytien251202@gmail.com");
                mail.From = new MailAddress("tranmytien251202@gmail.com");

                // Lấy thông tin từ form
                string fullName = _objModelMail.Fullname;
                string email = _objModelMail.Email;
                string subject = _objModelMail.Subject;
                string content = _objModelMail.Content;

                // Thiết lập nội dung email
                mail.Subject = subject;
                string body = $"Họ và tên: {fullName}<br/>Email: {email}<br/>Nội dung: {content}";
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("leoleoleo247247@gmail.com", "rhph fuik oxfg daea"); // Nhập tên người gửi và mật khẩu
                smtp.EnableSsl = true;
                smtp.Send(mail);

                // Trả về cái gì đó
                return View("LienHe", "NguoiDung");
            }
            else {
                return View();
            }

        }
    }
}



