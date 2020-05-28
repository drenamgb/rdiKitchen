using System;

namespace kitchen.Models
{
    public class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int TimeDelivery { get; set; }
        public DateTime HourStart { get; set; }
        public DateTime HourEnd { get; set; }
        public Item()
        {
            HourStart = DateTime.Now;
        }
    }

}