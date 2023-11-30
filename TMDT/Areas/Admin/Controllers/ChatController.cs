using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMDT.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        // GET: Admin/Chat
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult AdminChat()
        {
            return View();
        }
    }
}