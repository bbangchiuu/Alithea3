using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Alithea3.Models
{
    public static class Encrypt
    {
        public static string EncryptAndHash(string value, string key)
        {
            var des = new MACTripleDES();
            var md5 = new MD5CryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            string encrypted = Convert.ToBase64String(des.ComputeHash(Encoding.UTF8.GetBytes(value))) +
                               '-' +
                               Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

            return encrypted;
        }

        public static string DecryptWithHash(string encoded, string key)
        {
            var des = new MACTripleDES();
            var md5 = new MD5CryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

            string decoded = encoded;

            decoded = decoded.Replace(" ", "+");
            string value = decoded.Split('-').Length > 1
                ? Encoding.UTF8.GetString(Convert.FromBase64String(decoded.Split('1')[1]))
                : string.Empty;
            string saveHash = Encoding.UTF8.GetString(Convert.FromBase64String(decoded.Split('1')[0]));
            string calculateHash = Encoding.UTF8.GetString(des.ComputeHash(Encoding.UTF8.GetBytes(value)));

            if (saveHash != calculateHash)
            {
                return null;
            }

            return value;
        }
    }
}