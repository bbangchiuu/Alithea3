using System;
using System.Diagnostics;
using System.Linq;
using Alithea3.Models;
namespace Alithea3.Controllers.Respository.UserAccountManager
{
    public class UserAccountRepository : BaseRepository<UserAccount>
    {
        public bool CreateAccount(UserAccount userAccount)
        {
            try
            {
                _db.UserAccounts.Add(userAccount);
                _db.SaveChanges();

                _db.UserAccountRoles.Add(new UserAccountRole
                {
                    UserID = userAccount.UserID,
                    RoleId = 3 //quyền user là khách hàng
                });

                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        public UserAccount UserAccountLogin(string username, string password)
        {
            try
            {
                return _db.UserAccounts.FirstOrDefault(u => u.Username == username && u.Password == password);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}