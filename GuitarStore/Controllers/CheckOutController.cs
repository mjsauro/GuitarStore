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
                var o = new Order()
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
                };
                db.Orders.Add(o);
                db.SaveChanges();

                if (User.Identity.IsAuthenticated == false)
                {
                    return RedirectToAction("Index", "Home");
                }

                string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
                string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
                string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
                string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];
                Braintree.BraintreeGateway gateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);

                Braintree.TransactionRequest transaction = new Braintree.TransactionRequest();
                //transaction.Amount = 1m;    //I can hard-code a dollar amount for now to test everything else
                transaction.Amount = o.SubTotal + o.ShippingAndHandling + o.Tax;
                transaction.TaxAmount = o.Tax;
                //https://developers.braintreepayments.com/reference/general/testing/ruby
                transaction.CreditCard = new Braintree.TransactionCreditCardRequest
                {
                    CardholderName = model.CardholderName,
                    CVV = model.CVV,
                    Number = model.CreditCardNumber,
                    ExpirationYear = model.ExpirationYear,
                    ExpirationMonth = model.ExpirationMonth
                };

                var result = gateway.Transaction.Sale(transaction);

                //Reset the cart
                Response.SetCookie(new HttpCookie("cartID") { Expires = DateTime.UtcNow });
                db.CartProducts.RemoveRange(model.CurrentCart.CartProducts);
                db.Carts.Remove(model.CurrentCart);

                return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
            }
            return View(model);
        }
    }
}