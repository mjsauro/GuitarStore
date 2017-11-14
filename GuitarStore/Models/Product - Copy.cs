using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitarStore.Models
{
    public class Product
    {
        //all products
        public string ID { get; set; }

        public string ProductType { get; set; }
        public string Make { get; set; }
        public string Mod { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        //guitars
        public string Color { get; set; }

        //amps
        public string Watts { get; set; }

        public string Size { get; set; }

        //effects
        public string EffectType { get; set; }
    }
}