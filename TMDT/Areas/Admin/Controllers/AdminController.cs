using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        TMDTThucAnNhanhEntities database = new TMDTThucAnNhanhEntities();
        // GET: Admin/Admin


        public ActionResult Index()
        {
            //session = null thi chuyen den trang dang nhap
            if (Session["user"] == null) {
                return RedirectToAction("Login", "Admin");

            }
            else {

                return View();
            }


        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            var tk = database.Employees.Where(s => s.Email == user && s.password == password).FirstOrDefault();

            if (tk == null) {
                TempData["errorlogin"] = "Tài khoản đăng nhập không đúng";
                return View();
            }
            else {
                Session["user"] = tk;
                return RedirectToAction("Index", "Admin");
            }

        }


        public ActionResult LogOut()
        {

            Session.Remove("user");
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Admin");
        }

        public ActionResult Account()
        {
            var dsnv = database.Employees;
            return View(dsnv);
        }

        public ActionResult AddnewNV()
        {
            ViewBag.PositionID = new SelectList(database.Position.ToList(), "PositionID", "posName");
            return View();
        }
        [HttpPost]
        public ActionResult AddnewNV(Employees em, HttpPostedFileBase Hinhanhnv)
        {
            ViewBag.PositionID = new SelectList(database.Position.ToList(), "PositionID", "posName");
            if (Hinhanhnv == null) {
                ViewBag.ThongBao = "Vui lòng thêm ảnh nhân viên";
                return View();

            }
            else {
                if (ModelState.IsValid) {
                    var fileName = Path.GetFileName(Hinhanhnv.FileName);

                    var path = Path.Combine(Server.MapPath("~/Areas/Admin/Content/img/"), fileName);

                    if (System.IO.File.Exists(path)) {
                        ViewBag.ThongBao = "Hình đã tồn tại ";

                    }
                    else {
                        Hinhanhnv.SaveAs(path);
                    }
                    em.imgEP = fileName;
                    database.Employees.Add(em);
                    database.SaveChanges();

                }
                return RedirectToAction("Account", "Admin");
            }


        }
        public ActionResult DetailsNV(int id)
        {
            var em = database.Employees.FirstOrDefault(s => s.EmployeeID == id);

            return View(em);
        }

        public ActionResult EditNV(int id)
        {
            var em = database.Employees.FirstOrDefault(s => s.EmployeeID == id);
            if (em == null) {
                return HttpNotFound();
            }
            return View(em);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNV(Employees emp)
        {
            if (ModelState.IsValid) {

                database.Entry(emp).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges(); // Lưu thay đổi

                // Chuyển hướng đến trang danh sách nhân viên hoặc trang khác theo ý muốn
                return RedirectToAction("Account");
            }


            return View(emp);
        }
        public ActionResult DeleteNV(int id)
        {
            var em = database.Employees.FirstOrDefault(s => s.EmployeeID == id);
            if (em == null) {
                return HttpNotFound();
            }
            database.Employees.Remove(em);
            database.SaveChanges();
            return RedirectToAction("Account", "Admin");
        }
        [HttpGet]
        public ActionResult QlyKH(string searchstring = "")
        {
            var dskh = database.User;
            var lskh = database.User.Where(s => s.numberPhone.Contains(searchstring));
            if (searchstring != null) {

                return View(lskh.ToList());
            }
            return View(dskh);


        }
        public ActionResult DetailsKH(string id)
        {
            var kh = database.User.FirstOrDefault(s => s.numberPhone == id);
            return View(kh);
        }

        public ActionResult DisableAccount(string id)
        {
            //Cập nhật lại database, thêm 1 cột IsActive trong User
            var customer = database.User.FirstOrDefault(c => c.numberPhone == id);

            if (customer == null) {

                return HttpNotFound();
            }

            customer.IsActive = false;
            database.SaveChanges();
            return RedirectToAction("QlyKH");
        }
        public ActionResult EnableAccount(string id)
        {

            var customer = database.User.FirstOrDefault(c => c.numberPhone == id);

            if (customer == null) {

                return HttpNotFound();
            }

            customer.IsActive = true;
            database.SaveChanges();
            return RedirectToAction("QlyKH");
        }
        public ActionResult DonHang(int? id)
        {
            var donhang = database.Order.Include(s => s.OrderDetail);
            return View(donhang);
        }
        [HttpPost]
        public ActionResult DonHang(DateTime? startdate, DateTime? enddate)
        {
            IQueryable<Order> orders = database.Order;

            if (startdate != null && enddate != null) {
                enddate = enddate.Value.AddDays(1).AddTicks(-1);
                orders = orders.Where(o => o.datetime >= startdate && o.datetime <= enddate /*&& o.conditionID == 2*/);
                return View(orders.ToList());
            }
            else {
                var donhang = database.Order.Include(s => s.OrderDetail);
                return View(donhang);
            }

        }

        public ActionResult XacNhanDH(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order donhang = database.Order.Find(id);


            donhang.conditionID = 2;
            if (donhang.employeeID == null) {
                var searchU = (Employees)Session["user"];
                donhang.employeeID = searchU.EmployeeID;

            }
            database.SaveChanges();
            if (donhang == null) {
                return HttpNotFound();
            }

            return RedirectToAction("DonHang", "Admin");
        }
        public ActionResult Dagiao(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order donhang = database.Order.Find(id);

            donhang.conditionID = 3;
            //if (donhang.employeeID == null) {
            //    var searchU = (Employees)Session["user"];
            //    donhang.employeeID = searchU.EmployeeID;

            //}

            database.SaveChanges();
            if (donhang == null) {
                return HttpNotFound();
            }

            return RedirectToAction("DonHang", "Admin");
        }

        public ActionResult DetailsDH(int id)
        {
            var dh = database.OrderDetail.Where(s => s.orderID == id);

            var lsProduct = new List<Combo>();
            var lsCombo = new List<Combo>();

            foreach (var item in dh) {
                var lsCombotam = database.Combo.FirstOrDefault(l => l.comboID == item.comboID);

                if (lsCombotam.typeCombo == false) {
                    var combo = database.Combo.FirstOrDefault(f => f.comboID == item.comboID);
                    lsProduct.Add(combo);
                }
                else {
                    var combo = database.Combo.FirstOrDefault(f => f.comboID == item.comboID);
                    lsCombo.Add(combo);
                }
            }
            ViewBag.LsProduct = lsProduct;
            ViewBag.LsCombo = lsCombo;



            return View(dh);

        }
        public ActionResult Mypro()
        {
            var searchU = (Employees)Session["user"];
            if (searchU != null) {
                var emUser = database.Employees.FirstOrDefault(s => s.EmployeeID == searchU.EmployeeID);
                return View(emUser);
            }
            else {
                return View();
            }

        }
        public ActionResult EditMypro()
        {
            var searchU = (Employees)Session["user"];
            if (searchU != null) {
                var emUser = database.Employees.FirstOrDefault(s => s.EmployeeID == searchU.EmployeeID);
                return View(emUser);
            }
            else {
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditMypro(Employees em)
        {
            if (ModelState.IsValid) {
                var a = database.Employees.FirstOrDefault(f => f.EmployeeID == em.EmployeeID);

                a.FirstName = em.FirstName;
                a.LastName = em.LastName;
                a.numberPhone = em.numberPhone;
                a.Email = em.Email;

                database.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("MyPro");
        }
        public ActionResult ThongKe()
        {
            return View();
        }
        public ActionResult ThongKeAccKH()
        {
            var listU = database.User.Where(u => u.IsActive == true).ToList();
            int item = listU.Count;
            return PartialView(item);
        }


    }


}
