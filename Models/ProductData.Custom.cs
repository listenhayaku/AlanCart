using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace AlanCart.Models
{
    public partial class ProductData
    {
        public bool DeleteProduct()
        {
            System.Diagnostics.Debug.WriteLine(HttpContext.Current.Server.MapPath(this.ImgUrl));


            var root = Path.GetPathRoot(HostingEnvironment.MapPath("~/Content/Image"));
            var target = HttpContext.Current.Server.MapPath(this.ImgUrl);
            if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                if (false && File.Exists(target))    //
                {
                    File.Delete(target);
                    return true;
                }
            }
            return false;
        }
    }
}