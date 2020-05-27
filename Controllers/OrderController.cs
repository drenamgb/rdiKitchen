using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using kitchen.Models;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace kitchen.Controllers
{
    public class OrderController : CommomController
    {
        // GET: Home        
        static readonly string directoryPath = WebConfigurationManager.AppSettings["directoryPath"];
        static readonly string waitListFileName = WebConfigurationManager.AppSettings["waitListFileName"];
        static readonly string deliveryListFileName = WebConfigurationManager.AppSettings["deliveryListFileName"];

        public ActionResult Index()
        {
            return View("Order");
        }

        public bool checkIdOrder(int idOrder, List<Order> waitListOrder)
        {
            foreach (Order order in waitListOrder)
            {
                if (idOrder == order.IdOrder)
                {
                    return true;
                }
            }

            return false;
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

            Random random = new Random();
            int idOrder = random.Next(1, 100);
            while (checkIdOrder(idOrder, listOrders))
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

            string fileNamePath = Path.Combine(directoryPath, waitListFileName);
            StreamWriter sw = new StreamWriter(fileNamePath, true);

            try
            {

                sw.Write("Pedido|");
                sw.Write(newOrder.IdOrder + "|");
                sw.Write(newOrder.TotalPrice + "|");
                sw.Write(newOrder.HourOrder.ToString("HH:mm:ss"));
                sw.WriteLine();
                int cont = 1;
                foreach (Item item in newOrder.ListItens)
                {
                    sw.Write(cont++ + "|");
                    sw.Write(item.Name + "|");
                    sw.Write(item.Quantity + "|");
                    sw.Write(item.Price + "|");
                    sw.Write(item.TotalPrice + "|");
                    sw.Write(item.TimeDelivery + "|");
                    sw.Write(item.HourStart + "|");
                    sw.Write(item.HourEnd);
                    sw.WriteLine();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Close();
            }

            return idOrder;
        }
    }
}