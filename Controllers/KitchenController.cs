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
    public class KitchenController : CommomController
    {
        // GET: Kitchen
        static readonly string waitListFileName = WebConfigurationManager.AppSettings["waitListFileName"];
        static readonly string deliveryListFileName = WebConfigurationManager.AppSettings["deliveryListFileName"];
        static readonly string directoryPath = WebConfigurationManager.AppSettings["directoryPath"];

        public ActionResult Index()
        {
            return View("Kitchen");
        }

        [HttpPost]
        public JsonResult UpdateListOrder()
        {
            List<Order> waitListOrder = ReadFile(waitListFileName);
            List<Order> deliveryListOrder = ReadFile(deliveryListFileName);


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

            UpdateFile(waitListFileName, waitListOrder);
            deliveryListOrder = deliveryListOrder.OrderByDescending(x => x.HourOrder).ToList<Order>();
            UpdateFile(deliveryListFileName, deliveryListOrder);

            return Json(new { waitListOrder, deliveryListOrder });
        }
        public void UpdateFile(string fileName, List<Order> listOrders)
        {
            string fileNamePath = Path.Combine(directoryPath, fileName);
            StreamWriter sw = new StreamWriter(fileNamePath);

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
    }
}