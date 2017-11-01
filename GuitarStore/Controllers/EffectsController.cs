using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GuitarStore.Models;

namespace GuitarStore.Models
{
    public class EffectsController : Controller
    {

        public ActionResult List()
        {
            List<Effects> effects = new List<Effects>();

            effects.Add(new Effects
            {
                ID = 1,
                Make = "Boss",
                Model = "Metal Zone",
                Type = "Distortion Pedal",
                Description = "An effects pedal for bone crunching metal enthusiasts!",
                Image = "/images/metalzone.jpg",
                Price = 34m,
            });

            effects.Add(new Effects
            {
                ID = 2,
                Make = "Dunlop",
                Model = "CryBaby Wah",
                Type = "Wah Pedal",
                Description = "Add great wah effects to your guitar solos.",
                Image = "/images/wah.jpg",
                Price = 49m
            });

            effects.Add(new Effects
            {
                ID = 3,
                Make = "Digitech",
                Model = "Whammy",
                Type = "Tone shifter",
                Description = "Used by guitar legend Tom Morello of Rage Against the Machine fame.",
                Image = "/images/whammy.jpg",
                Price = 79m
            });

            return View(effects);
        }
        // GET: Effects
        public ActionResult Index(int id)
        {
            var effects = new Models.Effects();

            if (id == 1)
            {
                effects.Make = "Boss";
                effects.Model = "Metal Zone";
                effects.Type = "Distortion Pedal";
                effects.Description = "An effects pedal for bone crunching metal enthusiasts!";
                effects.Image = "/images/metalzone.jpg";
                effects.Price = 34m;
            }
            else if (id == 2)
            {
                effects.Make = "Dunlop";
                effects.Model = "CryBaby Wah";
                effects.Type = "Wah Pedal";
                effects.Description = "Add great wah effects to your guitar solos.";
                effects.Image = "/images/wah.jpg";
                effects.Price = 49m;
            }
            else if (id == 3)
            {
                effects.Make = "Digitech";
                effects.Model = "Whammy";
                effects.Type = "Tone shifter";
                effects.Description = "Used by guitar legend Tom Morello of Rage Against the Machine fame";
                effects.Image = "/images/whammy.jpg";
                effects.Price = 79m;
            }
            else
            {
                return HttpNotFound("This product cannot be found!");
            }

            return View(effects);
        }

    }
}
