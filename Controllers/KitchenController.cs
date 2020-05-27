using kitchen.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace kitchen.Controllers
{
    public class KitchenController : Controller
    {
        // GET: Kitchen
        static readonly string waitListPath = WebConfigurationManager.AppSettings["waitListPath"];
        static readonly string deliveryListPath = WebConfigurationManager.AppSettings["deliveryListPath"];

        public ActionResult Index()
        {
            return View("Kitchen");
        }

        [HttpPost]
        public JsonResult UpdateOrder()
        {
            List<Order> waitListOrder = ReadFile(waitListPath);

            if (!System.IO.File.Exists(deliveryListPath))
            {
                using (System.IO.File.Create(deliveryListPath)) { }
            }
            List<Order> deliveryListOrder = ReadFile(deliveryListPath);


            bool allItemReady = true;
            foreach (Order order in waitListOrder)
            {
                List<Item> listItens = order.ListItens;

                foreach (Item i in listItens)
                {
                    TimeSpan ts = i.HourEnd - DateTime.Now;
                    if (Convert.ToInt32(ts.TotalSeconds) > 1)
                    {
                        i.TimeDelivery--;
                        allItemReady = false;
                    }
                };

                order.OrderReady = allItemReady;

                if (order.OrderReady)
                {
                    deliveryListOrder.Add(order);
                }
            }

            waitListOrder.RemoveAll(x => x.OrderReady == true);

            UpdateFile(waitListPath, waitListOrder);
            UpdateFile(deliveryListPath, deliveryListOrder);

            return Json(new { waitListOrder, deliveryListOrder });
        }
        public void UpdateFile(string path, List<Order> listOrders)
        {

            StreamWriter sw = new StreamWriter(path);

            try
            {
                foreach (Order order in listOrders)
                {
                    sw.Write("Pedido|");
                    sw.Write(order.IdOrder + "|");
                    sw.Write(order.TotalPrice + "|");
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

    }
}