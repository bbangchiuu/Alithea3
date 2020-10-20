using Alithea3.Controllers.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Alithea3.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            //var url = "a%20a%5Db";
            //var test = System.Web.HttpUtility.UrlDecode(url);
            //Debug.WriteLine(test);

            ITestService test = new TestService();
            return Json(new { data = test.get() }, JsonRequestBehavior.AllowGet);
        }
    }
}