using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Alithea3.Models;

namespace Alithea3.Controllers
{
    [Author(RoleId = new[] { 1, 2})]
    public class UserAccountsController : Controller
    {
        private MyDbContext db = new MyDbContext();
       
        // GET: UserAccounts
        public ActionResult Index()
        {
            
            return View(db.UserAccounts.ToList());
        }

        // GET: UserAccounts/Details/5
        public ActionResult Details(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccount userAccount = db.UserAccounts.Find(id);
            if (userAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.listUserRoles = db.UserAccountRoles.Where(ur => ur.UserID == id).ToList();
            return View(userAccount);
        }

        // GET: UserAccounts/Edit/5
        [Author(RoleId = new []{1})]
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAccount userAccount = db.UserAccounts.Find(id);
            if (userAccount == null)
            {
                return HttpNotFound();
            }

            ViewBag.listRoles = db.Roles.ToList();
            ViewBag.listUserRoles = db.UserAccountRoles.Where(ur => ur.UserID == id).ToList();
            return View(userAccount);
        }

        // POST: UserAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Author(RoleId = new[] { 1 })]
        public ActionResult Edit(UserAccount userAccount, List<int> ints)
        {
           
            userAccount.UpdateAt = DateTime.Now;
            if (userAccount.Status == UserAccount.UserAccountStatus.Deleted)
            {
                userAccount.DeleteAt = DateTime.Now;
            }
            else
            {
                userAccount.DeleteAt = null;
            }
            userAccount.Display();

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (ints != null)
                        {
                            db.UserAccountRoles.RemoveRange(db.UserAccountRoles.Where(ur => ur.UserID == userAccount.UserID));

                            for (int i = 0; i < ints.Count; i++)
                            {
                                db.UserAccountRoles.Add(new UserAccountRole
                                {
                                    RoleId =  ints[i],
                                    UserID = userAccount.UserID
                                });
                            }

                            if (ints.Contains(1))
                            {
                                userAccount.admin = UserAccount.Decentralization.Admin;
                            }

                            db.SaveChanges();
                        }

                        db.Entry(userAccount).State = EntityState.Modified;
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Details/" + userAccount.UserID);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("loi: " + e.Message);
                    transaction.Rollback();
                }
            }

            TempData["Error"] = "Đã xảy ra lỗi";
            ViewBag.listRoles = db.Roles.ToList();
            ViewBag.listUserRoles = db.UserAccountRoles.Where(ur => ur.UserID == userAccount.UserID).ToList();
            return View(userAccount);
        }

        // POST: UserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Author(RoleId = new[] { 1 })]
        public ActionResult DeleteConfirmed(int id)
        {
            
            UserAccount userAccount = db.UserAccounts.Find(id);
            if (userAccount.admin == UserAccount.Decentralization.Admin)
            {
                TempData["Error"] = "Đây là tài khoản Admin, không xóa được";
                return RedirectToAction("Edit/" + userAccount.UserID);
            }

            db.UserAccounts.Remove(userAccount);
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
