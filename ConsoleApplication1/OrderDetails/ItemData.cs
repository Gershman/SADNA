using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.OrderDetails
{
    public class ItemData
    {
        public string Name { get; set; }
        public string ItemIDWebSite { get; set; }
        public double ItemPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ImageURL { get; set; }
        public double ShippingCost { get; set; }
        public DateTime ETA { get; set; }
       
    }
}
