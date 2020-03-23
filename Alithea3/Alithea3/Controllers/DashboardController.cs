using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alithea3.Controllers
{
    [Author]
    public class DashboardController : Controller
    {
        // GET: Dashboaed
        public ActionResult Index()
        {
            return View();
        }
    }
}