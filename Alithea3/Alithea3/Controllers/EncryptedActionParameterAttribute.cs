using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Alithea3.Models;

namespace Alithea3.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
                if (HttpContext.Current.Request.QueryString.Get("q") != null)
                {
                    string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                    string decrptedString = Encrypt.DecryptWithHash(encryptedQueryString, SessionName.KeyEncrypt);
                    string[] paramsArrs = decrptedString.Split('?');

                    for (int i = 0; i < paramsArrs.Length; i++)
                    {
                        string[] paramArr = paramsArrs[i].Split('=');
                        decryptedParameters.Add(paramArr[0], (paramArr[1]));// pass two string parameters
                    }
                }

                var allParams = filterContext.ActionDescriptor.GetParameters();
                var dicParamsType = new Dictionary<string, Type>();
                for (int i = 0; i < allParams.Length; i++)
                {
                    dicParamsType.Add(allParams[i].ParameterName, allParams[i].ParameterType);
                }

                for (int i = 0; i < decryptedParameters.Count; i++)
                {
                    var typeOfParamsOrg = dicParamsType[decryptedParameters.Keys.ElementAt(i)];

                    var typeOfParams = Nullable.GetUnderlyingType(typeOfParamsOrg) ?? typeOfParamsOrg;

                    var paramObjValue = decryptedParameters.Values.ElementAt(i);
                    if (paramObjValue != null)
                    {
                        filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = Convert.ChangeType(paramObjValue, typeOfParams);
                    }
                }
            }
            catch (Exception e)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}