using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitarStore.Models;

namespace GuitarStore.Controllers
{
    public class AmplifierController : Controller
    {
        public ActionResult List()
        {
            List<Amplifier> amplifer = new List<Amplifier>();

            amplifer.Add(new Amplifier
            {
                ID = 1,
                Make = "Marshall",
                Model = "JCM2000",
                Watts = "100W",
                Size = "24 x 24",
                Description = "Trademark Marshall Sound",
                Image = "/images/marshall.jpg",
                Price = 199m
            });

            amplifer.Add(new Amplifier
            {
                ID = 2,
                Make = "Fender",
                Model = "Twinspeaker",
                Watts = "50W",
                Size = "10 x 10",
                Description = "Packs a double punch",
                Image = "/images/fender.jpg",
                Price = 299m
            });

            amplifer.Add(new Amplifier
            {
                ID = 3,
                Make = "Vox",
                Model = "Tube",
                Watts = "75W",
                Size = "40 x 40",
                Description = "A classic tube amp",
                Image = "/images/vox.jpg",
                Price = 399m,
            });

            return View(amplifer);
        }

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

            else if (id == 2)
            {
                amplifier.Make = "Fender";
                amplifier.Model = "Twinspeaker";
                amplifier.Watts = "50W";
                amplifier.Size = "10 x 10";
                amplifier.Description = "Packs a double punch";
                amplifier.Image = "/images/fender.jpg";
                amplifier.Price = 299m;
            }

            else if (id == 3)
            {
                amplifier.Make = "Vox";
                amplifier.Model = "Tube";
                amplifier.Watts = "75W";
                amplifier.Size = "40 x 40";
                amplifier.Description = "A classic tube amp";
                amplifier.Image = "/images/vox.jpg";
                amplifier.Price = 399m;
            }
            else
            {
                return HttpNotFound("This product does not exist.");
            }

            return View(amplifier);

        }
    }
}