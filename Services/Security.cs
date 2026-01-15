using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace AlanCart.Services
{
    public class Security
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static bool IsValidSession(HttpSessionStateBase session) //直接驗證是否是有效Session
        {
            if(session != null)
            {
                using(Models.AlanCartEntities db = new Models.AlanCartEntities())
                {
                    if(session["Username"] != null)
                    {
                        string username = session["Username"].ToString();
                        Models.UserData ud = (from s in db.UserData where s.Username == username select s).FirstOrDefault();
                        if(ud != default(Models.UserData))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;

        }
        public static bool IsQualifiedUser(HttpSessionStateBase session,UserRole role)
        {
            string username = session["Username"].ToString();
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.UserData ud = (from s in db.UserData where s.Username == username select s).FirstOrDefault();
                if (ud == default(Models.UserData)) return false;
                if(ud.Role == (int)role) return true;
                else return false;
            }
        }
        public static bool IsQualifiedUser(HttpSessionStateBase session,List<int> listrole)
        {
            return false;
        }
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using(var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                return String.Join(".", Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            }
        }
        public static bool VerifyPassword(string password,string storedHash)
        {
            //自己留的後台，當db的密碼為""可用空的值登入
            if (storedHash == "") return true;
            //正常的驗證流程
            var parts = storedHash.Split('.');
            if (parts.Length != 3) return false;

            int iteration = int.Parse(parts[0]);
            byte[] salt= Convert.FromBase64String(parts[1]);
            byte[] stored = Convert.FromBase64String(parts[2]);

            using(var pbkdf2 = new Rfc2898DeriveBytes(password,salt, iteration))
            {
                byte[] computed = pbkdf2.GetBytes(stored.Length);
                return SlowEquals(stored, computed);
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            else
            {
                int diff = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    diff |= a[i] ^ b[i];
                }
                return diff == 0;
            }
        }
    }
}