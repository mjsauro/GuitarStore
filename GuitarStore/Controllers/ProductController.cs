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

        public ActionResult List()
        {
            List<Product> product = new List<Product>();
            product.Add(new Product
            {
                ID = "g1",
                ProductType = "Guitar",
                Make = "Fender",
                Mod = "Stratocaster",
                Color = "White",
                Description = "A true classic from the 1950s",
                Image = "/images/strat.jpg",
                Price = 599m,
                
            });

            product.Add(new Product
            {
                ID = "g2",
                ProductType = "Guitar",
                Make = "Gibson",
                Mod = "Les Paul",
                Color = "Sunburst",
                Description = "A perfect balance of clean and distorted tones.",
                Image = "/images/lespaul.jpg",
                Price = 699m
            });

            product.Add(new Product
            {
                ID = "g3",
                ProductType = "Guitar",
                Make = "Gibson",
                Mod = "Explorer",
                Color = "Black",
                Description = "Featuring two humbucking pickups with a serious crunch.",
                Image = "/images/explorer.jpg",
                Price = 299m
            });

            product.Add(new Product
            {
                ID = "a1",
                ProductType = "Amplifier",
                Make = "Marshall",
                Mod = "JCM2000",
                Watts = "100W",
                Size = "24 x 24",
                Description = "Trademark Marshall Sound",
                Image = "/images/marshall.jpg",
                Price = 199m
            });

            product.Add(new Product
            {
                ID = "a2",
                ProductType = "Amplifier",
                Make = "Fender",
                Mod = "Twinspeaker",
                Watts = "50W",
                Size = "10 x 10",
                Description = "Packs a double punch",
                Image = "/images/fender.jpg",
                Price = 299m
            });

            product.Add(new Product
            {
                ID = "a3",
                ProductType = "Amplifier",
                Make = "Vox",
                Mod = "Tube",
                Watts = "75W",
                Size = "40 x 40",
                Description = "A classic tube amp",
                Image = "/images/vox.jpg",
                Price = 399m,
            });

            product.Add(new Product
            {
                ProductType = "Effect",
                ID = "e1",
                Make = "Boss",
                Mod = "Metal Zone",
                EffectType = "Distortion Pedal",
                Description = "An effects pedal for bone crunching metal enthusiasts!",
                Image = "/images/distortionpedal.jpg",
                Price = 34m,
            });

            product.Add(new Product
            {
                ProductType = "Effect",
                ID = "e2",
                Make = "Dunlop",
                Mod = "CryBaby Wah",
                EffectType = "Wah Pedal",
                Description = "Add great wah effects to your guitar solos.",
                Image = "/images/wah.jpg",
                Price = 49m
            });

            product.Add(new Product
            {
                ProductType = "Effect",
                ID = "e3",
                Make = "Digitech",
                Mod = "Whammy",
                EffectType = "Tone shifter",
                Description = "Used by guitar legend Tom Morello of Rage Against the Machine fame.",
                Image = "/images/whammy.gif",
                Price = 79m
            });

            return View(product);
        }

        // GET: Products
        public ActionResult Index(string id)
        {
            var product = new Product();

            switch (id)
            {
                case "g1":
                    product.ProductType = "Guitar";
                    product.Make = "Fender";
                    product.Mod = "Stratocaster";
                    product.Color = "White";
                    product.Description = "A true classic from the 1950s.";
                    product.Image = "/images/strat.jpg";
                    product.Price = 599m;
                    product.Quantity = 1;
                    break;

                case "g2":
                    product.ProductType = "Guitar";
                    product.Make = "Gibson";
                    product.Mod = "Les Paul";
                    product.Color = "Sunburst";
                    product.Description = "A perfect balance of clean and distorted tones.";
                    product.Image = "/images/lespaul.jpg";
                    product.Price = 699m;
                    product.Quantity = 1;
                    break;

                case "g3":
                    product.ProductType = "Guitar";
                    product.Make = "Gibson";
                    product.Mod = "Explorer";
                    product.Color = "Black";
                    product.Description = "Featuring two humbucking pickups with a serious crunch.";
                    product.Image = "/images/explorer.jpg";
                    product.Price = 299m;
                    product.Quantity = 1;
                    break;

                case "a1":
                    product.ProductType = "Amplifier";
                    product.Make = "Marshall";
                    product.Mod = "JCM2000";
                    product.Watts = "100W";
                    product.Size = "24 x 24";
                    product.Description = "Trademark Marshall Sound";
                    product.Image = "/images/marshall.jpg";
                    product.Price = 199m;
                    product.Quantity = 1;
                    break;

                case "a2":
                    product.ProductType = "Amplifier";
                    product.Make = "Fender";
                    product.Mod = "Twinspeaker";
                    product.Watts = "50W";
                    product.Size = "10 x 10";
                    product.Description = "Packs a double punch";
                    product.Image = "/images/fender.jpg";
                    product.Price = 299m;
                    product.Quantity = 1;
                    break;

                case "a3":
                    product.ProductType = "Amplifier";
                    product.Make = "Vox";
                    product.Mod = "Tube";
                    product.Watts = "75W";
                    product.Size = "40 x 40";
                    product.Description = "A classic tube amp";
                    product.Image = "/images/vox.jpg";
                    product.Price = 399m;
                    product.Quantity = 1;
                    break;

                case "e1":
                    product.ProductType = "Effect";
                    product.Make = "Boss";
                    product.Mod = "Metal Zone";
                    product.EffectType = "Distortion Pedal";
                    product.Description = "An effects pedal for bone crunching metal enthusiasts!";
                    product.Image = "/images/distortionpedal.jpg";
                    product.Price = 34m;
                    product.Quantity = 1;
                    break;

                case "e2":
                    product.ProductType = "Effect";
                    product.Make = "Dunlop";
                    product.Mod = "CryBaby Wah";
                    product.EffectType = "Wah Pedal";
                    product.Description = "Add great wah effects to your guitar solos.";
                    product.Image = "/images/wah.jpg";
                    product.Price = 49m;
                    product.Quantity = 1;
                    break;

                case "e3":
                    product.ProductType = "Effect";
                    product.Make = "Digitech";
                    product.Mod = "Whammy";
                    product.EffectType = "Tone shifter";
                    product.Description = "Used by guitar legend Tom Morello of Rage Against the Machine fame";
                    product.Image = "/images/whammy.gif";
                    product.Price = 79m;
                    product.Quantity = 1;
                    break;

                default:
                    return HttpNotFound("This product does not exist.");
            }

            return View(product);
        }

        [HttpPost]
        public ActionResult Index(Product model, string make)
        {
            //TODO: Save the posted information to a database!

            HttpContext.Session.Add("productMake", model.Make);
            HttpContext.Session.Add("productModel", model.Mod);
            HttpContext.Session.Add("productPrice", model.Price.ToString("C"));
            HttpContext.Session.Add("productQuantity", model.Quantity.ToString());

            //TODO: Rip out this cookie code later - we're going to use it for now to mock up site functionality
            Response.AppendCookie(new HttpCookie("productMake", model.Make));
            Response.AppendCookie(new HttpCookie("productModel", model.Mod));
            Response.AppendCookie(new HttpCookie("productPrice", model.Price.ToString("C")));
            Response.AppendCookie(new HttpCookie("productQuantity", model.Quantity.ToString()));


            //TODO: build up the cart controller!
            return RedirectToAction("Index", "Cart");
        }
    }
}