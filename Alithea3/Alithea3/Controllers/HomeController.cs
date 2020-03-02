﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Alithea3.Controllers.Service.AttributeManager;
using Alithea3.Controllers.Service.CategoryManager;
using Alithea3.Controllers.Service.ProductCateogryManager;
using Alithea3.Controllers.Service.ProductManager;
using Alithea3.Controllers.Service.ShopManager;
using Alithea3.Controllers.Service.UserAccountManager;
using Alithea3.Models;
using Facebook;
using LinqKit;
using Microsoft.Ajax.Utilities;
using Attribute = System.Attribute;

namespace Alithea3.Controllers
{

    public class HomeController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private CategoryService _categoryService = new CategoryService();
        private ProductService _productService = new ProductService();
        private ShopService _shopService = new ShopService();
        private ProductCategoryService _productCategoryService = new ProductCategoryService();
        private AttributeService _attributeService = new AttributeService();
        private UserAccountService _userAccountService = new UserAccountService();

        public ActionResult Index()
        {
            try
            {
                ViewBag.listCategories = db.Categories.Take(3).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return View();
        }

        public ActionResult Search(string productname, int? page, int? limit)
        {
            ViewBag.productname = productname;

            if (productname == null)
            {
                return Redirect("/Home");
            }

            if (page == null)
            {
                page = 1;
            }

            if (limit == null)
            {
                limit = 6;
            }

            try
            {
                var productFilter = db.Products.Where(p => p.ProductName.Contains(productname)).ToList();

                ViewBag.TotalPage = Math.Ceiling((double)productFilter.Count() / limit.Value);
                ViewBag.CurrentPage = page;
                ViewBag.limit = limit;

                ViewBag.productFilter = productFilter.OrderByDescending(p => p.ProductName).Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();
                ViewBag.listCategories = _categoryService.GetAll().ToList(); 
                ViewBag.currentPara = "?productname=" + productname + "&";

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return View("Filter");
        }

        public ActionResult Filter(List<int> id, int? page, int? limit, double? MinPrice, double? MaxPrice)
        {

            Debug.WriteLine("MinPrice" + MinPrice);
            if (page == null)
            {
                page = 1;
            }

            if (limit == null)
            {
                limit = 6;
            }

            if (id == null && MinPrice == null && MaxPrice == null)
            {
                ViewBag.productFilter = _productService.listPagination(page, limit);
                ViewBag.TotalPage = Math.Ceiling((double)_productService.GetAll().Count() / limit.Value);
                ViewBag.currentPara = "?";
            }
            else
            {
                var hashtable = _shopService.FilterProduct(id, page.Value, limit.Value, MinPrice, MaxPrice);

                ViewBag.categoryFilter = hashtable["listCategory"];
                ViewBag.productFilter = hashtable["listProduct"];

                ViewBag.TotalPage = hashtable["totalPage"];
                ViewBag.currentPara = hashtable["currentPara"];
            }

            ViewBag.listCategories = _categoryService.GetAll().ToList();

            ViewBag.CurrentPage = page;
            ViewBag.limit = limit;
          
            ViewBag.MinPrice = MinPrice;
            ViewBag.MaxPrice = MaxPrice;
         
            return View();
        }

        [EncryptedActionParameterAttribute]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //------------------------
            var product = _productService.SelectById(id);
            ViewBag.listProductCategories = _productCategoryService.GetCategories(id);
            ViewBag.productAttribute = _attributeService.GetAttributesOfProduct(id);
            ViewBag.Size = db.Sizes.ToList();

            return View(product);
        }

        public ActionResult Login(string ReturnUrl = "")
        {
//            if (CheckUser())
//            {
//                return Redirect("/Home/Index");
//            }

            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login([Bind(Include = "Username,Password")] UserAccount userAccount, string ReturnUrl = "")
        {
            var errors = userAccount.ValidateLogin();
            if (errors.Count > 0)
            {
                ViewBag.Errors = errors;
                return View();
            }

            var getUserAccount = _userAccountService.UserAccountLogin(userAccount.Username, userAccount.Password);
            if (getUserAccount != null)
            {
                Session[SessionName.UserAccount] = getUserAccount;
                if (!ReturnUrl.IsNullOrWhiteSpace())
                {
                    return Redirect(ReturnUrl);
                }

                return Redirect("/Home/Index");
            }
            else
            {
                errors.Add("ErrorLogin", "Tài khoản hoặc mật khẩu không đúng");
            }

            ViewBag.Errors = errors;
            return View();
        }

        public ActionResult Register()
        {
            if (CheckUser())
            {
                return Redirect("/Home/Index");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Register(UserAccount userAccount, string confirmpassord)
        {
            var errors = userAccount.ValidateRegister();
            if (confirmpassord == "")
            {
                errors.Add("ConfirmPassword", "Bạn chưa nhập lại mật khẩu");
            }
            else if (userAccount.Password.Equals(confirmpassord) == false)
            {
                errors.Add("ConfirmPassword", "Mật khẩu nhập lại không đúng");
            }


            if (!errors.ContainsKey("Username") && !errors.ContainsKey("Email"))
            {
                //kiểm tra xem đã tồn tại username và email chưa
                var checkUser = _userAccountService.checkUser(userAccount.Username, userAccount.Email);
                if (checkUser.ContainsKey("Username"))
                {
                    errors["Username"] = checkUser["Username"];
                }

                if (checkUser.ContainsKey("Email"))
                {
                    errors["Username"] = checkUser["Username"];
                }
            }

            //tất cả đều hợp lệ
            if (errors.Count == 0)
            {
                if (_userAccountService.createAccount(userAccount))
                {
                    return Redirect("/Home/Login");
                }

                errors.Add("ErrorRegister", "Đăng ký thất bại");
                
            }

            ViewBag.Errors = errors;
            return View();
        }
//
//        private string RandomString(int size)
//        {
//            StringBuilder sb = new StringBuilder();
//            char c;
//            Random rand = new Random();
//            for (int i = 0; i < size; i++)
//            {
//                int a = rand.Next(1, 4);
//                if (a == 1)
//                {
//                    c = Convert.ToChar(Convert.ToInt32(rand.Next(48, 57)));
//                }
//                else if (a == 2)
//                {
//                    c = Convert.ToChar(Convert.ToInt32(rand.Next(65, 90)));
//                }
//                else
//                {
//                    c = Convert.ToChar(Convert.ToInt32(rand.Next(97, 122)));
//                }
//
//                sb.Append(c);
//            }
//
//            return sb.ToString();
//        }

//        private string HasPass(string pass)
//        {
//            //Tạo MD5 
//            MD5 mh = MD5.Create();
//            //Chuyển kiểu chuổi thành kiểu byte
//            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pass);
//            //mã hóa chuỗi đã chuyển
//            byte[] hash = mh.ComputeHash(inputBytes);
//            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
//            StringBuilder sb = new StringBuilder();
//
//            for (int i = 0; i < hash.Length; i++)
//            {
//                sb.Append(hash[i].ToString("X2"));
//            }
//
//            return sb.ToString();
//        }

        public ActionResult ListCategory(int? page, int? limit)
        {
            if (page == null)
            {
                page = 1;
            }

            if (limit == null)
            {
                limit = 9;
            }

            var listCategories = _categoryService.listPagination(page, limit);

            ViewBag.CurrentPage = page;
            ViewBag.limit = limit;
            ViewBag.TotalPage = Math.Ceiling((double)_categoryService.GetAll().Count() / limit.Value);

            return View(listCategories);
        }

        public ActionResult DeleteSessionUser()
        {
            Session[SessionName.UserAccount] = null;
            Session[SessionName.Customer] = null;
            return Redirect("/Home/Index");
        }

        public bool CheckUser()
        {
            UserAccount userAccount = Session[SessionName.UserAccount] as UserAccount;
            if (userAccount != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// start login facebook
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            Debug.WriteLine("loginUrl.AbsoluteUri");
            Debug.WriteLine(loginUrl.AbsoluteUri);
            return Redirect(loginUrl.AbsoluteUri);
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email, first name, middle name etc
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email");
                string email = me.email;
                string userName = me.email;
                string firstname = me.first_name;
                string middlename = me.middle_name;
                string lastname = me.last_name;

                var user = new UserAccount();
                user.RoleNumber = DateTime.Now.ToFileTimeUtc().ToString();
                user.Email = email;
                user.Username = userName;
                user.Status = UserAccount.UserAccountStatus.Active;
                user.FullName = firstname + " " + middlename + " " + lastname;
                user.CreatAt = DateTime.Now;
                user.UpdateAt = DateTime.Now;
                user.BirthDay = DateTime.Now;

                string id = me.id;
                string acsess = accessToken;
                Debug.WriteLine(acsess);
                Debug.WriteLine("thong tin user");
                Debug.WriteLine(id);
                Debug.WriteLine(user.Email);
                Debug.WriteLine(user.FullName);
                //data.UserAccounts.Add(user);
                //data.SaveChanges();

                Session.Add("UserAccount", user);
            }
            return Redirect("/");
        }

        //end login facebook
    }
}