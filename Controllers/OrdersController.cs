using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace AlanCart.Controllers
{
    public class OrdersController : Controller
    {
        // GET: Orders
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyOrders()  //買家看 已下訂
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            int.TryParse(Session["Id"].ToString(), out int userid);
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.OrderData> listOD = (from s in db.OrderData where s.BuyerId == userid select s).ToList();
                return View(listOD);
            }
        }

        public ActionResult MyOrderDetail(string strorderdataid)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout","Register");
            int.TryParse(Session["Id"].ToString(), out int userid);
            if(!int.TryParse(strorderdataid, out int orderdataid))  //invalid parameter
            {
                
                return RedirectToAction("MyOrders");
            }
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.OrderItem> listOI = (from s in db.OrderItem where s.OrderData.BuyerId == userid && s.OrderId == orderdataid select s).Include(s => s.ProductData).ToList();
                return View(listOI);
            }
        }
        public ActionResult PendingShipments()  //賣家看
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            int.TryParse(Session["Id"].ToString(), out int userid);
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.OrderData> listOD = (from s in db.OrderData where s.SellerId == userid select s).ToList();
                return View(listOD);
            }
        }
    }
}