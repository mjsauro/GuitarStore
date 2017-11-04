using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Models.Cart.BuildCart(Request);

            return View(cart);
        }



        // POST: Cart
        [HttpPost]
        public ActionResult Index(Models.Cart model)
        {
            Response.AppendCookie(new HttpCookie("productQuantity", model.Products[0].Quantity.ToString()));

            model.SubTotal = model.Products.Sum(x => x.Price * x.Quantity);

            model.Tax = model.SubTotal * .1025m;

            if (model.SubTotal < 500)
            {
                model.ShippingAndHandling = model.Products.Sum(x => x.Quantity) * 29m;
            }
            else
            {
                model.ShippingAndHandling = 0;
            }

            model.Total = model.SubTotal + model.Tax + model.ShippingAndHandling;

            return View(model);
        }
    }
}