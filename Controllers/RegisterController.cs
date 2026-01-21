using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlanCart.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(Models.UserData ud)
        {
            if (ud.Verify())    //如果驗證成功，ud的資料會被補全(原本ud的資料來自表單，只有username&password)
            {
                Session["Id"] = ud.Id;  //我最終還是妥協了 :D
                Session["Username"] = ud.Username;
                Session["Nickname"] = ud.Nickname;
                Session["Role"] = ud.Role;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Message"] = "Login Failed";
                return RedirectToAction("SignIn");
            }
        }
        public ActionResult SignUp()
        {
            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Models.UserData ud)
        {
            if (ud.Register())
            {
                TempData["Message"] = "SignUp Successed";
                return RedirectToAction("SignIn", "Register");
            }
            else
            {
                TempData["Message"] = "SignUp Failed";
                return RedirectToAction("SignUp", "Register");
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("SignIn", "Register");
        }
        public ActionResult UpdatePassword(string struserid)    //如果沒有帶參數就是本人，如果有，就是admin改別人的密碼
            {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            if(int.TryParse(struserid, out int id))
            {
                if (! Services.Security.IsQualifiedUser(Session, UserRole.Administrator)) return RedirectToAction("Logout", "Register");
                ViewBag.UserId = (int?)id;
                return View();
            }
            else
            {
                ViewBag.Message = TempData["Message"];
                return View();
            }
        }
        [HttpPost]
        public ActionResult UpdatePassword(Models.UserData ud)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            if(ud.Id == 0)
            {
                ud.Username = Session["Username"].ToString();
                if (ud.UpdatePassword())
                {
                    TempData["Message"] = "Successful";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Failed";
                    return RedirectToAction("UpdatePassword");
                }
            }
            else
            {
                if (!Services.Security.IsQualifiedUser(Session, UserRole.Administrator)) return RedirectToAction("Logout", "Register");
                if (ud.UpdatePassword())
                {
                    TempData["Message"] = "Successful";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Failed";
                    return RedirectToAction("UpdatePassword");
                }
            }
        }
    }
}