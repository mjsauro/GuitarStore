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
        public ActionResult Index(Product model, string make)
        {
            //TODO: Save the posted information to a database!

            HttpContext.Session.Add("productMake", model.Make);
            HttpContext.Session.Add("productModel", model.Model);
            HttpContext.Session.Add("productPrice", (model.Price ?? 0).ToString("C"));
            //HttpContext.Session.Add("productQuantity", model.Quantity.ToString());

            //TODO: Rip out this cookie code later - we're going to use it for now to mock up site functionality
            Response.AppendCookie(new HttpCookie("productMake", model.Make.Name));
            Response.AppendCookie(new HttpCookie("productModel", model.Model));
            Response.AppendCookie(new HttpCookie("productPrice", (model.Price ?? 0).ToString("C")));

            //TODO: build up the cart controller!
            return RedirectToAction("Index", "Cart");
        }
    }
}