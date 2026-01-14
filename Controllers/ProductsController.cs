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
            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                List<Models.ProductData> listpd = new List<Models.ProductData>();
                listpd = (from s in db.ProductData select s).ToList();
                return View(listpd);
            }
        }
        public ActionResult MyCart()
        {

            return View();
        }
    }
}