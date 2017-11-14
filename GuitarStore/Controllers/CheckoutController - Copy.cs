using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Checkout
        public ActionResult Index()
        {
            Models.CheckoutDetails details = new Models.CheckoutDetails();
            details.CurrentCart = Models.Cart.BuildCart(Request);

            return View(details);
        }

        // POST: Checkout
        [HttpPost]
        public ActionResult Index(Models.CheckoutDetails model)
        {
            model.CurrentCart = Models.Cart.BuildCart(Request);

            if (ModelState.IsValid)
            {
                //TODO: Persist this order to the database
                //TODO: send some confirmation emails to the person placing the order and the system admin
                //TODO: Reset the cart
                return RedirectToAction("Index", "Receipt");
            }
            return View(model);
        }
    }
}