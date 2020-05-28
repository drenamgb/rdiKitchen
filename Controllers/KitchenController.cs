using kitchen.Models;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;

namespace kitchen.Controllers
{
    public class KitchenController : BaseController
    {
        // GET: Kitchen
        static readonly string waitListFileName = WebConfigurationManager.AppSettings["waitListFileName"];
        static readonly string deliveryListFileName = WebConfigurationManager.AppSettings["deliveryListFileName"];
        static readonly string directoryPath = WebConfigurationManager.AppSettings["directoryPath"];

        public ActionResult Index()
        {
            return View("Kitchen");
        }

        public JsonResult DeleteDeliveryList()
        {
            CheckAndCreateDirectoryAndFile(deliveryListFileName);
            DeleteFile(deliveryListFileName);
            return Json(true);
        }

        public JsonResult UpdateListOrder()
        {
            List<Order> waitListOrder = ReadFile(waitListFileName);
            List<Order> deliveryListOrder = ReadFile(deliveryListFileName);

            bool allItemReady = true;
            foreach (Order order in waitListOrder)
            {
                List<Item> listItens = order.ListItens;
                allItemReady = true;
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
                    //deliveryListOrder.Add(order);
                    AddOrderList(deliveryListFileName, order);
                }
            }

            waitListOrder.RemoveAll(x => x.OrderReady == true);

            UpdateFile(waitListFileName, waitListOrder);

            //deliveryListOrder = deliveryListOrder.OrderByDescending(x => x.HourOrder).ToList<Order>();
            //UpdateFile(deliveryListFileName, deliveryListOrder);

            return Json(new { waitListOrder, deliveryListOrder });
        }

    }
}