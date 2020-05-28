using System;
using System.Collections.Generic;

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

        public static bool ExistIdOrder(int idOrder, List<Order> listOrder)
        {
            bool idOrderExist = true;
            while (idOrderExist)
            {
                foreach (Order order in listOrder)
                {
                    if (order.IdOrder == idOrder)
                    {
                        return true;
                    }
                }
                idOrderExist = false;
            }

            return idOrderExist;
        }
    }
}