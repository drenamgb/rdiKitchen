using kitchen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace kitchen.Controllers
{
    public class BaseController : Controller
    {
        static readonly string directoryPath = WebConfigurationManager.AppSettings["directoryPath"];

        public BaseController()
        {

        }

        public void checkDirectoryAndFile(string fileName)
        {
            string fileNamePath = Path.Combine(directoryPath, fileName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                if (!System.IO.File.Exists(fileNamePath))
                {
                    using (System.IO.File.Create(fileNamePath)) { }
                }
            }
            else
            {
                if (!System.IO.File.Exists(fileNamePath))
                {
                    using (System.IO.File.Create(fileNamePath)) { }
                }
            }
        }

        public List<Order> ReadFile(string fileName)
        {
            checkDirectoryAndFile(fileName);
            string fileNamePath = Path.Combine(directoryPath, fileName);
            StreamReader sr = new StreamReader(fileNamePath);

            try
            {
                Order order = new Order
                {
                    ListItens = new List<Item>()
                };

                List<Order> listOrders = new List<Order>();

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

                if (order.IdOrder == 0)
                    listOrders = new List<Order>();
                else
                    listOrders.Add(order);

                return listOrders;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
            }
        }

        public void AddOrderList(string fileName, Order newOrder)
        {
            string fileNamePath = Path.Combine(directoryPath, fileName);
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