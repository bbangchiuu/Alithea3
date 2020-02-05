using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Alithea3.Models;

namespace Alithea3.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserAccount) Session[SessionName.UserAccount];
            if (session == null || session.admin != UserAccount.Decentralization.Admin)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    Controller = "Home",
                    Action = "Login"
                }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}