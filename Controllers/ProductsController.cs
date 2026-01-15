using System;
using System.Collections.Generic;
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
            ViewBag.Message = TempData["Message"];
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.ProductData> listpd = new List<Models.ProductData>();
                listpd = (from s in db.ProductData select s).ToList();
                return View(listpd);
            }
        }
        [HttpPost]
        public ActionResult AddToCart(int ProductId,int Quantity)
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("SignIn", "Register");
            System.Diagnostics.Debug.WriteLine("ProductId:" + Convert.ToString(ProductId));
            using (Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                string sessionusername = Session["Username"].ToString();
                Models.UserData ud = (from s in db.UserData where s.Username == sessionusername select s).FirstOrDefault();
                if(ud == default(Models.UserData))
                {
                    TempData["Message"] = "Username of Session error";
                    return RedirectToAction("Index");
                }
                Models.CartOfUser cou = (from s in db.CartOfUser where s.UserId == ud.Id && s.ProductId == ProductId select s).FirstOrDefault();
                if(cou == default(Models.CartOfUser))   //資料庫還沒有這個使用者對這個產品的資料
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
        public ActionResult MyCart()
        {
            if (!Services.Security.IsValidSession(Session)) return RedirectToAction("SignIn", "Register");
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                string username = Session["Username"].ToString();
                Models.UserData ud = (from s in db.UserData where s.Username == username select s).FirstOrDefault();
                if(ud == default(Models.UserData))  //Session["Username"] 有問題
                {
                    TempData["Message"] = "Session Username error";
                    return RedirectToAction("SignIn", "Register");
                }
                else
                {
                    List<Models.CartOfUser> listcartofuser = new List<Models.CartOfUser>();
                    listcartofuser = (from s in db.CartOfUser where s.UserId == ud.Id select s).ToList();
                    List<ViewModel.MyCartViewModel> listmycartviewmodel = new List<ViewModel.MyCartViewModel>();
                    foreach(Models.CartOfUser cartofuser in listcartofuser)
                    {
                        ViewModel.MyCartViewModel mycartviewmodel = new ViewModel.MyCartViewModel();
                        mycartviewmodel.UserId = cartofuser.UserId;
                        mycartviewmodel.ProductId = cartofuser.ProductId;
                        mycartviewmodel.ProductName = (from s in db.ProductData where s.Id == cartofuser.ProductId select s.Productname).FirstOrDefault();
                        mycartviewmodel.Stock = cartofuser.Stock;
                        mycartviewmodel.ImgUrl = (from s in db.ProductData where s.Id == cartofuser.ProductId select s.ImgUrl).FirstOrDefault();
                        listmycartviewmodel.Add(mycartviewmodel);
                    }
                    return View(listmycartviewmodel);
                }
            }
        }
        public ActionResult NewProduct()
        {
            if (!(Services.Security.IsValidSession(Session) && Services.Security.IsQualifiedUser(Session,UserRole.Administrator))) return RedirectToAction("SignIn", "Register");
            return View();
        }
    }
}