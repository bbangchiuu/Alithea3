using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alithea3.Controllers.Service.UserAccountManager;
using Alithea3.Models;

namespace Alithea3.Controllers
{
    public class Author : AuthorizeAttribute
    {
        public int[] RoleId { set; get; }
        private UserAccountService _userAccount = new UserAccountService();
        private MyDbContext db = new MyDbContext();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userAccount = (UserAccount)HttpContext.Current.Session[SessionName.UserAccount];
            if (userAccount == null)
            {
                return false;
            }

            if (RoleId == null)
            {
                return true;
            }
            else
            {
                List<int> UserRole = db.UserAccountRoles.Where(ur => ur.UserID == userAccount.UserID)
                    .Select(ur => ur.RoleId).ToList();

                for (int i = 0; i < RoleId.Length; i++)
                {
                    if (UserRole.Contains(RoleId[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}