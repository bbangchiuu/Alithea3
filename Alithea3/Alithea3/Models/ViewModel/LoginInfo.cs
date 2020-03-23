using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alithea3.Models.ViewModel
{
    public class LoginInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class SessionInfo
    {
        public static LoginInfo Currentuser
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                {
                    return null;
                }

                var json = HttpContext.Current.User.Identity.Name;
                var accInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginInfo>(json);
                return accInfo;
            }
        }
    }
}