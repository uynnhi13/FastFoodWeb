using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.MauThietKe;
using TMDT.Models;
using NLog;
using System.Collections;

namespace TMDT.Areas.Admin.Controllers
{
    public class ProductController : ControllerTemplateMethod
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();
        private ComboSingleton comboSingleton = ComboSingleton.instance;
        private updateCombo update;
        private Logger log;

        public ProductController()
        {
            comboSingleton.Init(db);
            log = LogManager.GetCurrentClassLogger();
            update = new updateCombo(db);
        }

        // GET: Admin/Product
        public ActionResult Index()
        {
            var allProduct = comboSingleton.lsCombo;

            var lsCombo = allProduct.Where(w => w.typeCombo == true);
            var lsProduct = allProduct.Where(w => w.typeCombo == false);
            var lsComboDetail = db.ComboDetail.ToList();
            var lsView = new List<Combo>();


            try {
                // tại product được lưu trong table Combo, nếu product đó có thể size up thì 1 Product = 2 Combo => lọc trường hợp trên và lấy 1 combo thôi 

                lsView.AddRange(lsCombo);
                foreach (var item in lsProduct) {
                    if (lsComboDetail.FirstOrDefault(f => f.comboID == item.comboID && f.sizeUP == false) != null)
                        lsView.Add(item);
                }

                log.Info("Get list in controller product");
            }
            catch (Exception ex) {
                log.Error("Error: " + ex);
                throw;
            }

            PrintInformation();

            return View(lsView);
        }

        public ActionResult locCombo(int loai)
        {
            // Lọc dữ liệu từ comboSingleton.lsCombo một lần
            var allProduct = comboSingleton.lsCombo;

            var lsCombo = allProduct.Where(w => w.typeCombo == true);
            var lsProduct = allProduct.Where(w => w.typeCombo == false);
            var lsComboDetail = db.ComboDetail.ToList();
            var lsView = new List<Combo>();


            try {
                // tại product được lưu trong table Combo, nếu product đó có thể size up thì 1 Product = 2 Combo => lọc trường hợp trên và lấy 1 combo thôi 

                lsView.AddRange(lsCombo);
                foreach (var item in lsProduct) {
                    if (lsComboDetail.FirstOrDefault(f => f.comboID == item.comboID && f.sizeUP == false) != null)
                        lsView.Add(item);
                }

                log.Info("Get list in controller product");
            }
            catch (Exception ex) {
                log.Error("Error: " + ex);
                throw;
            }

            var lsComboActive = lsView.Where(c => c.typeCombo == true && c.hiden == true).ToList();
            var lsComboInactive = lsView.Where(c => c.typeCombo == true && c.hiden == false).ToList();
            var lsComboal = lsView.Where(c => c.typeCombo= true).ToList();

            var listReturn = new List<Combo>();

            // Chọn danh sách dựa trên biến loai
            switch (loai) {
                case 0:
                    if (lsCombo != null && lsComboal.Count > 0) {
                        listReturn.Clear();
                        listReturn.AddRange(lsCombo);
                    }
                    else {
                        listReturn = new List<Combo>();
                    }
                    break;
                case 1:
                    if (lsComboActive != null && lsComboActive.Count > 0) {
                        listReturn.Clear();
                        listReturn.AddRange(lsComboActive);
                    }
                    else {
                        listReturn = new List<Combo>();
                    }
                    break;
                case 2:
                    if(lsComboInactive != null && lsComboInactive.Count > 0) {
                        listReturn.Clear();
                        listReturn.AddRange(lsComboInactive);
                    }
                    else {
                        listReturn = new List<Combo>();
                    }
                    break;
                default:
                    break;
            }

            return View("Index",listReturn);
        }

        [HttpPost]
        public JsonResult getCombo(int cateID)
        {

            var lsitemCombo = new List<itemProduct>();
            lsitemCombo = LayCombo();

            if (lsitemCombo.FirstOrDefault(f => f.producID == cateID) != null) {

                lsitemCombo.Remove(lsitemCombo.FirstOrDefault(f => f.producID == cateID));
                Session["combo"] = lsitemCombo;

            }

            return Json(lsitemCombo);
        }

