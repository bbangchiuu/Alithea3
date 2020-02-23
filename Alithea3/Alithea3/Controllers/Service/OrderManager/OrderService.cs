using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Alithea3.Controllers.Respository.OrderManager;
using Alithea3.Models;
using Microsoft.Ajax.Utilities;

namespace Alithea3.Controllers.Service.OrderManager
{
    public class OrderService : BaseService<Order>
    {
        private OrderRespository _orderRespository = new OrderRespository();
        private MyDbContext _db = new MyDbContext();

        public Hashtable listPagination(int? page, int? limit, string start, string end)
        {
            if (page == null)
            {
                page = 1;
            }

            if (limit == null)
            {
                limit = 10;
            }

            var hashtable = new Hashtable();
            var listOrder = _orderRespository.GetAll();
            double totalPage = 0;
            if (start.IsNullOrWhiteSpace() && end.IsNullOrWhiteSpace())
            {
                listOrder = listOrder.ToList();
            }
            else
            {
                var startTime = DateTime.Now;
                startTime = startTime.AddYears(-1);
                try
                {
                    startTime = DateTime.Parse(start);
                    listOrder = listOrder.Where(o => o.OrderDate >= startTime);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                var endTime = DateTime.Now;
                try
                {
                    endTime = DateTime.Parse(end);
                    listOrder = listOrder.Where(o => o.OrderDate <= endTime);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                hashtable.Add("Start", startTime.ToString("yyyy-MM-dd"));
                hashtable.Add("End", endTime.ToString("yyyy-MM-dd"));

                listOrder = listOrder.ToList();
            }

            hashtable.Add("totalPage", Math.Ceiling((double)listOrder.Count() / limit.Value));
            hashtable.Add("listOrder", listOrder.OrderByDescending(o => o.OrderDate).
                Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList());
            return hashtable;
        }

        public DbSet<Order> getOrderByOrderDate(string start, string end)
        {
            if (start.IsNullOrWhiteSpace() && end.IsNullOrWhiteSpace())
            {
                return _db.Orders;
            }

            var listOrder = _db.Orders;

            var startTime = DateTime.Now;
            startTime = startTime.AddYears(-1);
            try
            {
                startTime = DateTime.Parse(start);
                listOrder = (DbSet<Order>)listOrder.Where(o => o.OrderDate >= startTime);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            var endTime = DateTime.Now;
            try
            {
                endTime = DateTime.Parse(end);
                listOrder = (DbSet<Order>)listOrder.Where(o => o.OrderDate <= endTime);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return listOrder;
        }

        public List<OrderDetail> GetListOrderDetails(int orderId)
        {
            try
            {
                return _orderRespository.GetListOrderDetails(orderId);
            }
            catch
            { 
                return null;
            }
        }
    }
}