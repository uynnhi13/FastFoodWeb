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
        [HttpGet]
        public ActionResult LocSanPhamS(int id, int? page, string searchstring)
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
            ViewBag.CurrentFilter = searchstring;
            if (!string.IsNullOrEmpty(searchstring)) {
                lstCombo = lstCombo.Where(o => o.nameCombo.ToLower().Contains(searchstring) || o.nameCombo.ToLower().ToString().Length == searchstring.ToLower().Length).ToList();
               
            }
            int pagesize = 16;
            int pagenum = (page ?? 1);
            return View(lstCombo.ToPagedList(pagenum, pagesize));
        }

        [HttpGet]
        public ActionResult LocComboS(string searchstring)
        {
            var lstCombo = db.Combo.Where(s => s.typeCombo == true).ToList();
            ViewBag.CurrentFilter = searchstring;
            if (!string.IsNullOrEmpty(searchstring)) {
                lstCombo = lstCombo.Where(o => o.nameCombo.Contains(searchstring) || o.nameCombo.ToString().Length == searchstring.Length).ToList();

            }
            return View(lstCombo);
        }
        public ActionResult Km(int? page,string searchstring)
        {
            var lstCombo = db.Combo
                      .Where(sp => sp.sale > 0)
                      .Join(db.ComboDetail, combo => combo.comboID, detail => detail.comboID, (combo, detail) => new { Combo = combo, Detail = detail })
                      .Where(n => n.Detail.sizeUP == false)
                      .Select(h => h.Combo)
                      .ToList();

            // Tìm kiếm theo tên sản phẩm
            ViewBag.CurrentFilter = searchstring;
            if (!string.IsNullOrEmpty(searchstring)) {
                lstCombo = lstCombo.Where(o => o.nameCombo.ToLower().IndexOf(searchstring.ToLower()) >= 0).ToList();
            }

            int pagesize = 16;
            int pagenum = (page ?? 1);
            return View(lstCombo.ToPagedList(pagenum, pagesize));
        }
    }
}