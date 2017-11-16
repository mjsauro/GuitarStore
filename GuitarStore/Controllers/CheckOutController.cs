using GuitarStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class CheckoutController : Controller
    {
        protected GuitarStoreEntities db = new GuitarStoreEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Checkout
        public ActionResult Index()
        {
            Models.CheckoutDetails details = new Models.CheckoutDetails();
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            details.CurrentCart = db.Carts.Find(cartID);

            return View(details);
        }

        // POST: Checkout
        [HttpPost]
        public ActionResult Index(Models.CheckoutDetails model)
        {
            //model.CurrentCart = Models.Cart.BuildCart(Request);
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            model.CurrentCart = db.Carts.Find(cartID);

            if (ModelState.IsValid)
            {
                string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8);
                db.Orders.Add(new Order
                {
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    TrackingNumber = trackingNumber,
                    ShippingAndHandling = model.CurrentCart.CartProducts.Sum(x => x.Quantity),
                    Tax = (model.CurrentCart.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0) * .1025m,
                    SubTotal = model.CurrentCart.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0,
                    Email = model.ContactEmail,
                    PurchaserName = model.ContactName,
                    ShippingAddress1 = model.ShippingAddress,
                    ShippingCity = model.ShippingCity,
                    ShippingPostalCode = model.ShippingPostalCode,
                    ShippingState = model.ShippingState
                });

                db.SaveChanges();
                //TODO: send some confirmation emails to the person placing the order and the system admin
                //TODO: Reset the cart
                return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
            }
            return View(model);
        }
    }
}