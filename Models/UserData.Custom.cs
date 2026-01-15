using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlanCart.Models
{
    public partial class UserData
    {
        public bool Verify()    //如果驗證成功，ud的資料會被補全(原本ud的資料來自表單，只有username&password)
        {
            using(Models.AlanCartEntities db = new AlanCartEntities())
            {
                UserData ud = (from s in db.UserData where s.Username == this.Username select s).FirstOrDefault();
                if (ud == default(UserData)) return false;
                else
                {
                    if (Services.Security.VerifyPassword(this.Password, ud.Password))
                    {
                        this.Id = ud.Id;
                        this.Username = ud.Username;
                        this.Password = ud.Password;
                        this.Nickname = ud.Nickname;
                        this.Role = ud.Role;
                        return true;
                    }
                    else
                        return false;
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
                    this.Role = (int)UserRole.User;
                    db.UserData.Add(this);
                    db.SaveChanges();
                    return true;
                }
            }
        }
        public bool UpdatePassword()
        {
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.UserData ud = (from s in db.UserData where s.Username == this.Username select s).FirstOrDefault();
                if (ud == default(Models.UserData)) return false;
                ud.Password = Services.Security.HashPassword(this.Password);
                db.SaveChanges();
                return true;
            }
        }
    }
}