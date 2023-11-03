using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;


namespace TMDT.Areas.KhachHang.Controllers
{
    public class HomeController : Controller
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: KhachHangVangLai/Home
        public ActionResult Index()
        {
            var lstProduct = db.Product.ToList();
            return View(lstProduct);
        }

        public ActionResult LayProductType()
        {
            var lstType = db.Category.ToList();
            return PartialView(lstType);
        }

        public ActionResult LocSanPham(int id)
        {
            //Lấy sản phẩm theo id Type
            var lstProductType = db.Product.Where(Product => Product.typeID == id).ToList();

            //Trả về view để render các sản phẩm trên
            return View("Index", lstProductType);
        }

        public ActionResult ChiTietSP(int id)
        {
            var sp = db.Product.FirstOrDefault(s => s.cateID == id);
            return View(sp);
        }
    }
}