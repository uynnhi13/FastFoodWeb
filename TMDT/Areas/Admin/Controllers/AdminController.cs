using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
            //session = null thi chuyen den trang dang nhap
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
            var tk = database.Employees.Where(s => s.Email == user && s.password == password).FirstOrDefault();

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
            if (em == null) {
                return HttpNotFound();
            }
            database.Employees.Remove(em);
            database.SaveChanges();
            return RedirectToAction("Account", "Admin");
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
        public ActionResult DetailsKH(string id)
        {
            var kh = database.User.FirstOrDefault(s => s.numberPhone == id);
            return View(kh);
        }
       
        public ActionResult EditKHang(string id)
        {
             var em = database.User.FirstOrDefault(s => s.numberPhone == id);
                if (em == null) {
                    return HttpNotFound();
                }
                return View(em);
            
            
        }
        [HttpPost]
       public ActionResult EditKHang(User us)
        {
            if (ModelState.IsValid) {
                var a = database.User.FirstOrDefault(f => f.numberPhone == us.numberPhone);

                a.fullName = us.fullName;
                a.gmail = us.gmail;
                a.password = us.password;
                a.numberPhone = us.numberPhone;

                database.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("QlyKH");
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

            List<SelectListItem> pt = new List<SelectListItem>()
             {
                new SelectListItem { Text = "Phương thức", Value = "0"},
                new SelectListItem { Text = "Tại cửa hàng", Value = "1"},
                new SelectListItem { Text = "VNPay", Value = "2"}
            };

            ViewBag.PaymentMethods = pt;

            return View(donhang);
        }

        [HttpGet]
        public ActionResult DonHang(DateTime? startdate, DateTime? enddate, int? conditionID, Order ord, string selectedPaymentMethod)
        {
            var orders = database.Order.ToList();

            ViewBag.conditionID = new SelectList(database.Condition.ToList(), "conditionID", "nameCon");
           
            if (startdate != null && enddate != null) {
                enddate = enddate.Value.AddDays(1).AddTicks(-1);
                orders = orders.Where(o => o.datetime >= startdate && o.datetime <= enddate).ToList();
            }
            if(conditionID != 0 && conditionID != null) {
                orders = orders.Where(o => o.conditionID == conditionID).ToList();
               
                
            }
            List<SelectListItem> pt = new List<SelectListItem>()
             {
                new SelectListItem { Text = "Phương thức", Value = "0"},
                new SelectListItem { Text = "Tại cửa hàng", Value = "1"},
                new SelectListItem { Text = "VNPay", Value = "2"}
            };

           
            ViewBag.PaymentMethods = pt;

            // Lọc danh sách orders dựa trên phương thức thanh toán được chọn
            if (selectedPaymentMethod == "1") {
                orders = orders.Where(o => o.TypePayment == 1).ToList();
            }
            if (selectedPaymentMethod == "2") {
                orders = orders.Where(o => o.TypePayment == 2).ToList();
            }
            var donhang = orders.ToList();
            return View(orders);

        }
        


        public ActionResult XacNhanDH(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order donhang = database.Order.Find(id);

            // don hang dc tim thay
            if (donhang != null) {
                // chinh trang thai don hang
                donhang.conditionID = 2;
                database.SaveChanges();
                if (donhang.employeeID == null) {
                    var searchU = (Employees)Session["user"];
                    donhang.employeeID = searchU.EmployeeID;

                }

                //lọc danh sách id sản phẩm kèm số lượng sản phẩm
                var lsOrder = database.OrderDetail.Where(o => o.orderID == id).ToList();

                // chua id combo and so luong trong order Detail
                List<IngredientProduct> lsThanhPhan = new List<IngredientProduct>();

                foreach (var productOrder in lsOrder) {
                    var _thanhphan = new IngredientProduct(productOrder.comboID, productOrder.quantity);
                    setIngredientProduct(lsThanhPhan, _thanhphan, true);
                }

                //neu co sp la combo thi chuyen sang product * tạm *
                List<IngredientProduct> lsQuantityProduct = new List<IngredientProduct>();

                //lọc danh sách conbo and non combo
                var lsAllProductInDB = database.Combo.ToList();
                List<Combo> lsAllProduct = lsAllProductInDB.Where(w => lsThanhPhan.Any(a => a.id == w.comboID)).ToList();
                List<ComboDetail> lsAllComboDetail = database.ComboDetail.ToList();

                var lsCombo = new List<Combo>();

                foreach (var item in lsAllProduct) {
                    if (item.typeCombo) {
                        lsCombo.Add(item);
                    }
                    else {
                        var chaneThanhPhan = lsThanhPhan.FirstOrDefault(a => a.id == item.comboID);
                        var getIDProduct = lsAllComboDetail.FirstOrDefault(f => f.comboID == item.comboID);

                        setIngredientProduct(lsQuantityProduct, new IngredientProduct(getIDProduct.cateID, chaneThanhPhan.quantity), true);
                    }
                }


                //list product in combo
                var lsQuantityProInCombo = new List<ComboDetail>();

                foreach (var item in lsCombo) {
                    var chaneThanhPhan = lsThanhPhan.FirstOrDefault(a => a.id == item.comboID);
                    var lsProductInCombo = lsAllComboDetail.Where(f => f.comboID == item.comboID).ToList();

                    var quantity = chaneThanhPhan.quantity;

                    for (int i = 0; i < lsProductInCombo.Count; i++)
                        lsProductInCombo[i].quantity *= (int)quantity;

                    lsQuantityProInCombo.AddRange(lsProductInCombo);
                }
                //=> lsQuantityProInCombo xử lý trùng lặp
                var lsQuantityProInComboGrouped = lsQuantityProInCombo
                                                    .GroupBy(x => x.cateID)
                                                    .Select(g => new ComboDetail {
                                                        cateID = g.Key,
                                                        quantity = g.Sum(x => x.quantity)
                                                    }).ToList();

                foreach (var item in lsQuantityProInComboGrouped) {
                    setIngredientProduct(lsQuantityProduct, new IngredientProduct(item.cateID, item.quantity), true);
                }
                //=> lsQuantityProduct chua ls id product and so luong trong detail order

                var lsIngredientInProduct = new List<Recipe>();
                foreach (var item in lsQuantityProduct) {
                    var lsRecipe = database.Recipe.Where(w => w.cateID == item.id).ToList();

                    for (int i = 0; i < lsRecipe.Count; i++)
                        lsRecipe[i].quantity *= (int)item.quantity;

                    lsIngredientInProduct.AddRange(lsRecipe);
                }

                var lsIngredientInProductGrouped = lsIngredientInProduct
                                                    .GroupBy(x => x.ingID)
                                                    .Select(g => new Recipe {
                                                        ingID = g.Key,
                                                        quantity = g.Sum(x => x.quantity)
                                                    }).ToList();

                foreach(var item in lsIngredientInProductGrouped) {
                    var itemIngredient = database.Ingredient.FirstOrDefault(f => f.ingID == item.ingID);
                    itemIngredient.quantity -= item.quantity;
                    database.SaveChanges();
                }

            }

            if (donhang == null) {
                return HttpNotFound();
            }

            return RedirectToAction("DonHang", "Admin");
        }

        public void setIngredientProduct(List<IngredientProduct> lsThanhPhan, IngredientProduct item, bool calculation)
        {
            //if calculation == true => phep cong
            //else phep tru
            bool find = true;

            for (int i = 0; i < lsThanhPhan.Count; i++) {
                if (lsThanhPhan[i].id == item.id) {
                    if (calculation)
                        lsThanhPhan[i].quantity += item.quantity;
                    else
                        lsThanhPhan[i].quantity *= item.quantity;

                    find = false;
                    break;
                }
            }

            if(find)
                lsThanhPhan.Add(item);
        }

        public void ListToList(List<IngredientProduct> lsThanhPhan, List<IngredientProduct> lsAdd)
        {
        }

        public ActionResult Dagiao(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order donhang = database.Order.Find(id);

            donhang.conditionID = 3;
            //if (donhang.employeeID == null) {
            //    var searchU = (Employees)Session["user"];
            //    donhang.employeeID = searchU.EmployeeID;

            //}

            database.SaveChanges();
            if (donhang == null) {
                return HttpNotFound();
            }

            return RedirectToAction("DonHang", "Admin");
        }

        public ActionResult DetailsDH(int id)
        {
            var dh = database.OrderDetail.Where(s => s.orderID == id);

            var lsProduct = new List<Combo>();
            var lsCombo = new List<Combo>();

            foreach (var item in dh) {
                var lsCombotam = database.Combo.FirstOrDefault(l => l.comboID == item.comboID);

                if (lsCombotam.typeCombo == false) {
                    var combo = database.Combo.FirstOrDefault(f => f.comboID == item.comboID);
                    lsProduct.Add(combo);
                }
                else {
                    var combo = database.Combo.FirstOrDefault(f => f.comboID == item.comboID);
                    lsCombo.Add(combo);
                }
            }
            ViewBag.LsProduct = lsProduct;
            ViewBag.LsCombo = lsCombo;



            return View(dh);

        }
        public ActionResult Mypro()
        {
            var searchU = (Employees)Session["user"];
            if (searchU != null) {
                var emUser = database.Employees.FirstOrDefault(s => s.EmployeeID == searchU.EmployeeID);
                return View(emUser);
            }
            else {
                return View();
            }

        }
        public ActionResult EditMypro()
        {
            var searchU = (Employees)Session["user"];
            if (searchU != null) {
                var emUser = database.Employees.FirstOrDefault(s => s.EmployeeID == searchU.EmployeeID);
                return View(emUser);
            }
            else {
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditMypro(Employees em)
        {
            if (ModelState.IsValid) {
                var a = database.Employees.FirstOrDefault(f => f.EmployeeID == em.EmployeeID);

                a.FirstName = em.FirstName;
                a.LastName = em.LastName;
                a.numberPhone = em.numberPhone;
                a.Email = em.Email;

                database.SaveChanges();// LUU THAY DOI
            }
            return RedirectToAction("MyPro");
        }
       

        public ActionResult nameLogin()
        {
            var searchU = new Employees();
            searchU = (Employees)Session["user"];
            return PartialView(searchU);
        }
        [HttpGet]
        public ActionResult QlyDanhGia(string selected)
        {
            var orders = database.Order.ToList();
            List<SelectListItem> star = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Sắp xếp", Value = "0"},
                new SelectListItem { Text = "1 Sao", Value = "1"},
                new SelectListItem { Text = "2 Sao", Value = "2"},
                new SelectListItem { Text = "3 Sao", Value = "3"},
                new SelectListItem { Text = "4 Sao", Value = "4"},
                new SelectListItem { Text = "5 Sao", Value = "5"}
            };

            ViewBag.Star = star;

            // Lọc danh sách orders dựa trên phương thức thanh toán được chọn
            if (selected== "1") {
                orders = orders.Where(o => o.star == 1).ToList();
            }
            if (selected == "2") {
                orders = orders.Where(o => o.star == 2).ToList();
            }
            if (selected == "3") {
                orders = orders.Where(o => o.star == 3).ToList();
            }
            if (selected == "4") {
                orders = orders.Where(o => o.star == 4).ToList();
            }
            if (selected == "5") {
                orders = orders.Where(o => o.star == 5).ToList();
            }
            var donhang = orders.ToList();
            return View(orders);


            /*// Tạo danh sách sao đánh giá
            List<SelectListItem> stars = new List<SelectListItem>()
            {
                new SelectListItem { Text = "1 Sao", Value = "1"},
                new SelectListItem { Text = "2 Sao", Value = "2"},
                new SelectListItem { Text = "3 Sao", Value = "3"},
                new SelectListItem { Text = "4 Sao", Value = "4"},
                new SelectListItem { Text = "5 Sao", Value = "5"}
            };
            if (stars != null) {
                ViewBag.StarList = stars;
                return View();
            }
            var commt = database.Order.FirstOrDefault(o => o.star == ord.star);
            return View(commt);*/
        }
        [HttpGet]
        public ActionResult ThongKe(DateTime? startdate, DateTime? enddate, int? ConditionID)
        {
            var orders = database.Order.ToList();

            ViewBag.conditionID = new SelectList(database.Condition.ToList(), "conditionID", "nameCon");
            if (startdate != null && enddate != null ) {
                enddate = enddate.Value.AddDays(1).AddTicks(-1);
                orders = orders.Where(o => o.datetime >= startdate && o.datetime <= enddate).ToList();

                decimal Tong = 0;
                foreach (var item in orders) {
                    if (item.conditionID == 3) {
                        Tong += item.total;
                    }
                    
                }
                ViewBag.TongTien = Tong;
                if (ConditionID!=0) {
                    var chuaxn = 0;
                    chuaxn = orders.Count(o => o.conditionID == 1);
                    ViewBag.Chuaxn = chuaxn;

                    var daxn = 0;
                    daxn = orders.Count(o => o.conditionID == 2);
                    ViewBag.Daxn = daxn;

                    var dagiao = 0;
                    dagiao = orders.Count(o => o.conditionID == 3);
                    ViewBag.Dagiao = dagiao;
                   
                }

            }
            
            
            return View(orders);

        }

    }


}
