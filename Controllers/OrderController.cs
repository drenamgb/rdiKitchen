using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using kitchen.Models;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace kitchen.Controllers
{
    public class OrderController : Controller
    {
        // GET: Home        
        static readonly string directoryPath = WebConfigurationManager.AppSettings["directoryPath"];
        static readonly string waitListFileNname = WebConfigurationManager.AppSettings["waitListFileName"];
        static readonly string deliveryListFileName = WebConfigurationManager.AppSettings["deliveryListFileName"];

        public ActionResult Index()
        {
            return View("Order");
        }


        [HttpPost]
        public JsonResult CheckPrice(int idItem)
        {

            double price = 0;
            switch (idItem)
            {
                case 1:
                    price = 10.3;
                    break;

                case 2:
                    price = 18.9;
                    break;

                case 3:
                    price = 5.7;
                    break;

                case 4:
                    price = 22.5;
                    break;
            }

            return Json(price);
        }

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

            List<Order> listOrders = new List<Order>();

            Order newOrder = new Order
            {
                ListItens = listItem,
                HourOrder = DateTime.Now,
                TotalPrice = Math.Round(totalPriceOrder, 2)
            };

            listOrders.Add(newOrder);

            //bloco para criar o diretório e os arquivos

            string waitListPath = Path.Combine(directoryPath, waitListFileNname);
            string deliveryListPath = Path.Combine(directoryPath, deliveryListFileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                using (System.IO.File.Create(waitListPath)) { }
                using (System.IO.File.Create(deliveryListPath)) { }
            }


            //gravar o pedido no arquivo
            StreamWriter sw = new StreamWriter(waitListPath, true);
            Random random = new Random();
            int idOrder = random.Next(1, 100);
            try
            {
                foreach (Order order in listOrders)
                {
                    sw.Write("Pedido|");
                    sw.Write(idOrder + "|");
                    sw.Write(totalPriceOrder + "|");
                    sw.Write(order.HourOrder.ToString("HH:mm:ss"));
                    sw.WriteLine();
                    int cont = 1;
                    foreach (Item item in order.ListItens)
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