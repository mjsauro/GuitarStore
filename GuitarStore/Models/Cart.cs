using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuitarStore.Models
{
    public class Cart
    {
        public Product[] Products { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingAndHandling { get; set; }
        public decimal Total { get; set; }


        public static Cart BuildCart(HttpRequestBase request)
        {
            Cart cart = new Cart();
            cart.Products = new Product[1];
            //For the moment, getting product data from cookies.
            //TODO: Pull this out of a database at some point!
            cart.Products[0] = new Product();
            cart.Products[0].Make = request.Cookies["productMake"].Value;
            cart.Products[0].Mod = request.Cookies["productModel"].Value;
            cart.Products[0].Price = decimal.Parse(request.Cookies["productPrice"].Value.Replace("$", ""));
            cart.Products[0].Quantity = int.Parse(request.Cookies["productQuantity"].Value);

            cart.SubTotal = cart.Products.Sum(x => x.Price * x.Quantity);

            cart.Tax = cart.SubTotal * .1025m;
            cart.ShippingAndHandling = cart.Products.Sum(x => x.Quantity) * 1m;
            cart.Total = cart.SubTotal + cart.Tax + cart.ShippingAndHandling;
            return cart;
        }
    }
}