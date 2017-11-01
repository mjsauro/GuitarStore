using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitarStore.Models
{
    public class Effects
    {
        public int ID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
    }
}