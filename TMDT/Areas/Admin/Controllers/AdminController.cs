using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;
using System.IO;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Net;

namespace TMDT.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        TMDTThucAnNhanhEntities database = new TMDTThucAnNhanhEntities();
        // GET: Admin/Admin

        
        public ActionResult Index()
        {
            if (Session["user"] == null) {
                return RedirectToAction("Login", "Admin");
            }
            else {
                return View();
            }

            //return View();
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
                return RedirectToAction("Index","Admin");
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
            if(em == null) {
                return HttpNotFound();
            }
            database.Employees.Remove(em);
            database.SaveChanges();
            return RedirectToAction("Account","Admin");
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

        public ActionResult XacNhanDH(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order donhang = database.Order.Find(id);
            donhang.conditionID= 2;
            database.SaveChanges();
            if (donhang == null) {
                return HttpNotFound();
            }
            return RedirectToAction("DonHang", "Admin");
        }
       

    }

}
