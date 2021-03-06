﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Alithea3.Controllers.Service.OrderManager;
using Alithea3.Models;
using Alithea3.Models.ViewModel;

namespace Alithea3.Controllers.Api
{
    public class ApiAjaxController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private OrderService _orderService = new OrderService();

        public ActionResult GetListOrder(int? page, int? limit, string start, string end)
        {
//            var hashtable = _orderService.listPagination(page, limit, start, end);
//            var data = hashtable["listOrder"] as List<Order>;

            var orders = db.Orders.Select(o => new OrdersViewModel
            {
                OrderID = o.OrderID,
                RoleNumber = o.RoleNumber,
                OrderDate = o.OrderDate,
                Quantity = o.Quantity,
                TotalPrice = o.TotalPrice,
                UserID = o.UserID,
                FullName = o.FullName,
                Email = o.Email,
                Phone = o.Phone,
                Address = o.Address,
                Commnet = o.Commnet,
                ShippedDate = o.ShippedDate,
                Status = o.Status,
                RequireDate = o.RequireDate
            }).ToList();
//            var jsonResult = new JsonResult() { JsonRequestBehavior  = JsonRequestBehavior.AllowGet};
//            jsonResult.Data = new
//            {
//                success = true,
//                data = orders
//            };

            return Json(new {data = orders}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataTest(Classtest model)
        {
            try
            {
                Debug.WriteLine(model.username);
            }
            catch
            {
                Debug.WriteLine("loi");
            }
            
            var data = db.Categories.Find(1);
            if(data != null)
            {
                return Json(new JavaScriptSerializer().Serialize(data), JsonRequestBehavior.AllowGet);
            }

            return Json("Khong ton tai", JsonRequestBehavior.AllowGet);
        }
    }

    public class Classtest
    {
        public string username { get; set; }
    }
}