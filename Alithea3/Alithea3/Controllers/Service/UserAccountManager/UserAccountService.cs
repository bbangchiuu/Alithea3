using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Alithea3.Controllers.Respository.UserAccountManager;
using Alithea3.Models;
namespace Alithea3.Controllers.Service.UserAccountManager
{
    public class UserAccountService
    {
        private MyDbContext db = new MyDbContext();

        UserAccountRepository _userAccountRepository = new UserAccountRepository();

        public bool createAccount(UserAccount userAccount)
        {
            userAccount.RoleNumber = DateTime.Now.ToFileTimeUtc().ToString();
            userAccount.admin = UserAccount.Decentralization.Customer;
            userAccount.Password = HasPass(userAccount.Password);
            userAccount.CreatAt = DateTime.Now;
            userAccount.UpdateAt = DateTime.Now;
            userAccount.Status = UserAccount.UserAccountStatus.Active;
            userAccount.Image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRQ8xzdv564ewROcTBYDdv51oTD5SgNOCDDwMw4XXIdvxFGyQzn&s";
            
            return _userAccountRepository.CreateAccount(userAccount);
        }

        private string HasPass(string pass)
        {
            //Tạo MD5 
            MD5 mh = MD5.Create();
            //Chuyển kiểu chuổi thành kiểu byte
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pass);
            //mã hóa chuỗi đã chuyển
            byte[] hash = mh.ComputeHash(inputBytes);
            //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public Dictionary<string, string> checkUser(string username, string email)
        {
            var errors = new Dictionary<string, string>();

            var getListUserAccount = new List<UserAccount>();
            try
            {
                getListUserAccount = db.UserAccounts.Where(u => u.Username == username || u.Email == email).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            if (getListUserAccount != null && getListUserAccount.Count > 0)
            {
                Debug.WriteLine("count: " + getListUserAccount.Count);
                for (int i = 0; i < getListUserAccount.Count; i++)
                {
                    if (username.Equals(getListUserAccount[i].Username))
                    {
                        errors["Username"] = "Tài khoản này đã tồn tại";
                    }
                    if (email.Equals(getListUserAccount[i].Email))
                    {
                        errors["Email"] = "Email này đã tồn tại";
                    }
                }
            }

            return errors;
        }

        public UserAccount UserAccountLogin(string username, string password)
        {
            password = HasPass(password);
            try
            {
                return _userAccountRepository.UserAccountLogin(username, password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}