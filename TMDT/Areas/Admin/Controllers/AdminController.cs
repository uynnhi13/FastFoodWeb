﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;
using System.IO;

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
            var tk = database.AdminUser.Where(s => s.nameUser == user && s.passWord == password).FirstOrDefault();
          
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
        

    }

}
