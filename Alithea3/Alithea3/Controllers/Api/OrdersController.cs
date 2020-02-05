using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Alithea3.Models;
using Microsoft.Ajax.Utilities;

namespace Alithea3.Controllers.Api
{
    public class OrdersController : ApiController
    {
        private MyDbContext db = new MyDbContext();

        public OrdersController()
        {
            db.Configuration.ProxyCreationEnabled = false;
        }

        // GET: api/Orders
        public IQueryable<Object> GetOrders()
        {
            var listOrder = db.Orders.GroupBy( o => new
            {
                Year = o.OrderDate.Year,
                Month = o.OrderDate.Month,
                Day = o.OrderDate.Day
            }).Select( o => new
            {
                datetime = o.FirstOrDefault().OrderDate,
                price = o.Sum(order => order.TotalPrice)
            });
            return listOrder;
        }

        public IQueryable<Object> GetOrderss(string start, string end)
        {
            if (start.IsNullOrWhiteSpace() && end.IsNullOrWhiteSpace())
            {
                var listOrders = db.Orders.GroupBy(o => new
                {
                    Year = o.OrderDate.Year,
                    Month = o.OrderDate.Month,
                    Day = o.OrderDate.Day
                }).Select(o => new
                {
                    datetime = o.FirstOrDefault().OrderDate,
                    price = o.Sum(order => order.TotalPrice)
                });
                return listOrders;
            }

            var listOrder= db.Orders.GroupBy(o => new
            {
                Year = o.OrderDate.Year,
                Month = o.OrderDate.Month,
                Day = o.OrderDate.Day
            }).Select(o => new
            {
                datetime = o.FirstOrDefault().OrderDate,
                price = o.Sum(order => order.TotalPrice)
            });
            
            var startTime = DateTime.Now;
            startTime = startTime.AddYears(-1);
            try
            {
                startTime = DateTime.Parse(start);
                listOrder = listOrder.Where(o => o.datetime >= startTime);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            var endTime = DateTime.Now;
            try
            {
                endTime = DateTime.Parse(end);
                listOrder = listOrder.Where(o => o.datetime <= endTime);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return listOrder;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.OrderID)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.OrderID }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderID == id) > 0;
        }
    }
}