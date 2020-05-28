using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace kitchen.Models
{
    public class Order
    {
        public int IdOrder { get; set; }
        public DateTime HourOrder { get; set; }
        public List<Item> ListItens { get; set; }
        public decimal TotalPrice { get; set; }
        public bool OrderReady { get; set; }
        public Order()
        {
            HourOrder = DateTime.Now;
            OrderReady = false;
        }

        public bool ricardo()
        {
            return false;
        }
    }
}