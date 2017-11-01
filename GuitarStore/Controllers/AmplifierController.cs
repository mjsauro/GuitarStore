using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuitarStore.Controllers
{
    public class AmplifierController : Controller
    {
        // GET: Amplifier
        public ActionResult Index(int id)
        {
            var amplifier = new Models.Amplifier();
            if (id == 1)
            {
                amplifier.Make = "Marshall";
                amplifier.Model = "JCM2000";
                amplifier.Watts = "100W";
                amplifier.Size = "24 x 24";
                amplifier.Description = "Trademark Marshall Sound";
                amplifier.Image = "/images/marshall.jpg";
                amplifier.Price = 199m;
            }

            if (id == 2)
            {
                amplifier.Make = "Fender";
                amplifier.Model = "Twinspeaker";
                amplifier.Watts = "50W";
                amplifier.Size = "10 x 10";
                amplifier.Description = "Packs a double punch";
                amplifier.Image = "/images/fender.jpg";
                amplifier.Price = 299m;
            }

            if (id == 3)
            {
                amplifier.Make = "Vox";
                amplifier.Model = "Tube";
                amplifier.Watts = "75W";
                amplifier.Size = "40 x 40";
                amplifier.Description = "A classic tube amp";
                amplifier.Image = "/images/vox.jpg";
                amplifier.Price = 399m;
            }

            return View(amplifier);

        }
    }
}