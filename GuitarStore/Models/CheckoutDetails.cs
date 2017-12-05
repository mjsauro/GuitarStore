using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Braintree;

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

        [Required]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; }

        [Required]
        [Display(Name = "Credit Card Number")]
        public string CreditCardNumber { get; set; }

        [Required]
        [Display(Name = "Card Verification Value (CVV)")]
        public string CVV { get; set; }

        [Required]
        [Display(Name = "Credit Card Expiration")]
        public string ExpirationMonth { get; set; }

        [Required]
        public string ExpirationYear { get; set; }

        public Braintree.Address[] Addresses { get; internal set; }
        public Braintree.CreditCard[] CreditCards { get; internal set; }
    }
}