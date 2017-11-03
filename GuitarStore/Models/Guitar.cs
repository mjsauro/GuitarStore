using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitarStore.Models
{
    public class Guitar
    {
        public int ID { get; set; }
        public string Make { get; set;  }
        public string Mod { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}