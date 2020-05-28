using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using kitchen.Models;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace kitchen.Controllers
{
    public class OrderController : BaseController
    {
        // GET: Home        
        static readonly string waitListFileName = WebConfigurationManager.AppSettings["waitListFileName"];

        public ActionResult Index()
        {
            return View("Order");
        }

        public bool ExistIdOrder(int idOrder, List<Order> listOrder)
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

        [HttpPost]
        public double CheckPrice(int idItem)
        {

            double price = 0;
            switch (idItem)
            {
                case 1:
                    price = 18.3;
                    break;

                case 2:
                    price = 14;
                    break;

                case 3:
                    price = 25.9;
                    break;

                case 4:
                    price = 12.5;
                    break;
                case 5:
                    price = 7.5;
                    break;
                case 6:
                    price = 6;
                    break;
                case 7:
                    price = 3.7;
                    break;
                case 8:
                    price = 4.5;
                    break;
            }

            return price;
        }

        [HttpPost]
        public int AddOrder(string listItensJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Item> listItem = serializer.Deserialize<List<Item>>(listItensJSON);

            decimal totalPriceOrder = 0;
            foreach (Item i in listItem)
            {
                totalPriceOrder += i.TotalPrice;
                i.HourEnd = i.HourStart.AddSeconds(i.TimeDelivery);
            }

            List<Order> listOrders = ReadFile(waitListFileName);

            //gera novo numero de pedido
            Random random = new Random();
            int idOrder = random.Next(1, 100);
            while (ExistIdOrder(idOrder, listOrders))
            {
                idOrder = random.Next(1, 100);
            }

            Order newOrder = new Order
            {
                IdOrder = idOrder,
                ListItens = listItem,
                HourOrder = DateTime.Now,
                TotalPrice = Math.Round(totalPriceOrder, 2)
            };

            AddOrderList(waitListFileName, newOrder);

            return newOrder.IdOrder;
        }
    }
}