using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GuitarStore.Models
{
    public class CheckoutDetails
    {
        public Cart CurrentCart { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string ContactName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string ShippingAddress { get; set; }

        [Required]
        [Display(Name = "City")]
        public string ShippingCity { get; set; }

        [Required]
        [Display(Name = "State")]
        public string ShippingState { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        public string ShippingPostalCode { get; set; }

    }
}