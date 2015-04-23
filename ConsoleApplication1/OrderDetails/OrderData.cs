using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.OrderDetails
{
    public class OrderData
    {
        public string UserName { get; set; }
        public string ShoppingWebSite { get; set; }
        public string OrderID { get; set; }
        public DateTime DateOrderReceivedToParse { get; set; }
        public DateTime OrderDate { get; set; }

        public string Address { get; set; }
        public double ShippingCost { get; set; } 
        public double TotalPrice { get; set; }
        public string Currency { get; set; }

        private LinkedList<ItemData> items;

        public OrderData()
        {
            items = new LinkedList<ItemData>(); 
        }

        public LinkedList<ItemData> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }


    }

}
