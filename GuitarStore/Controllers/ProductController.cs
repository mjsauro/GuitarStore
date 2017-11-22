using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitarStore.Models;

namespace GuitarStore.Controllers
{
    public class ProductController : Controller
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

        // GET: Product/List
        public ActionResult List(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                //Get all records
                return View(db.Products);
            }

            return View(db.Products.Where(x => x.ProductTypeName == id));
        }

        // GET: Product
        public ActionResult Index(int? id)
        {
            //Get a record by its primary key
            return View(db.Products.Find(id));
        }

        [HttpPost]
        public ActionResult Index(Product model)
        {
            //TODO: Save the posted information to a database!
            Guid cartID;
            Cart cart = null;
            if (Request.Cookies.AllKeys.Contains("cartID"))
            {
                cartID = Guid.Parse(Request.Cookies["cartID"].Value);
                cart = db.Carts.Find(cartID);
            }
            if (cart == null)
            {
                cartID = Guid.NewGuid();
                cart = new Cart
                {
                    ID = cartID,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };
                db.Carts.Add(cart);
                Response.AppendCookie(new HttpCookie("cartID", cartID.ToString()));
            }

            CartProduct cartProduct = cart.CartProducts.FirstOrDefault(x => x.ProductID == model.ID);
            if (cartProduct == null)
            {
                cartProduct = new CartProduct
                {
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    ProductID = model.ID,
                    Quantity = 0
                };
                cart.CartProducts.Add(cartProduct);
            }

            cartProduct.Quantity += model.Quantity ?? 1;
            cartProduct.DateModified = DateTime.UtcNow;
            cart.DateModified = DateTime.UtcNow;

            db.SaveChanges();
            TempData.Add("NewItem", "Your item has been added to the cart!");
            //TODO: build up the cart controller!
            return RedirectToAction("Index", "Cart");
        }
    }
}