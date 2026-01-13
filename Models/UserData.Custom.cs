using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlanCart.Models
{
    public partial class UserData
    {
        public string Verify()
        {
            using(Models.AlanCartEntities db = new AlanCartEntities())
            {
                UserData ud = (from s in db.UserData where s.Username == this.Username select s).FirstOrDefault();
                if (ud == default(UserData)) return null;
                else
                {
                    if (Services.Security.VerifyPassword(this.Password,ud.Password))
                        return ud.Username;
                    else
                        return null;
                }
            }
        }

        public bool Register()
        {
            using(Models.AlanCartEntities db = new AlanCartEntities())
            {
                Models.UserData ud = (from s in db.UserData where s.Username == this.Username select s).FirstOrDefault();
                //如果已經有相同username
                if (ud != default(UserData)) return false;
                else
                {
                    this.Password = Services.Security.HashPassword(this.Password);
                    db.UserData.Add(this);
                    db.SaveChanges();
                    return true;
                }
            }
        }
    }
}