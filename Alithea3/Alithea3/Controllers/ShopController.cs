using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alithea3.Models;

namespace Alithea3.Controllers
{
    public class ShopController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Shop
        public ActionResult QuanAoNam()
        {
            var listProNam = db.ProductCategories.Where(pc => pc.CategoryID == 1).OrderByDescending(pc => pc.Product.UpdatedAt).Take(4).ToList();
            return PartialView(listProNam);
        }

        public ActionResult QuanAoNu()
        {
            var listProNu = db.ProductCategories.Where(pc => pc.CategoryID == 2).OrderByDescending(pc => pc.Product.UpdatedAt).Take(4).ToList();
            return PartialView(listProNu);
        }

        public ActionResult GiayNam()
        {
            var listGiayNam = db.ProductCategories.Where(pc => pc.CategoryID == 4).OrderByDescending(pc => pc.Product.UpdatedAt).Take(4).ToList();
            return PartialView(listGiayNam);
        }

        public ActionResult GiayNu()
        {
            var listGiayNu = db.ProductCategories.Where(pc => pc.CategoryID == 5).OrderByDescending(pc => pc.Product.UpdatedAt).Take(4).ToList();
            return PartialView(listGiayNu);
        }
    }
}