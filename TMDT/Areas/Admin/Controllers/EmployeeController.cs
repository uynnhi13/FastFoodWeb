using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Areas.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        private TMDTThucAnNhanhEntities db = new TMDTThucAnNhanhEntities();

        // GET: Admin/Employee
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.location).Include(e => e.Position);
            return View(employees.ToList());
        }

        // GET: Admin/Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // GET: Admin/Employee/Create
        public ActionResult Create()
        {
            ViewBag.locationID = new SelectList(db.location, "locationID", "StreetAddress");
            ViewBag.positionID = new SelectList(db.Position, "PositionID", "posName");
            return View();
        }

        // POST: Admin/Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,FirstName,LastName,Email,positionID,imgEP,numberPhone,locationID,password,roleUser")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.locationID = new SelectList(db.location, "locationID", "StreetAddress", employees.locationID);
            ViewBag.positionID = new SelectList(db.Position, "PositionID", "posName", employees.positionID);
            return View(employees);
        }

        // GET: Admin/Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            ViewBag.locationID = new SelectList(db.location, "locationID", "StreetAddress", employees.locationID);
            ViewBag.positionID = new SelectList(db.Position, "PositionID", "posName", employees.positionID);
            return View(employees);
        }

        // POST: Admin/Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,FirstName,LastName,Email,positionID,imgEP,numberPhone,locationID,password,roleUser")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.locationID = new SelectList(db.location, "locationID", "StreetAddress", employees.locationID);
            ViewBag.positionID = new SelectList(db.Position, "PositionID", "posName", employees.positionID);
            return View(employees);
        }

        // GET: Admin/Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Admin/Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employees employees = db.Employees.Find(id);
            db.Employees.Remove(employees);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
