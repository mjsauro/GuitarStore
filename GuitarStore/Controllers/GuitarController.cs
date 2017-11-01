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
                Model = "Stratocaster",
                Color = "White",
                Description = "A true classic from the 1950s",
                Image = "/images/strat.jpg",
                Price = 599m
            });

            guitar.Add(new Guitar
            {
                ID = 2,
                Make = "Gibson",
                Model = "Les Paul",
                Color = "Sunburst",
                Description = "A perfect balance of clean and distorted tones.",
                Image = "/images/lespaul.jpg",
                Price = 699m
            });

            guitar.Add(new Guitar
            {
                ID = 3,
                Make = "Gibson",
                Model = "Explorer",
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
                guitar.Model = "Stratocaster";
                guitar.Color = "White";
                guitar.Description = "A true classic from the 1950s.";
                guitar.Image = "/images/strat.jpg";
                guitar.Price = 599m;
            }

            else if (id == 2)
            {
                guitar.Make = "Gibson";
                guitar.Model = "Les Paul";
                guitar.Color = "Sunburst";
                guitar.Description = "A perfect balance of clean and distorted tones.";
                guitar.Image = "/images/lespaul.jpg";
                guitar.Price = 699m;
            }

            else if (id == 3)
            {
                guitar.Make = "Gibson";
                guitar.Model = "Explorer";
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
    }
}