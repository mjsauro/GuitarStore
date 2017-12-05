using Braintree;
using GuitarStore.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Configuration;
using System.Linq;
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
            details.Addresses = new Braintree.Address[0];
            details.CreditCards = new Braintree.CreditCard[0];
            if (User.Identity.IsAuthenticated)
            {
                string merchantId = ConfigurationManager.AppSettings["Braintree.MerchantId"];
                string environment = ConfigurationManager.AppSettings["Braintree.Environment"];
                string publicKey = ConfigurationManager.AppSettings["Braintree.PublicKey"];
                string privateKey = ConfigurationManager.AppSettings["Braintree.PrivateKey"];
                BraintreeGateway gateway = new Braintree.BraintreeGateway(environment, merchantId, publicKey, privateKey);

                var customerGateway = gateway.Customer;
                Braintree.CustomerSearchRequest query = new Braintree.CustomerSearchRequest();
                query.Email.Is(User.Identity.Name);
                var matchedCustomers = customerGateway.Search(query);
                Braintree.Customer customer = null;
                if (matchedCustomers.Ids.Count == 0)
                {
                    CustomerRequest newCustomer = new Braintree.CustomerRequest();
                    newCustomer.Email = User.Identity.Name;

                    var result = customerGateway.Create(newCustomer);
                    customer = result.Target;
                }
                else
                {
                    customer = matchedCustomers.FirstItem;
                }
                details.Addresses = customer.Addresses;
                details.CreditCards = customer.CreditCards;
            }

            return View(details);
        }

        // POST: Checkout
        [HttpPost]
        public ActionResult Index(Models.CheckoutDetails model, string addressId)
        {
            //model.CurrentCart = Models.Cart.BuildCart(Request);
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            model.CurrentCart = db.Carts.Find(cartID);
            model.Addresses = new Braintree.Address[0];
            model.CreditCards = new Braintree.CreditCard[0];
            if (ModelState.IsValid)
            {
                string trackingNumber = Guid.NewGuid().ToString().Substring(0, 8);
                decimal tax = (model.CurrentCart.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0) * .1025m;
                decimal subTotal = model.CurrentCart.CartProducts.Sum(x => x.Product.Price * x.Quantity) ?? 0;
                decimal shipping = model.CurrentCart.CartProducts.Sum(x => x.Quantity);
                decimal total = subTotal + tax + shipping;

                #region pay for order

                GuitarStorePaymentService payments = new GuitarStorePaymentService();
                string email = User.Identity.IsAuthenticated ? User.Identity.Name : model.ContactEmail;
                string message = payments.AuthorizeCard(email, total, tax, trackingNumber, addressId, model.CardholderName, model.CVV, model.CreditCardNumber, model.ExpirationMonth, model.ExpirationYear);

                #endregion pay for order

                #region save order

                if (string.IsNullOrEmpty(message))
                {
                    Order o = new Order
                    {
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        TrackingNumber = trackingNumber,
                        ShippingAndHandling = shipping,
                        Tax = tax,
                        SubTotal = subTotal,
                        Email = model.ContactEmail,
                        PurchaserName = model.ContactName,
                        ShippingAddress1 = model.ShippingAddress,
                        ShippingCity = model.ShippingCity,
                        ShippingPostalCode = model.ShippingPostalCode,
                        ShippingState = model.ShippingState
                    };
                    db.Orders.Add(o);

                    db.SaveChanges();

                    #endregion save order

                    #region send email

                    SendThankYouEmail();
                    IRestResponse SendThankYouEmail()
                    {
                        RestClient client = new RestClient();
                        client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                        client.Authenticator =
                            new HttpBasicAuthenticator("api",
                                                        System.Configuration.ConfigurationManager.AppSettings["MailGun.PrivateKey"]);
                        RestRequest request = new RestRequest();
                        request.AddParameter("domain", "sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org", ParameterType.UrlSegment);
                        request.Resource = "{domain}/messages";
                        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa9cdb0fb3e0a4168a77655ff39fe11ae.mailgun.org>");
                        request.AddParameter("to", model.ContactEmail);
                        request.AddParameter("subject", String.Format("Thank you, {0}!", model.ContactName));
                        request.AddParameter("html", "<html><head><style> .strong { font-weight: bold; }</style> <title></title></head><body> <header> <div> <h1>Thank you, " + @model.ContactName + ", for your order!</h1> </div><div> <h3>Your receipt is below.</h3> </div></header> <table> <thead> <tr> <th class='strong'>Your receipt from Matt's Guitar Store</th> </tr></thead> <tbody> <tr> <td class='strong'>Purchaser Name:</td><td>" + @model.ContactName + "</td></tr><tr> <td class='strong'>Purchaser Email:</td><td>" + @model.ContactEmail + "</td></tr><tr> <td class='strong'>Shipping Address:</td><td>" + @model.ShippingAddress + "</td></tr><tr> <td class='strong'>City:</td><td>" + @model.ShippingCity + "</td></tr><tr> <td class='strong'>State:</td><td>" + @model.ShippingState + "</td></tr><tr> <td class='strong'>Zip:</td><td>" + @model.ShippingPostalCode + "</td></tr><tr> <td class='strong'>Cardholder Name:</td><td>" + @model.CardholderName + "</td></tr><tr> <td class='strong'>Card Number:</td><td>************" + @model.CreditCardNumber.Substring(12) + "</td></tr></tbody> </table> <footer> <h1>Thank you for shopping with us. We hope to see you again soon.</h1> </footer></body></html>");
                        request.Method = Method.POST;
                        return client.Execute(request);
                    }

                    #endregion send email

                    #region reset cart

                    Response.SetCookie(new System.Web.HttpCookie("cartID")
                    {
                        Expires = DateTime.UtcNow
                    });

                    db.CartProducts.RemoveRange(model.CurrentCart.CartProducts);
                    db.Carts.Remove(model.CurrentCart);
                    db.SaveChanges();

                    #endregion reset cart

                    return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
                }
                ModelState.AddModelError("CreditCardNumber", message);
            }
            return View(model);
        }
    }
}