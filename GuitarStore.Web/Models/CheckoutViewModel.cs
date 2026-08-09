using System.ComponentModel.DataAnnotations;

namespace GuitarStore.Web.Models;

/// <summary>
/// The checkout form. Card fields are used to authorize the sale and are then discarded —
/// only the last four digits reach the saved order.
/// </summary>
public class CheckoutViewModel
{
    /// <summary>Populated server-side for display; never trusted from the form post.</summary>
    public CartViewModel Cart { get; set; } = new();

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string ContactEmail { get; set; } = "";

    [Required]
    [Display(Name = "Name")]
    public string ContactName { get; set; } = "";

    [Required]
    [Display(Name = "Address")]
    public string ShippingAddress { get; set; } = "";

    [Required]
    [Display(Name = "City")]
    public string ShippingCity { get; set; } = "";

    [Required]
    [Display(Name = "State")]
    public string ShippingState { get; set; } = "";

    [Required]
    [Display(Name = "Zip Code")]
    public string ShippingPostalCode { get; set; } = "";

    [Required]
    [Display(Name = "Cardholder Name")]
    public string CardholderName { get; set; } = "";

    [Required]
    [Display(Name = "Credit Card Number")]
    public string CreditCardNumber { get; set; } = "";

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "The CVV is the 3 or 4 digit code on your card.")]
    [Display(Name = "CVV")]
    public string CVV { get; set; } = "";

    [Required]
    [Display(Name = "Expiration Month")]
    public string ExpirationMonth { get; set; } = "";

    [Required]
    [Display(Name = "Expiration Year")]
    public string ExpirationYear { get; set; } = "";
}
