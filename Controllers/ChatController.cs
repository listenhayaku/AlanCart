using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlanCart.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contact()
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout","Register");
            if (!int.TryParse(Session["Id"].ToString(),out int userid)) return RedirectToAction("Logout", "Register");
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.UserData> listud = (from s in db.UserData where s.Id != userid select s).ToList();
                return View(listud);
            }
        }
        public ActionResult Chat(string with)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            if (!int.TryParse(with, out int userid)) return RedirectToAction("Contact");
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.UserData userdata = (from s in db.UserData where s.Id == userid select s).FirstOrDefault();
                return View(userdata);
            }
        }
    }
}