        [HttpPost]
        public JsonResult getProduct(int comboID)
        {
            var list = comboSingleton.lsCombo;
            var item = list.FirstOrDefault(f => f.comboID == comboID);

            //kiểm tra item là combo hay product 

            return Json(new { comboID = item.comboID, nameCombo = item.nameCombo, price = item.price, item.image });
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.typeID = new SelectList(db.Category, "typeID", "nameType");
            ViewBag.lstCategory = db.Category;
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for     
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cateID,name,price,image,typeID,priceUp")] Product product, HttpPostedFileBase HinhAnh)
        {
            try {
                if ((HinhAnh != null && HinhAnh.ContentLength > 0) && ModelState.IsValid) {
                    // luu file
                    string Noiluu = Server.MapPath("/Images/Product/");
                    String PathImg = Noiluu + HinhAnh.FileName;
                    HinhAnh.SaveAs(PathImg);

                    // Màu sắc điện thoại

                    string img = "/Images/Product/" + (string)HinhAnh.FileName;

                    db.AddProductAndCombo(product.name, product.price, img, product.typeID, product.priceUp);
                    db.SaveChanges();

                    comboSingleton.Update(db);
                    ViewBag.notification = true;
                    return View("Create");
                }
                else {
                    ViewBag.notification = false;
                    return View("Create");
                }
            }
            catch (Exception e) {
                ViewBag.notification = false;
                return View("Create");
            }

        }

        public ActionResult Edit(int? comboID)
        {

            if (comboID == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = comboSingleton.lsCombo.FirstOrDefault(f => f.comboID == comboID);
            var lsComboDetail = db.ComboDetail.ToList();

            if (product == null) {
                return HttpNotFound();
            }
            else {

                if (product.typeCombo) {
                    return RedirectToAction("EditCombo", new { comboID = product.comboID});
                }
                else {
                    var cateID = lsComboDetail.FirstOrDefault(f => f.comboID == product.comboID).cateID;
                    return RedirectToAction("EditProduct", new {cateID = cateID });
                }
            }
        }

        // GET: Admin/Product/Edit/5
        public ActionResult EditCombo(int? comboID)
        {
            try {

                var product = comboSingleton.lsCombo.FirstOrDefault(f => f.comboID == comboID);

                if (product != null) {
                    return View(product);
                }

                return View();
            }
            catch (Exception) {

                return RedirectToAction("index");
            }
        }
        [HttpPost]
        public ActionResult EditCombo([Bind(Include = "comboID,nameCombo,sale, hiden")] Combo _combo, HttpPostedFileBase HinhAnh)
        {
            try {
                var combo = db.Combo.FirstOrDefault(f => f.comboID == _combo.comboID);
                var lsProduct = db.Product.ToList();
                if (HinhAnh != null && HinhAnh.ContentLength > 0) {
                    // luu file
                    string Noiluu = Server.MapPath("/Images/Combo/");
                    String PathImg = Noiluu + HinhAnh.FileName;
                    HinhAnh.SaveAs(PathImg);

                    // Màu sắc điện thoại

                    string img = "/Images/Combo/" + (string)HinhAnh.FileName;

                    

                    var lsitemCombo = db.ComboDetail.Where(f => f.comboID == combo.comboID).ToList();

                    //tong tien
                    decimal sumPrice = 0;

                    foreach (var item in lsitemCombo) {
                        var product = lsProduct.FirstOrDefault(f => f.cateID == item.cateID);
                        if (item.sizeUP == false) sumPrice += product.price * item.quantity;
                        else sumPrice += (product.price + product.priceUp) * item.quantity;
                    }
                    if (!img.Contains(combo.image)) {
                        System.IO.File.Delete(combo.image);
                        combo.image = img;
                    }
                    combo.nameCombo = _combo.nameCombo;
                    combo.hiden = _combo.hiden;
                    combo.sale = _combo.sale;
                    combo.price = sumPrice * (100 - combo.sale) / 100;
                    db.SaveChanges();
                    comboSingleton.Update(db);
                    TempData["EditSuccess"] = true;

                    return RedirectToAction("Index");
                }
                else {
                    

                    var lsitemCombo = db.ComboDetail.Where(f => f.comboID == combo.comboID).ToList();

                    //tong tien
                    decimal sumPrice = 0;

                    foreach (var item in lsitemCombo) {
                        var product = lsProduct.FirstOrDefault(f => f.cateID == item.cateID);
                        if (item.sizeUP == false) sumPrice += product.price * item.quantity;
                        else sumPrice += (product.price + product.priceUp) * item.quantity;
                    }

                    combo.nameCombo = _combo.nameCombo;
                    combo.hiden = _combo.hiden;
                    combo.sale = _combo.sale;
                    combo.price = sumPrice * (100 - combo.sale) / 100;
                    db.SaveChanges();
                    comboSingleton.Update(db);
                    TempData["EditSuccess"] = true;

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e) {
                
                TempData["EditSuccess"] = false;

                return RedirectToAction("Index", "Product");
            }
        }
        public ActionResult EditProduct(int? cateID)
        {
            try {
                var proItem = db.Product.FirstOrDefault(f => f.cateID == cateID);


                if (proItem != null) {
                    return View(proItem);
                }

                return View();
            }
            catch (Exception) {

                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public ActionResult EditProduct([Bind(Include = "cateID,name,price,image,typeID,priceUp")] Product product, HttpPostedFileBase HinhAnh)
        {
            try {
                var itemProduct = db.Product.FirstOrDefault(p => p.cateID == product.cateID);
                if ((HinhAnh != null && HinhAnh.ContentLength > 0) && ModelState.IsValid && itemProduct != null) {

                    // luu file
                    string Noiluu = Server.MapPath("/Images/Product/");
                    String PathImg = Noiluu + HinhAnh.FileName;
                    HinhAnh.SaveAs(PathImg);

                    // Màu sắc điện thoại

                    string img = "/Images/Product/" + (string)HinhAnh.FileName;

                    if (!img.Contains(itemProduct.image)) {
                        System.IO.File.Delete(itemProduct.image);
                        itemProduct.image = img;
                    }

                    itemProduct.name = product.name;
                    itemProduct.price = product.price;
                    
                    
                    if(product.priceUp != 0) {
                        itemProduct.priceUp = product.priceUp;
                    }
                    itemProduct.typeID = product.typeID;
                    db.SaveChanges();
                    
                    udpateCom(db, itemProduct);

                    comboSingleton.Update(db);
                    TempData["EditSuccess"] = true;
                    return RedirectToAction("index");
                }
                else {

                    itemProduct.name = product.name;
                    itemProduct.price = product.price;

                    if (product.priceUp != null && product.priceUp > 0) {
                        itemProduct.priceUp = product.priceUp;
                    }

                    itemProduct.typeID = product.typeID;
                    db.SaveChanges();

                    udpateCom(db, itemProduct);

                    comboSingleton.Update(db);
                    TempData["EditSuccess"] = true;

                    return RedirectToAction("index");
                }
            }
            catch (Exception e) {
                ViewBag.notification = false;
                return RedirectToAction("index");
            }
        }

        private void udpateCom(TMDTThucAnNhanhEntities db, Product itemProduct)
        {
            var lsComboID = update.getlistComboID(itemProduct);
            // sản phẩm k có size up => combo chỉ có 1 sản phẩm nên count = 1
            if (lsComboID.Count == 1) {
                var comboID = lsComboID[0];

                var item = db.Combo.FirstOrDefault(f => f.comboID == comboID);

                item.nameCombo = (itemProduct.name.Contains(item.nameCombo)) ? item.nameCombo : itemProduct.name;
                item.price = (itemProduct.price == (item.price * 100 / (100 - item.sale))) ? item.price : itemProduct.price;
                item.image = (item.image.Contains(itemProduct.image)) ? item.image : itemProduct.image;
                db.SaveChanges();
            }
            // sản phẩm k có size up => combo có 2 sản phẩm nên count = 2
            if (lsComboID.Count == 2) {
                for (int i = 0; i < lsComboID.Count; i++) {
                    var comboID = lsComboID[i];

                    var item = db.Combo.FirstOrDefault(f => f.comboID == comboID);

                    item.nameCombo = (itemProduct.name.Contains(item.nameCombo)) ? item.nameCombo : itemProduct.name;
                    item.price = (itemProduct.price == (item.price * 100 / (100 - item.sale))) ? item.price : itemProduct.price;
                    item.image = (item.image.Contains(itemProduct.image)) ? item.image : itemProduct.image;
                    db.SaveChanges();
                }
            }
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cateID,name,price,image,typeID,priceUp")] Product product)
        {
            if (ModelState.IsValid) {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                comboSingleton.Update(db);
                return RedirectToAction("Index");
            }
            comboSingleton.Update(db);

            ViewBag.typeID = new SelectList(db.Category, "typeID", "nameType", product.typeID);
            return View(product);
        }

        public ActionResult CreateComboDetailt()
        {
            bool test = false;

            if (db.ComboDetail.ToList().Count > 0) {
                foreach (var item in db.Product) {
                    if (db.Recipe.FirstOrDefault(f => f.cateID == item.cateID) != null) {
                        test = true;
                        break;
                    }
                }
            }

            ViewBag.test = test;

            var lsProduct = db.Product.ToList();

            ViewBag.lstProduct = lsProduct;
            ViewBag.lstComboDetail = new List<Product>();

            return View(lsProduct);
        }

        [HttpPost]
        public JsonResult deleteProduct(int cateID)
        {

            var lsitemCombo = new List<itemProduct>();
            lsitemCombo = LayCombo();

            if (lsitemCombo.FirstOrDefault(f => f.producID == cateID) != null) {

                lsitemCombo.Remove(lsitemCombo.FirstOrDefault(f => f.producID == cateID));
                Session["combo"] = lsitemCombo;
            }

            return Json(lsitemCombo);
        }

        [HttpPost]
        public JsonResult uppSize(int cateID, bool sizeUp)
        {
            var lsitemCombo = new List<itemProduct>();
            lsitemCombo = LayCombo();

            if (lsitemCombo.FirstOrDefault(f => f.producID == cateID) != null) {

                lsitemCombo.FirstOrDefault(f => f.producID == cateID).upSize = sizeUp;
                Session["combo"] = lsitemCombo;

            }

            return Json(lsitemCombo);
        }

        [HttpPost]
        public JsonResult CrCombo(int cateID, string name, int quantity, bool sizeUp)
        {
            itemProduct ing = new itemProduct(cateID, name, quantity, sizeUp);
            if (quantity != 0) {
                addCombo(ing);
            }
            else {
                deleteCombo(cateID);
            }

            var lsitemCombo = new List<itemProduct>();
            lsitemCombo = LayCombo();

            return Json(lsitemCombo);
        }

        public List<itemProduct> LayCombo()
        {
            List<itemProduct> lstCombo = Session["combo"] as List<itemProduct>;

            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (lstCombo == null) {
                lstCombo = new List<itemProduct>();
                Session["combo"] = lstCombo;
            }
            return lstCombo;
        }
        public void addCombo(itemProduct _itemCombo)
        {
            List<itemProduct> lstCombo = LayCombo();
            var test = new itemProduct();
            test = lstCombo.FirstOrDefault(f => f.producID == _itemCombo.producID);

            if (test == null) lstCombo.Add(_itemCombo);
            else {
                for (int i = 0; i < lstCombo.Count; i++)
                    if (lstCombo[i].producID == _itemCombo.producID) {
                        lstCombo[i].quantity = _itemCombo.quantity;
                    }
            }

            Session["combo"] = lstCombo;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (lstCombo == null) {
                lstCombo = new List<itemProduct>();
                Session["combo"] = lstCombo;
            }
        }

        public void deleteCombo(int id)
        {
            List<itemProduct> lstCombo = LayCombo();
            var ing = lstCombo.FirstOrDefault(f => f.producID == id);
            if (ing != null) lstCombo.Remove(ing);
            Session["combo"] = lstCombo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public override void PrintRouter()
        {
            log.Info($@"{GetType().Name}
            GET: Admin/Product/index
            GET: Admin/Product/locCombo
            POST: Admin/Product/getCombo
            POST: Admin/Product/getProduct
            GET: Admin/Product/Create
            POST: Admin/Product/Create
            GET: Admin/Product/Edit?comboID=?
            POST: Admin/Product/Edit
            GET: Admin/Product/CreateComboDetailt
            POST: Admin/Product/deleteProduct
            POST: Admin/Product/uppSize
            POST: Admin/Product/CrCombo
            ");
        }

        public override void PrintDIs()
        {
            log.Info($@"
            Dependencies: 
            TMDTThucAnNhanhEntities db, ComboSingleton comboSingleton, Logger log
            ");
        }

        [HttpPost]
        public ActionResult addRecipe(int cateID, int ingID, double quantity)
        {
            var item = db.Recipe.FirstOrDefault(f => f.cateID == cateID && f.ingID == ingID);

            if (item != null) {
                item.quantity = quantity;
                db.SaveChanges();
                return Json(new { cateID = cateID, ingID = ingID, quantity = quantity, message = "Data updated successfully!" });
            }
            else {
                // Xử lý khi không tìm thấy đối tượng cần cập nhật
                return Json(new { message = "Item not found or unable to update!" });
            }
        }

        [HttpPost]
        public ActionResult addComboDetail(int comboID, int cateID, int quantity, bool sizeUP)
        {
            var item = db.ComboDetail.FirstOrDefault(f => f.cateID == cateID && f.comboID == comboID);

            if (item != null) {
                item.quantity = quantity;
                db.SaveChanges();
                return Json(new { comboID = comboID, cateID = cateID, quantity = quantity, sizeUP = sizeUP, message = "Data updated successfully!" });
            }
            else {
                // Xử lý khi không tìm thấy đối tượng cần cập nhật
                return Json(new { message = "Item not found or unable to update!" });
            }
        }
    }
}
