using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlanCart.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList()
        {
            if (!(Services.Security.IsValidSession(Session) && Services.Security.IsQualifiedUser(Session, UserRole.Administrator))) return RedirectToAction("Logout", "Register");
            ViewBag.Message = TempData["Message"];
            List<Models.UserData> listud = new List<Models.UserData>();
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                listud = (from s in db.UserData select s).ToList();
                return View(listud);
            }
        }
        public ActionResult EditUser(string struserid)
        {
            if (!(Services.Security.IsValidSession(Session) && Services.Security.IsQualifiedUser(Session, UserRole.Administrator))) return RedirectToAction("Logout", "Register");
            if (!int.TryParse(struserid, out int userid)) return RedirectToAction("UserList");
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.UserData ud = (from s in db.UserData where s.Id == userid select s).FirstOrDefault();
                if (ud == default(Models.UserData)) return RedirectToAction("UserList");
                return View(ud);
            }
        }
        [HttpPost]
        public ActionResult EditUser(Models.UserData userdata)
        {
            if(!(Services.Security.IsValidSession(Session) && Services.Security.IsQualifiedUser(Session, UserRole.Administrator))){
                TempData["Message"] = "Invalid Session or User";
                return RedirectToAction("EditUser");
            }
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.UserData ud = (from s in db.UserData where (s.Id == userdata.Id && s.Username == userdata.Username) select s).FirstOrDefault();
                if(ud == default(Models.UserData))
                {
                    TempData["Message"] = "Cannot find userdata";
                    return RedirectToAction("EditUser");
                }
                ud.Nickname = userdata.Nickname;
                ud.Role = userdata.Role;
                db.SaveChanges();
                TempData["Message"] = "EditUser successful";
                return RedirectToAction("UserList");
            }
        }
    }
}