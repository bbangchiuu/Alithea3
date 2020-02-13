using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Alithea3.Models;
using Attribute = Alithea3.Models.Attribute;

namespace Alithea3.Controllers
{
    [Author(RoleId = new[] {1})]
    public class AttributesController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Attributes
        public ActionResult Index()
        {
            var attributes = db.Attributes.Include(a => a.Color).Include(a => a.Product);
            return View(attributes.ToList());
        }

        // GET: Attributes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attribute attribute = db.Attributes.Find(id);
            if (attribute == null)
            {
                return HttpNotFound();
            }
            return View(attribute);
        }

        // GET: Attributes/Create
        public ActionResult Create()
        {
            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "NameColor");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "RoleNumber");
            return View();
        }

        // POST: Attributes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ProductID,ProductImage,ColorID,UnitPrice,Quantity")] Attribute attribute)
        {
            if (ModelState.IsValid)
            {
                db.Attributes.Add(attribute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "NameColor", attribute.ColorID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "RoleNumber", attribute.ProductID);
            return View(attribute);
        }

        // GET: Attributes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attribute attribute = db.Attributes.Find(id);
            if (attribute == null)
            {
                return HttpNotFound();
            }
            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "NameColor", attribute.ColorID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "RoleNumber", attribute.ProductID);
            return View(attribute);
        }

        // POST: Attributes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProductID,ProductImage,ColorID,UnitPrice,Quantity")] Attribute attribute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attribute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ColorID = new SelectList(db.Colors, "ColorID", "NameColor", attribute.ColorID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "RoleNumber", attribute.ProductID);
            return View(attribute);
        }

        // GET: Attributes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attribute attribute = db.Attributes.Find(id);
            if (attribute == null)
            {
                return HttpNotFound();
            }
            return View(attribute);
        }

        // POST: Attributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attribute attribute = db.Attributes.Find(id);
            db.Attributes.Remove(attribute);
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
