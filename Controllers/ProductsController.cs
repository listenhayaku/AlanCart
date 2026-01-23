using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlanCart.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");
            ViewBag.Message = TempData["Message"];
            string username = Session["Username"].ToString();
            int userid = 0;
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.UserData ud = (from s in db.UserData where s.Username == username select s).FirstOrDefault();
                userid = ud.Id;
            }
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {

                List<Models.ProductData> listpd = new List<Models.ProductData>();
                listpd = (from s in db.ProductData where s.SellerId != userid select s).ToList();
                return View(listpd);
            }
        }
        [HttpPost]
        public ActionResult AddToCart(string productid, string quantity)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("SignIn", "Register");

            if(int.TryParse(productid, out int ProductId) && int.TryParse(quantity,out int Quantity))
            {

                using (Models.AlanCartEntities db = new Models.AlanCartEntities())
                {
                    string sessionusername = Session["Username"].ToString();
                    Models.UserData ud = (from s in db.UserData where s.Username == sessionusername select s).FirstOrDefault();
                    if (ud == default(Models.UserData))
                    {
                        TempData["Message"] = "Username of Session error";
                        return RedirectToAction("Index");
                    }
                    Models.CartOfUser cou = (from s in db.CartOfUser where s.UserId == ud.Id && s.ProductId == ProductId select s).FirstOrDefault();
                    if (cou == default(Models.CartOfUser))   //資料庫還沒有這個使用者對這個產品的資料
                    {
                        cou = new Models.CartOfUser();
                        cou.ProductId = ProductId;
                        cou.UserId = ud.Id;
                        cou.Stock = 1;
                        db.CartOfUser.Add(cou);
                        db.SaveChanges();
                        TempData["Message"] = "Add To Cart Success";
                        return RedirectToAction("Index");
                    }
                    else    //資料庫已有這個使用者對這個產品的資料
                    {
                        cou.Stock += Quantity;
                        db.SaveChanges();
                        TempData["Message"] = "Add To Cart Success";
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                TempData["Message"] = "Invalid Parameter";
                return RedirectToAction("Index");
            }
        }
        public ActionResult MyCart()
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("SignIn", "Register");
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                string username = Session["Username"].ToString();
                Models.UserData ud = (from s in db.UserData where s.Username == username select s).FirstOrDefault();
                if (ud == default(Models.UserData))  //Session["Username"] 有問題
                {
                    TempData["Message"] = "Session Username error";
                    ViewBag.Message = TempData["Message"];
                    return RedirectToAction("SignIn", "Register");
                }
                else    //Session["Username"] 正常
                {
                    List<Models.CartOfUser> listcartofuser = new List<Models.CartOfUser>();
                    listcartofuser = (from s in db.CartOfUser where s.UserId == ud.Id select s).ToList();
                    List<ViewModel.MyCartViewModel> listmycartviewmodel = new List<ViewModel.MyCartViewModel>();
                    foreach (Models.CartOfUser cartofuser in listcartofuser)
                    {
                        ViewModel.MyCartViewModel mycartviewmodel = new ViewModel.MyCartViewModel();
                        mycartviewmodel.CartOfUserId = cartofuser.Id;
                        mycartviewmodel.ProductName = (from s in db.ProductData where s.Id == cartofuser.ProductId select s.Productname).FirstOrDefault();
                        mycartviewmodel.Price = (from s in db.ProductData where s.Id == cartofuser.ProductId select s.Price).FirstOrDefault();
                        mycartviewmodel.Stock = cartofuser.Stock;
                        mycartviewmodel.ImgUrl = (from s in db.ProductData where s.Id == cartofuser.ProductId select s.ImgUrl).FirstOrDefault();
                        listmycartviewmodel.Add(mycartviewmodel);
                    }
                    ViewBag.Message = TempData["Message"];
                    return View(listmycartviewmodel);
                }
            }
        }
        [HttpPost]
        public ActionResult EditCart(ViewModel.MyCartViewModel mycarviewmodel)
        {
            if (Services.Security.IsValidSession(Session))
            {
                using(Models.AlanCartEntities db = new Models.AlanCartEntities())
                {
                    Models.CartOfUser cartofuser = (from s in db.CartOfUser where s.Id == mycarviewmodel.CartOfUserId select s).FirstOrDefault();
                    if(cartofuser != default(Models.CartOfUser))
                    {
                        if(cartofuser.Stock != mycarviewmodel.Stock)
                        {
                            cartofuser.Stock = mycarviewmodel.Stock;
                            db.SaveChanges();
                            TempData["Message"] = "Successful";
                            return RedirectToAction("MyCart");
                        }
                        else
                        {
                            TempData["Message"] = "stock quantity still the same";
                            return RedirectToAction("MyCart");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "item not found";
                        return RedirectToAction("MyCart");
                    }
                }
            }
            else return RedirectToAction("SignIn", "Register");
        }
        public ActionResult DeleteItem(string strcartofuserid)  //刪除購物車的，刪除賣場的在DeleteProduct
        {
            if (!Services.Security.IsValidSession(Session))
            {
                TempData["Message"] = "Invalid Session";
                return RedirectToAction("MyCart");
            }
            int cartofuserid = 0;
            int userid = -1;
            if (!int.TryParse(strcartofuserid, out cartofuserid))
            {
                TempData["Message"] = "Invalid Parameter";
                return RedirectToAction("MyCart");
            }
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                string tempusername = Session["Username"].ToString();
                Models.UserData ud = (from s in db.UserData where s.Username == tempusername select s).FirstOrDefault();
                if(ud != null)
                {
                    userid = ud.Id;
                }
                else
                {
                    TempData["Message"] = "Invalid Session";
                    return RedirectToAction("MyCart");
                }
            }
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.CartOfUser cou = (from s in db.CartOfUser where s.UserId == userid && s.Id == cartofuserid select s).FirstOrDefault();
                if(cou != null)
                {
                    db.CartOfUser.Remove(cou);
                    db.SaveChanges();
                    TempData["Message"] = "Delete Successful";
                    return RedirectToAction("MyCart");
                }
                else
                {
                    TempData["Message"] = "User doesn't have this item";
                    return RedirectToAction("MyCart");
                }
            }
            TempData["Message"] = "Invalid Session";
            return RedirectToAction("MyCart");
        }
        public ActionResult Checkout()  //這個不用參數，直接暴力清空購物車
        {
            if (Services.Security.IsValidSession(Session) && int.TryParse(Session["Id"].ToString(),out int userid))
            {
                using (Models.AlanCartEntities db = new Models.AlanCartEntities())
                {
                    List<Models.CartOfUser> listCOU = (from s in db.CartOfUser where s.UserId == userid select s).ToList();
                    List<Models.OrderData> listOD = new List<Models.OrderData>();
                    List<Models.OrderItem> listOI = new List<Models.OrderItem>();
                    //先單純建立OrderData
                    foreach(Models.CartOfUser COU in listCOU)
                    {
                        Models.OrderItem orderitem = new Models.OrderItem();
                        orderitem.ProductId = COU.ProductId;
                        orderitem.Stock = COU.Stock;
                        orderitem.OrderStatus = 0;
                        listOI.Add(orderitem);

                        //OrderData
                        int found = -1;
                        for(int i =0;i < listOD.Count; i++)
                        {
                            if (listOD[i].SellerId == COU.ProductData.SellerId)
                            {
                                found = i;
                            }
                        }
                        if (found == -1)
                        {
                            Models.OrderData orderdata = new Models.OrderData();
                            orderdata.SellerId = COU.ProductData.SellerId;
                            orderdata.BuyerId = COU.UserId;
                            orderdata.TotalAmount += COU.ProductData.Price * COU.Stock;
                            orderdata.OrderStatus = 0;
                            listOD.Add(orderdata);
                        }
                        else
                        {
                            listOD[found].TotalAmount += COU.ProductData.Price * COU.Stock;
                        }
                    }
                    foreach (Models.OrderData orderdata in listOD) db.OrderData.Add(orderdata);
                    db.SaveChanges();
                    listOD = (from s in db.OrderData select s).ToList();
                    foreach(Models.OrderItem orderitem in listOI)
                    {
                        for(int i = 0;i < listOD.Count; i++)
                        {
                            orderitem.ProductData = (from s in db.ProductData where s.Id == orderitem.ProductId select s).FirstOrDefault();
                            if(orderitem.ProductData.SellerId == listOD[i].SellerId)
                            {
                                orderitem.OrderId = listOD[i].Id;
                            }
                        }
                        db.OrderItem.Add(orderitem);
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("MyCart");
            }
            else
            {
                TempData["Message"] = "Invalid Session";
                return RedirectToAction("MyCart");
            }

        }
        public ActionResult NewProduct()
        {
            if (!(Services.Security.IsValidSession(Session) && Services.Security.IsQualifiedUser(Session, UserRole.Administrator))) return RedirectToAction("SignIn", "Register");
            if (TempData["Message"] != null) ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public ActionResult NewProduct(ViewModel.NewProductViewModel npv)
        {
            if (!(Services.Security.IsValidSession(Session) && Services.Security.IsQualifiedUser(Session, UserRole.Administrator))) return RedirectToAction("SignIn", "Register");


            if (npv.Excute())
            {
                TempData["Message"] = "Successful";
                return RedirectToAction("NewProduct");
            }
            else
            {
                TempData["Message"] = "Failed";
                return RedirectToAction("NewProduct");
            }

        }
        public ActionResult MyProducts()
        {
            if (!Services.Security.IsValidSession(Session))
            {
                TempData["Message"] = "Invalid Session";
                return RedirectToAction("Logout", "Register");
            }
            if (!int.TryParse(Session["Id"].ToString(),out int userid))
            {
                TempData["Message"] = "parsing session id failed";
                return RedirectToAction("Logout", "Register");
            }
            ViewBag.Message = TempData["Message"];
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.ProductData> listpd = new List<Models.ProductData>();
                listpd = (from s in db.ProductData where s.SellerId == userid select s).ToList();
                return View(listpd);
            }
        }
        public ActionResult MyOrders()
        {
            return View();
        }
        public ActionResult DeleteProduct(string strproductid)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("Logout", "Register");  //還敢皮?直接給你登出
            if (!int.TryParse(strproductid, out int productid)) return RedirectToAction("MyProducts");
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.ProductData pd = (from s in db.ProductData where s.Id == productid select s).FirstOrDefault();
                if (pd == default(Models.ProductData)) return RedirectToAction("MyProducts");
                if (pd.DeleteProduct()) //這個方法會自己把資料從資料庫刪掉
                {
                    TempData["Message"] = "Delete Successful";
                    return RedirectToAction("MyProducts");
                }
                else
                {
                    TempData["Message"] = "Delete Failed";
                    return RedirectToAction("MyProducts");
                }
            }
        }
    }
}