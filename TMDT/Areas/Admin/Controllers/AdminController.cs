using System;
using System.Collections.Generic;
using System.Linq;
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
            var tk = database.AdminUser.Where(s => s.nameUser == user && s.passWord == password).FirstOrDefault();
            var check = database.AdminUser.SingleOrDefault(s => s.nameUser == user && s.passWord == password);
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
            return View();
        }
    }

}
