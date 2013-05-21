﻿using ByteartRetail.DataObjects;
using ByteartRetail.Infrastructure.Communication;
using ByteartRetail.ServiceContracts;
using System;
using System.Web.Mvc;

namespace ByteartRetail.Web.MVC.Controllers
{
    [HandleError]
    public class HomeController : ControllerBase
    {
        public ActionResult Index(string categoryID = null, int pageNumber = 1)
        {
            return View();
        }

        public ActionResult Category(string categoryID = null, int pageNumber = 1)
        {
            ViewData["CategoryID"] = categoryID;
            ViewData["FromIndexPage"] = false;
            return View();
        }

        public ActionResult Product(string id)
        {
            using (var proxy = new ServiceProxy<IProductService>())
            {
                var product = proxy.Channel.GetProductByID(new Guid(id), QuerySpec.VerboseOnly);
                return View(product);
            }
        }

        [Authorize]
        public ActionResult AddToCart(string productID, string items)
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                int quantity;
                if (!int.TryParse(items, out quantity))
                    quantity = 1;
                proxy.Channel.AddProductToCart(UserID, new Guid(productID), quantity);
                return RedirectToAction("ShoppingCart");
            }
        }

        [Authorize]
        public ActionResult ShoppingCart()
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                var model = proxy.Channel.GetShoppingCart(UserID);
                return View(model);
            }
        }

        [Authorize]
        public ActionResult UpdateShoppingCartItem(string shoppingCartItemID, int quantity)
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                proxy.Channel.UpdateShoppingCartItem(new Guid(shoppingCartItemID), quantity);
                return Json(null);
            }
        }

        [Authorize]
        public ActionResult DeleteShoppingCartItem(string shoppingCartItemID)
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                proxy.Channel.DeleteShoppingCartItem(new Guid(shoppingCartItemID));
                return Json(null);
            }
        }

        [Authorize]
        public ActionResult Checkout()
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                var model = proxy.Channel.Checkout(UserID);
                return View(model);
            }
        }

        [Authorize]
        public ActionResult SalesOrders()
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                var model = proxy.Channel.GetSalesOrdersForUser(UserID);
                return View(model);
            }
        }

        [Authorize]
        public ActionResult SalesOrder(string id)
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                var model = proxy.Channel.GetSalesOrder(new Guid(id));
                return View(model);
            }
        }

        [Authorize]
        public ActionResult Confirm(string id)
        {
            using (var proxy = new ServiceProxy<IOrderService>())
            {
                proxy.Channel.Confirm(new Guid(id));
                return RedirectToSuccess("确认收货成功！", "SalesOrders");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Please use the following information to contact the author.";
            return View();
        }

        public ActionResult SuccessPage(string pageTitle, string pageMessage = null, string retAction = "Index", string retController = "Home", int waitSeconds = 5)
        {
            ViewBag.PageTitle = pageTitle;
            ViewBag.PageMessage = pageMessage;
            ViewBag.RetAction = retAction;
            ViewBag.RetController = retController;
            ViewBag.WaitSeconds = waitSeconds;
            return View();
        }
    }
}
