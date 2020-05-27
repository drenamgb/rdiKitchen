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
        public JsonResult CheckPrice(int idItem)
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

            return Json(price);
        }

        public List<Order> ReadFile(string path)
        {
            List<Order> listOrders = new List<Order>();

            StreamReader sr = new StreamReader(path);

            Order order = new Order
            {
                ListItens = new List<Item>()
            };
            try
            {
                while (!sr.EndOfStream)
                {
                    string linha = sr.ReadLine();
                    string[] infoPedido = linha.Split('|');

                    if (infoPedido[0].Trim() == "Pedido")
                    {
                        if (order.ListItens.Count > 0)
                            listOrders.Add(order);

                        order = new Order
                        {
                            IdOrder = Convert.ToInt32(infoPedido[1]),
                            TotalPrice = Convert.ToDecimal(infoPedido[2]),
                            HourOrder = Convert.ToDateTime(infoPedido[3]),
                            ListItens = new List<Item>()
                        };
                    }
                    else
                    {
                        Item item = new Item
                        {
                            Name = infoPedido[1],
                            Quantity = Convert.ToInt32(infoPedido[2]),
                            Price = Convert.ToDecimal(infoPedido[3]),
                            TotalPrice = Convert.ToDecimal(infoPedido[4]),
                            TimeDelivery = Convert.ToInt32(infoPedido[5]),
                            HourStart = Convert.ToDateTime(infoPedido[6]),
                            HourEnd = Convert.ToDateTime(infoPedido[7])
                        };
                        order.ListItens.Add(item);
                    };

                }
                listOrders.Add(order);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
            }

            return listOrders;
        }

        public int AddOrder(string listItensJSON)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Item> listItem = serializer.Deserialize<List<Item>>(listItensJSON);

            //bloco para criar o diretório e os arquivos

            string waitListPath = Path.Combine(directoryPath, waitListFileName);
            string deliveryListPath = Path.Combine(directoryPath, deliveryListFileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                using (System.IO.File.Create(waitListPath)) { }
                using (System.IO.File.Create(deliveryListPath)) { }
            }

            decimal totalPriceOrder = 0;
            foreach (Item i in listItem)
            {
                totalPriceOrder += i.TotalPrice;
                i.HourEnd = i.HourStart.AddSeconds(i.TimeDelivery);
            }

            List<Order> listOrders = ReadFile(waitListPath);

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


            //gravar o pedido no arquivo
            StreamWriter sw = new StreamWriter(waitListPath, true);

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