﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Alithea3.Models;
using Attribute = Alithea3.Models.Attribute;

namespace Alithea3.Controllers.Respository.OrderManager
{
    public class OrderRespository : BaseRepository<Order>
    {
        public List<Order> listPagination(int page, int limit)
        {

            var list = new List<Order>();
            try
            {
                list = _table.OrderByDescending(o => o.OrderDate).Skip(page).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return list;
        }

        public List<Order> listPaginationOrderByDate(int page, int limit, DateTime startTime, DateTime endTime)
        {

            var list = new List<Order>();
            try
            {
                list = _table.OrderByDescending(o => o.OrderDate).Where(s => s.OrderDate >= startTime && s.OrderDate <= endTime).Skip(page).Take(limit).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return list;
        }

        public int ListCountOrderByDate(DateTime startTime, DateTime endTime)
        {
            try
            {
                return _table
                    .OrderByDescending(o => o.OrderDate).Count(s => s.OrderDate >= startTime && s.OrderDate <= endTime);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return 0;
            }
        }

        public bool createOrder(List<Product> listShoppingCart, Order newOrder)
        {
            try
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        newOrder.Display();

                        var listOrderDetail = new List<OrderDetail>();
                        for (int i = 0; i < listShoppingCart.Count; i++)
                        {
                            int? idColor = null;
                            if (listShoppingCart[i].Color != 0)
                            {
                                idColor = listShoppingCart[i].Color;
                            }

                            int? idSize = null;
                            if (listShoppingCart[i].Size != 0)
                            {
                                idSize = listShoppingCart[i].Size;
                            }

                            listOrderDetail.Add(new OrderDetail()
                            {
                                OrderID = newOrder.OrderID,
                                ProductID = listShoppingCart[i].ProductID,
                                Quantity = listShoppingCart[i].Quantity,
                                UnitPrice = listShoppingCart[i].UnitPrice,
                                ColorID = idColor,
                                NameColor = listShoppingCart[i].NameColor,
                                SizeID = idSize,
                                NameSize = listShoppingCart[i].NameSize,
                                ProductImage = listShoppingCart[i].ProductImage,
                                VAT = listShoppingCart[i].VAT
                            });

//                            if (idColor != null)
//                            {
//                                var attr = _db.Attributes.Where(a => a.ColorID == idColor.Value && 
//                                                                     a.ProductID == listShoppingCart[i].ProductID).ToList();
//
//                                attr[0].Quantity -= listShoppingCart[i].Quantity;
//                                _db.Entry(attr).State = EntityState.Modified;
//
//                            }
//                            else
//                            {
//                                var product = _db.Products.Find(listShoppingCart[i].ProductID);
//                                product.Quantity -= listShoppingCart[i].Quantity;
//                                _db.Entry(product).State = EntityState.Modified;
//                            }
                        }

                        newOrder.OrderDetails = listOrderDetail;
                        _db.Orders.Add(newOrder);

                        _db.SaveChanges();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("loi: " + e.Message);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public List<OrderDetail> GetListOrderDetails(int orderId)
        {
            return _db.OrderDetails.Where(od => od.OrderID == orderId).ToList();
        }
    }
}