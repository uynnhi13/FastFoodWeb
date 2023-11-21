using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;


namespace TMDT.Areas.Admin.Controllers
{
    public class IngredientsController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: Admin/Ingredients
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNum = 1;

            var ingredient = db.Ingredient.Include(i => i.Unit);
            return View(ingredient);
        }

        public ActionResult listIngredient(int? id)
        {
            var ingredient = db.Recipe.Where(w => w.cateID == id);
            return View("Index", ingredient);
        }

        // GET: Admin/Ingredients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null) {
                return HttpNotFound();
            }
            return View(ingredient);
        }

        // GET: Admin/Ingredients/Create
        public ActionResult Create()
        {
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU");
            return View();
        }

        // POST: Admin/Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ingID,ingName,quantity,unitID,quantityMin,arrivalDate")] Ingredient ingredient)
        {
            if (ingredient is null) {
                throw new ArgumentNullException(nameof(ingredient));
            }

            if (ModelState.IsValid) {
                if (ingredient.quantity == null) ingredient.quantity = 0;
                if (ingredient.quantityMin == null) ingredient.quantityMin = 0;
                ingredient.arrivalDate = DateTime.Now;

                Ingredient ingre = new Ingredient();
                ingre = ingredient;

                db.Ingredient.Add(ingre);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU", ingredient.unitID);
            return View(ingredient);
        }

        public ActionResult CreateRecipe()
        {
            bool test = false;

            if (db.Recipe.ToList().Count > 0) {
                foreach (var item in db.Product) {
                    if (db.Recipe.FirstOrDefault(f => f.cateID == item.cateID) != null) {
                        test = true;
                        break;
                    }
                }
            }

            List<ingre> lstingre = new List<ingre>();
            Session["ingre"] = lstingre;

            ViewBag.test = test;
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU");
            var ingredient = db.Ingredient.Include(i => i.Unit);
            return View(ingredient);
        }
        public ActionResult addRecipeInProduct()
        {
            var lsIngredient = new List<ingre>();
            lsIngredient = LayIngre();
            return View(lsIngredient);
        }

        [HttpPost]
        public ActionResult addRecipeInProduct(string name, List<ingre> lsIngredient)
        {

            return RedirectToAction("index");
        }
        public ActionResult createRecipeInProduct()
        {
            var lsIngredient = new List<ingre>();
            lsIngredient = LayIngre();

            if (lsIngredient.Count == 0) {
                ViewBag.message = "Chưa chọn món";
                return RedirectToAction("CreateRecipe");
            }

            return View(lsIngredient);
        }

        [HttpPost]
        public ActionResult createRecipeInProduct([Bind(Include = "cateID,name,price,typeID,priceUp")] Product product, HttpPostedFileBase HinhAnh)
        {
            try {
                if ((HinhAnh != null && HinhAnh.ContentLength > 0) && ModelState.IsValid) {
                    // luu file
                    string Noiluu = Server.MapPath("/Images/Product/");
                    String PathImg = Noiluu + HinhAnh.FileName;
                    HinhAnh.SaveAs(PathImg);

                    // Màu sắc điện thoại

                    string img = "/Images/Product/" + (string)HinhAnh.FileName;

                    List<ingre> lsDeci = LayIngre();

                    db.createRecipeDB(product.name, product.price, product.priceUp, img, product.typeID, lsDeci);
                    db.SaveChanges();



                    ViewBag.notification = true;
                    return RedirectToAction("index");
                }
                else {
                    ViewBag.notification = false;
                    return RedirectToAction("CreateRecipe");
                }
            }
            catch (Exception e) {
                ViewBag.notification = false;
                return RedirectToAction("CreateRecipe");
            }

        }


        [HttpPost]
        public JsonResult CrRecipe(int id, double quantity)
        {
            ingre ing = new ingre(id, quantity);
            if (quantity != 0) {
                addIngre(ing);
            }
            else {
                deleteIngre(id);
            }

            List<ingre> list = LayIngre();
            var lsIngredient = new List<ingre>();
            lsIngredient = LayIngre();

            return Json(list);
        }


        public List<ingre> LayIngre()
        {
            List<ingre> lstingre = Session["ingre"] as List<ingre>;

            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (lstingre == null) {
                lstingre = new List<ingre>();
                Session["ingre"] = lstingre;
            }
            return lstingre;
        }
        public void addIngre(ingre _ingre)
        {
            List<ingre> lstingre = LayIngre();
            var test = new ingre();
            test = lstingre.FirstOrDefault(f => f.id == _ingre.id);

            if (test == null) lstingre.Add(_ingre);
            else {
                for (int i = 0; i < lstingre.Count; i++)
                    if (lstingre[i].id == _ingre.id) {
                        lstingre[i].quantity = _ingre.quantity;
                    }
            }

            Session["ingre"] = lstingre;
            //Nếu giỏ hàng chưa tồn tại thì tạo mới và đưa vào session
            if (lstingre == null) {
                lstingre = new List<ingre>();
                Session["ingre"] = lstingre;
            }

        }
        public void deleteIngre(int id)
        {
            List<ingre> lstingre = LayIngre();
            var ing = lstingre.FirstOrDefault(f => f.id == id);
            if (ing != null) lstingre.Remove(ing);
            Session["ingre"] = lstingre;
        }

        // GET: Admin/Ingredients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingredient = db.Ingredient.Find(id);
            if (ingredient == null) {
                return HttpNotFound();
            }
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU", ingredient.unitID);
            return View(ingredient);
        }

        // POST: Admin/Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ingID,ingName,quantity,unitID,quantityMin,arrivalDate")] Ingredient ingredient)
        {
            if (ModelState.IsValid) {
                db.Entry(ingredient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.unitID = new SelectList(db.Unit, "unitID", "nameU", ingredient.unitID);
            return View(ingredient);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
