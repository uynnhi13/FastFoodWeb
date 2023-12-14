using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.KhachHang.Controllers
{
    public class ChatController : Controller
    {
        // GET: KhachHang/Chat
        public ActionResult Chat()
        {
            var user = (User)Session["TaiKhoan"];
            ViewBag.user_id = user?.numberPhone;
            return PartialView("Chat");
        }
    }
}