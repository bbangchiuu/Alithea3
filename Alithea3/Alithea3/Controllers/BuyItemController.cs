using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Alithea3.Controllers.Service.ShopManager;
using Alithea3.Models;
using Alithea3.Models.ViewModel;
using Newtonsoft.Json;

namespace Alithea3.Controllers
{

    public class BuyItemController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private ShopService _shopService = new ShopService();

        // GET: BuyItem
        public ActionResult Index()
        {
            Debug.WriteLine("dang chay Index");
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Sizes = db.Sizes.ToList();

            return View();
        }

        public ActionResult AddItem(int pro_id, int quantity, int color, string nameColor, int size, string nameSize)
        {
            Debug.WriteLine("dang chay add item");

            var hashtable = _shopService.AddItem(Session[SessionName.ShoppingCart] as List<Product>, pro_id, quantity, color, nameColor, size, nameSize);

            Session[SessionName.ShoppingCart] = hashtable[SessionName.ShoppingCart];
            Session[SessionName.TotalPrice] = (double)hashtable[SessionName.TotalPrice];
            Session[SessionName.TotalQuantity] = (int)hashtable[SessionName.TotalQuantity];

            return Json(new
            {
                ShoppingCart = hashtable[SessionName.ShoppingCart] as List<Product>,
                TotalPrice = (double)hashtable[SessionName.TotalPrice],
                TotalQuantity = (int)hashtable[SessionName.TotalQuantity]
            });
        }

        [HttpPost]
        public ActionResult DeleteItem(int idItem)
        {
            var hashtable = _shopService.DeleteItem(Session[SessionName.ShoppingCart] as List<Product>, idItem);

            Session[SessionName.ShoppingCart] = hashtable[SessionName.ShoppingCart];
            Session[SessionName.TotalPrice] = (double) hashtable[SessionName.TotalPrice];
            Session[SessionName.TotalQuantity] = (int) hashtable[SessionName.TotalQuantity];

            return Json(new
            {
                ShoppingCart = hashtable[SessionName.ShoppingCart] as List<Product>,
                TotalPrice = (double)hashtable[SessionName.TotalPrice],
                TotalQuantity = (int)hashtable[SessionName.TotalQuantity]
            });
        }

        [HttpPost]
        public ActionResult UpdateItem(int idItem, int quantity)
        {
            var hashtable = _shopService.UpdateItem(Session[SessionName.ShoppingCart] as List<Product>, idItem, quantity);

            Session[SessionName.ShoppingCart] = hashtable[SessionName.ShoppingCart];
            Session[SessionName.TotalPrice] = (double)hashtable[SessionName.TotalPrice];
            Session[SessionName.TotalQuantity] = (int)hashtable[SessionName.TotalQuantity];

            return Json(new
            {
                ShoppingCart = hashtable[SessionName.ShoppingCart] as List<Product>,
                totalPriceOneProduct = (double) hashtable["totalPriceOneProduct"],
                TotalPrice = (double)hashtable[SessionName.TotalPrice],
                TotalQuantity = (int)hashtable[SessionName.TotalQuantity]
            });
        }

