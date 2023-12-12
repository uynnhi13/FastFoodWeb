using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TMDT.Models;
using PagedList;


namespace TMDT.Areas.KhachHang.Controllers
{
    public class HomeController : Controller
    {
        TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: KhachHangVangLai/Home
        public ActionResult Index()
        {
            var lstProduct = db.Combo;
            return View(lstProduct);
        }

        public ActionResult LayProductType(int? idCate)
        {
            var lstType = db.Category;
            return PartialView(lstType);
        }

        public ActionResult LocSanPham(int id)
        {
            //Lấy sản phẩm theo id Type
            var lstProductType = db.Product.Where(Product => Product.typeID == id);

            var lstCombo = new List<Combo>();
            foreach (var product in lstProductType) {
                var combodetails = db.ComboDetail.FirstOrDefault(s => s.cateID == product.cateID && s.sizeUP == false);
                var comboproduct = db.Combo.FirstOrDefault(s => s.comboID == combodetails.comboID);
                lstCombo.Add(comboproduct);
            }

            //Trả về view để render các sản phẩm trên
            return View("Index", lstCombo);
        }

        public ActionResult LocCombo()
        {
            var lstCombo = db.Combo.Where(s => s.typeCombo == true);
            return View("Index", lstCombo);
        }

        public ActionResult ChiTietSP(int id)
        {
            var sp = db.Combo.FirstOrDefault(s => s.comboID == id);
            return View(sp);
        }
        //NN

        public ActionResult LayProductTypeS(int? idCate)
        {
            var lstType = db.Category;
            return PartialView(lstType);
        }

        public ActionResult LocSanPhamS(int id, int? page)
        {
            //Lấy sản phẩm theo id Type
            var lstProductType = db.Product.Where(Product => Product.typeID == id);

            var lstCombo = new List<Combo>();
            foreach (var product in lstProductType) {
                var combodetails = db.ComboDetail.FirstOrDefault(s => s.cateID == product.cateID && s.sizeUP == false);
                var comboproduct = db.Combo.FirstOrDefault(s => s.comboID == combodetails.comboID);
                lstCombo.Add(comboproduct);
            }

            int pagesize = 16;
            int pagenum = (page ?? 1);
            return View(lstCombo.ToPagedList(pagenum, pagesize));
            //Trả về view để render các sản phẩm trên
          
        }
        public ActionResult LocComboS()
        {
            var lstCombo = db.Combo.Where(s => s.typeCombo == true);
            return View(lstCombo);
        }

    }
}