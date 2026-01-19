using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlanCart.ViewModel
{
    public class MyCartViewModel
    {
        public int CartOfUserId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string ImgUrl { get; set; }

    }
}