        public ActionResult ThongTinKhacHang()
        {
            if (Session[SessionName.ShoppingCart] == null)
            {
                return RedirectToAction("Index");
            }

            Customer customer = new Customer();
            if (SessionInfo.Currentuser != null)
            {
                try
                {
                    //UserAccount userAccount = Session[SessionName.UserAccount] as UserAccount;
                    customer.FullName = SessionInfo.Currentuser.Name;
                    customer.Email = SessionInfo.Currentuser.Email;
                    customer.Address = SessionInfo.Currentuser.Address;
                    customer.Phone = SessionInfo.Currentuser.Phone;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            else if(Session[SessionName.Customer] != null)
            {
                customer = Session[SessionName.Customer] as Customer;
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinKhacHang([Bind(Include = "ID,FullName,Address,Phone,Email")] Customer customer, string tinh, string huyen)
        {
            customer.Display();
            Dictionary<string, string> errors = customer.ValidateRegister();
            if (string.IsNullOrEmpty(tinh))
            {
                errors.Add("Tinh", "Bạn chưa chọn tỉnh");
            }
            if (string.IsNullOrEmpty(huyen))
            {
                errors.Add("Huyen", "Bạn chưa chọn huyện, thành phố");
            }

            //HttpCookie addressCookie = new HttpCookie("Address");
            //addressCookie["Tinh"] = tinh;
            //addressCookie["Huyen"] = huyen;
            //Response.Cookies.Add(addressCookie);
            
            if (errors.Count == 0)
            {
                customer.Tinh = tinh;
                customer.Huyen = huyen;
                Session[SessionName.Customer] = customer;
                return RedirectToAction("XacNhanDonHang");
            }

            ViewBag.errors = errors;
            return View(customer);
        }

        public ActionResult XacNhanDonHang()
        {
            if (Session[SessionName.ShoppingCart] == null)
            {
                Debug.WriteLine("ko co");
                return Redirect("/BuyItem/Index");
            }

            if (Session[SessionName.Customer] == null)
            {
                TempData["Error"] = "Bạn phải đăng nhập";
                return Redirect("/BuyItem/Index");
            }

            return View();
        }

        [HttpPost]
        public ActionResult XacNhanDonHang(string comment)
        {
            Debug.WriteLine("--------------");
            Debug.WriteLine("Dang chay xac nhan don hang");
            //Kiểm tra danh sách sản phẩm và thông tin đăng nhập
            if (Session[SessionName.ShoppingCart] == null || Session[SessionName.Customer] == null)
            {
                return Redirect("/BuyItem/Index");
            }

            var createAt = DateTime.Now;
            //HttpCookie addressCookie = Request.Cookies["Address"];

            if (_shopService.createOrder(Session[SessionName.ShoppingCart] as List<Product>, createAt, Session[SessionName.Customer] as Customer,
                (int) Session[SessionName.TotalQuantity], (double) Session[SessionName.TotalPrice], comment, SessionInfo.Currentuser?.Id))
            {
                var result = SendEmail(createAt.ToFileTimeUtc().ToString(), (Session[SessionName.Customer] as Customer)?.Email);
                Debug.WriteLine("result: " + result);

                //if (result)
                //{
                //    DeleteSession();
                //    TempData["Success"] = "Đặt hàng thành công";
                //    TempData["RollNumber"] = createAt.ToFileTimeUtc().ToString();
                //    TempData["Email"] = (Session[SessionName.Customer] as Customer)?.Email;

                //    return Redirect("/BuyItem/Success");
                //}
                DeleteSession();
                TempData["Success"] = "Đặt hàng thành công";
                TempData["RollNumber"] = createAt.ToFileTimeUtc().ToString();
                TempData["Email"] = (Session[SessionName.Customer] as Customer)?.Email;

                return Redirect("/BuyItem/Success");
            }

            TempData["Error"] = "Đã xảy ra lỗi";
            return View();
        }

        public void DeleteSession()
        {
            Session[SessionName.ShoppingCart] = null;
            Session[SessionName.TotalPrice] = null;
            Session[SessionName.TotalQuantity] = null;
        }

        public ActionResult test()
        {
            return View();
        }

        public ActionResult Success()
        {
            if (TempData["Success"] == null || TempData["Success"] == null)
            {
                return Redirect("/BuyItem/Index");
            }
            return View();
        }

        public bool SendEmail(string rollnumber, string _email)
        {
            string senderID = "nguyenhaibangbkn@gmail.com";
            string senderPassword = "lkbnymugdxshtvjq";
            string result = "Email Sent Successfully";

            string body = "Đơn hàng #" + rollnumber + " đã sẵn sàng giao đến quý khách";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(_email);
                mail.From = new MailAddress(senderID);
                mail.Subject = "Đơn đặt hàng Alithea";
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
                smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                result = "problem occurred";
                Response.Write("Exception in sendEmail:" + ex.Message);
                return false;
            }

            return true;
        }
    }
}