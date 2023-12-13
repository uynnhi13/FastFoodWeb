using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.Admin.Controllers
{
    public class ChatController : Controller
    {
        MongoDBconnection db = new MongoDBconnection();
        TMDTThucAnNhanhEntities _db = new TMDTThucAnNhanhEntities();
        // GET: Admin/Chat
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult AdminChat()
        {
            
            List<Bmessage> allChat = new List<Bmessage>();
            allChat = db.lsMessage;

            List<Bmessage> lsPeopleChat = new List<Bmessage>();

            setLsPeople(lsPeopleChat, allChat);

            ViewBag.allChat = allChat;
            ViewBag.lsPeopleChat = lsPeopleChat;
            ViewBag.PeopleChat = new Bmessage();

            return View();
        }

        public ActionResult filterChat(string user_id)
        {
            List<Bmessage> lsChat = new List<Bmessage>();
            lsChat = db.lsMessage.Where(f=>f.user_id.Contains(user_id)).ToList();

            List<Bmessage> lsPeopleChat = new List<Bmessage>();

            setLsPeople(lsPeopleChat, db.lsMessage);

            var user = lsPeopleChat.FirstOrDefault(f => f.user_id.Contains(user_id) && f.user_id.Length == user_id.Length);

            ViewBag.lsPeopleChat = lsPeopleChat;
            ViewBag.PeopleChat = user;

            return View("AdminChat", lsChat);
        }

        private void setLsPeople(List<Bmessage> lsPeopleChat, List<Bmessage> allChat)
        {
            for (int i = 0; i < allChat.Count; i++) {
                if (!lsPeopleChat.Any(a => a.user_id.Contains(allChat[i].user_id) && a.user_id.Length == allChat[i].user_id.Length)) {
                    lsPeopleChat.Add(allChat[i]);
                }
            }

            //1: lấy danh sách từ db | 2: lọc danh sách user đã chat
            var lsUser = _db.User.ToList();

            User tam;
            int dem = 0;
            for (int i = 0; i < lsPeopleChat.Count; i++) {
                tam = lsUser.FirstOrDefault(a => a.numberPhone.Contains(lsPeopleChat[i].user_id) && a.numberPhone.Length == lsPeopleChat[i].user_id.Length);
                if (tam != null) {
                    lsPeopleChat[i].message = tam.fullName;
                }
                else {
                    dem++;
                    lsPeopleChat[i].message = "Người dùng " + dem;
                }
            }
        }
    }
}