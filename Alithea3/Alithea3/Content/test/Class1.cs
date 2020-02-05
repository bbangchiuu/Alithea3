using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alithea3.Content.test
{
    public class Class1
    {
        //            var orders = new List<Order>();
        //          
        //            ViewBag.CurrentPage = page;
        //            ViewBag.limit = limit;
        //
        //            if (start.IsNullOrWhiteSpace() || end.IsNullOrWhiteSpace())
        //            {
        //                Debug.WriteLine("khong co date");
        //                try
        //                {
        //                    orders = db.Orders.OrderByDescending(o => o.OrderDate).ToList();
        //                    ViewBag.TotalPage = Math.Ceiling((double)orders.Count() / limit.Value);
        //                    orders = orders.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();
        //                }
        //                catch (Exception e)
        //                {
        //                    Debug.WriteLine("loi: " + e.Message);
        //                }
        //            }
        //            else
        //            {
        //                var startTime = DateTime.Now;
        //                startTime = startTime.AddYears(-1);
        //                try
        //                {
        //                    startTime = DateTime.Parse(start);
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine(e);
        //                }
        //
        //                var endTime = DateTime.Now;
        //                try
        //                {
        //                    endTime = DateTime.Parse(end);
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine(e);
        //                }
        //
        //                try
        //                {
        //                    orders = db.Orders.OrderByDescending(s => s.OrderDate).Where(s => s.OrderDate >= startTime && s.OrderDate <= endTime).ToList();
        //
        //                    ViewBag.TotalPage = Math.Ceiling((double)orders.Count() / limit.Value);
        //
        //                    orders = orders.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();
        //
        //                    ViewBag.Start = startTime.ToString("yyyy-MM-dd");
        //                    ViewBag.End = endTime.ToString("yyyy-MM-dd");
        //                }
        //                catch (Exception e)
        //                {
        //                    Console.WriteLine(e);
        //                }
        //            }
    }
}