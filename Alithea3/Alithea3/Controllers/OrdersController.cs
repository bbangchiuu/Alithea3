using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Alithea3.Controllers.Service.OrderManager;
using Alithea3.Models;
using Microsoft.Ajax.Utilities;

namespace Alithea3.Controllers
{
    [Author(RoleId = new[] { 1, 2 })]
    public class OrdersController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private OrderService _orderService = new OrderService();
     
        // GET: Orders
        public ActionResult Index(int? page, int? limit, string start, string end)
        {
           
            var hashtable = _orderService.listPagination(page, limit, start, end);
            var orders = hashtable["listOrder"] as List<Order>;
          
            if (orders == null)
            {
                Debug.WriteLine("no null");
            }

            ViewBag.CurrentPage = page == null ? 1 : page.Value;
            ViewBag.Limit = limit == null ? 10 : limit.Value;
            ViewBag.Start = hashtable["Start"];
            ViewBag.End = hashtable["End"];
            ViewBag.TotalPage = hashtable["totalPage"];

            return View(orders);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = _orderService.SelectById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.listOrderDetails = _orderService.GetListOrderDetails(id.Value);

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            
            var order = _orderService.SelectById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,RoleNumber,OrderDate,RequireDate,ShippedDate,Quantity,TotalPrice,Commnet,UserID,FullName,Address,Phone,Email,Status")] Order order)
        {
           
            Dictionary<string, string> errors = order.Validate();

            if (errors.Count == 0)
            {
                if (order.Status == Order.StatusOrder.Finish)
                {
                    order.ShippedDate = DateTime.Now;
                }

                if (ModelState.IsValid)
                {
                    _orderService.Update(order);
                    _orderService.Save();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Errors = errors;
            return View(order);
        }

        private Order UpdateOrder(Order order)
        {
            List<OrderDetail> listOrderDetails = new List<OrderDetail>();
            int totalQuantity = 0;
            double totalPrice = 0;
            try
            {
                listOrderDetails = db.OrderDetails.Where(od => od.OrderID == order.OrderID).ToList();

                for (int i = 0; i < listOrderDetails.Count; i++)
                {
                    totalQuantity += listOrderDetails[i].Quantity;
                    totalPrice += listOrderDetails[i].UnitPrice * listOrderDetails[i].Quantity;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            order.Quantity = totalQuantity;
            order.TotalPrice = totalPrice;

            return order;
        }

        // GET: Orders/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (!CheckUser())
//            {
//                return Redirect("/Home/Login");
//            }
//
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Order order = db.Orders.Find(id);
//            if (order == null)
//            {
//                return HttpNotFound();
//            }
//            return View(order);
//        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            var order = _orderService.SelectById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.Status = Order.StatusOrder.Deleted;
            _orderService.Update(order);
            _orderService.Save();
            //            db.Entry(order).State = EntityState.Modified;
            //            db.SaveChanges();
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
