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
                List<Models.OrderData> listOD = (from s in db.OrderData where s.BuyerId == userid select s).Include(s => s.UserData1).ToList();
                return View(listOD);
            }
        }

        public ActionResult MyOrderDetail(string strorderdataid)    //買家看 已下訂的明細 其實這個應該也可以設計給賣家看，但目前先各別做
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
                ViewBag.Message = TempData["Message"];
                return View(listOD);
            }
        }

        public ActionResult PendingShipmentDetail(string strorderdataid)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            int.TryParse(Session["Id"].ToString(), out int userid);
            if(!int.TryParse(strorderdataid, out int orderdataid))
            {
                TempData["Message"] = "Invalid Parameter";
                return RedirectToAction("PendingShipments", "Orders");
            }
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.OrderItem> listOI = (from s in db.OrderItem where s.OrderData.SellerId == userid && s.OrderId == orderdataid select s).Include(s => s.ProductData).Include(s=>s.OrderData).ToList();
                ModelState.Clear();
                return View(listOI);
            }
        }
        [HttpPost]
        public ActionResult EditOrderStatus(string strgenericid,string strorderstatuscode,string strtrigger)    //
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            if(strtrigger == "orderitem")   //這邊設計成2種方式，1. 想改orderitem 2. 想改orderdata
            {
                int.TryParse(Session["Id"].ToString(), out int userid);
                if (!int.TryParse(strgenericid, out int orderitemid))
                {
                    TempData["Message"] = "Invalid Parameter";
                    return RedirectToAction("PendingShipments");
                }
                if (!int.TryParse(strorderstatuscode, out int orderstatuscode))
                {
                    TempData["Message"] = "Invalid Parameter";
                    return RedirectToAction("PendingShipments");
                }
                using (Models.AlanCartEntities db = new Models.AlanCartEntities())
                {
                    Models.OrderItem orderitem = (from s in db.OrderItem where s.Id == orderitemid && s.ProductData.SellerId == userid select s).FirstOrDefault();
                    if (orderitem == default(Models.OrderItem))
                    {
                        TempData["Message"] = "Item not found";
                        return RedirectToAction("PendingShipments");
                    }
                    if (orderstatuscode >= 0 && orderstatuscode < 3) //can be only 1,2,3
                    {
                        orderitem.OrderStatus = orderstatuscode;
                    }
                    db.SaveChanges();
                    return RedirectToAction("PendingShipments");
                }
            }
            else if(strtrigger == "orderdata")
            {
                int.TryParse(Session["Id"].ToString(), out int userid);
                if(!int.TryParse(strgenericid,out int orderdataid))
                {
                    TempData["Message"] = "Invalid Parameter";
                    return RedirectToAction("PendingShipments");
                }
                if (!int.TryParse(strorderstatuscode, out int orderstatuscode))
                {
                    TempData["Message"] = "Invalid Parameter";
                    return RedirectToAction("PendingShipments");
                }
                using(Models.AlanCartEntities db = new Models.AlanCartEntities())
                {
                    Models.OrderData orderdata = (from s in db.OrderData where s.Id == orderdataid select s).FirstOrDefault();
                    if(orderdata == default(Models.OrderData) || orderdata.SellerId != userid)
                    {
                        TempData["Message"] = "this order is not yours";
                        return RedirectToAction("PendingShipments");
                    }
                    orderdata.OrderStatus = orderstatuscode;
                    db.SaveChanges();
                }
                return RedirectToAction("PendingShipments");
            }
            else
            {
                TempData["Message"] = "Trigger Error";
                return RedirectToAction("PendingShipments");
            }
        }

    }
}