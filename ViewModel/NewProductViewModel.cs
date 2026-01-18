using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace AlanCart.ViewModel
{
    public class NewProductViewModel
    {
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public HttpPostedFileBase FileUploaded { get; set; }

        public bool Excute()
        {
            //先驗證
            if(this.ProductName == default(string) || this.Price == default(int) || this.Stock == default(int) || this.FileUploaded == default(HttpPostedFileBase) || this.FileUploaded.ContentLength == 0)
            {
                //欄位填寫不完整
                return false;
            }
            //處理檔案
            string orifilename = Path.GetFileName(this.FileUploaded.FileName);

            string destinationdictionary = HttpContext.Current.Server.MapPath("~/Content/Image/");
            if (!Directory.Exists(destinationdictionary))
            {
                Directory.CreateDirectory(destinationdictionary);
            }

            string newFilename = Guid.NewGuid() + Path.GetExtension(orifilename);
            string fullpath = Path.Combine(destinationdictionary, newFilename);

            this.FileUploaded.SaveAs(fullpath); 

            using(Models.AlanCartEntities db = new Models.AlanCartEntities())
            {
                Models.ProductData pd = new Models.ProductData();
                pd.Productname = this.ProductName;
                pd.Price = this.Price;
                pd.Stock = this.Stock;
                pd.Available = 1;
                pd.ImgUrl = "~/Content/Image/" + newFilename;

                db.ProductData.Add(pd);
                db.SaveChanges();
            }

            return true;

        }
    }
}