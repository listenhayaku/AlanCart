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
            string result = ud.Verify();
            if (result != null)
            {
                Session["Username"] = result;
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

        public ActionResult UpdatePassword()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePassword(Models.UserData ud)
        {
            ud.Username = Session["Username"].ToString();
            if (Services.Security.UpdatePassword(ud.Username, ud.Password))
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