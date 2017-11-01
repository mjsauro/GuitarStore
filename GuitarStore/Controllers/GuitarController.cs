using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class GuitarController : Controller
    {
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

            if (id == 2)
            {
                guitar.Make = "Gibson";
                guitar.Model = "Les Paul";
                guitar.Color = "Sunburst";
                guitar.Description = "A perfect balance of clean and distorted tones.";
                guitar.Image = "/images/lespaul.jpg";
                guitar.Price = 699m;
            }

            if (id == 3)
            {
                guitar.Make = "Gibson";
                guitar.Model = "Explorer";
                guitar.Color = "Black";
                guitar.Description = "Featuring two humbucking pickups with a serious crunch.";
                guitar.Image = "/images/explorer.jpg";
                guitar.Price = 299m;
            }

            return View(guitar);
            
        }
    }
}