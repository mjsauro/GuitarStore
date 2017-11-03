using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitarStore.Models;

namespace GuitarStore.Controllers
{
    public class GuitarController : Controller
    {
        public ActionResult List()
        {
            List<Guitar> guitar = new List<Guitar>();
            guitar.Add(new Guitar
            {
                ID = 1,
                Make = "Fender",
                Mod = "Stratocaster",
                Color = "White",
                Description = "A true classic from the 1950s",
                Image = "/images/strat.jpg",
                Price = 599m
            });

            guitar.Add(new Guitar
            {
                ID = 2,
                Make = "Gibson",
                Mod = "Les Paul",
                Color = "Sunburst",
                Description = "A perfect balance of clean and distorted tones.",
                Image = "/images/lespaul.jpg",
                Price = 699m
            });

            guitar.Add(new Guitar
            {
                ID = 3,
                Make = "Gibson",
                Mod = "Explorer",
                Color = "Black",
                Description = "Featuring two humbucking pickups with a serious crunch.",
                Image = "/images/explorer.jpg",
                Price = 299m
            });
            return View(guitar);
        }

        // GET: Guitar
        public ActionResult Index(int id)
        {
            var guitar = new Models.Guitar();
            if (id == 1)
            {
                guitar.Make = "Fender";
                guitar.Mod = "Stratocaster";
                guitar.Color = "White";
                guitar.Description = "A true classic from the 1950s.";
                guitar.Image = "/images/strat.jpg";
                guitar.Price = 599m;
            }

            else if (id == 2)
            {
                guitar.Make = "Gibson";
                guitar.Mod = "Les Paul";
                guitar.Color = "Sunburst";
                guitar.Description = "A perfect balance of clean and distorted tones.";
                guitar.Image = "/images/lespaul.jpg";
                guitar.Price = 699m;
            }

            else if (id == 3)
            {
                guitar.Make = "Gibson";
                guitar.Mod = "Explorer";
                guitar.Color = "Black";
                guitar.Description = "Featuring two humbucking pickups with a serious crunch.";
                guitar.Image = "/images/explorer.jpg";
                guitar.Price = 299m;
            }
            else
            {
                return HttpNotFound("This product does not exist.");
            }

            return View(guitar);
        }

        [HttpPost]
        public ActionResult Index(Guitar model, string make)
        {
            //TODO: Save the posted information to a database!

            HttpContext.Session.Add("guitarMake", model.Make);
            HttpContext.Session.Add("guitarModel", model.Mod);
            HttpContext.Session.Add("guitarPrice", model.Price.ToString("C"));
            HttpContext.Session.Add("guitarQuantity", model.Quantity.ToString());

            //TODO: Rip out this cookie code later - we're going to use it for now to mock up site functionality
            Response.AppendCookie(new HttpCookie("guitarMake", model.Make));
            Response.AppendCookie(new HttpCookie("guitarModel", model.Mod));
            Response.AppendCookie(new HttpCookie("guitarPrice", model.Price.ToString("C")));
            Response.AppendCookie(new HttpCookie("guitarQuantity", model.Quantity.ToString()));


            //TODO: build up the cart controller!
            return RedirectToAction("Index", "Cart");
        }
    }
}