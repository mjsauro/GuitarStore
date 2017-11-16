using GuitarStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class CartController : Controller
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

        // GET: Cart
        public ActionResult Index()
        {
            Guid cartID = Guid.Parse(Request.Cookies["cartID"].Value);

            return View(db.Carts.Find(cartID));
        }

        // POST: Cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.Cart model)
        {
            var cart = db.Carts.Find(model.ID);
            for (int i = 0; i < model.CartProducts.Count; i++)
            {
                cart.CartProducts.ElementAt(i).Quantity = model.CartProducts.ElementAt(i).Quantity;
            }
            db.SaveChanges();
            return View(cart);
        }
    }
}