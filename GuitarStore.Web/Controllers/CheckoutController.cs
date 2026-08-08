using GuitarStore.Web.Data;
using GuitarStore.Web.Models;
using GuitarStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

public class CheckoutController : Controller
{
    private readonly ICartRepository _carts;
    private readonly IOrderRepository _orders;
    private readonly IPaymentService _payments;
    private readonly CartService _cartService;

    public CheckoutController(
        ICartRepository carts,
        IOrderRepository orders,
        IPaymentService payments,
        CartService cartService)
    {
        _carts = carts;
        _orders = orders;
        _payments = payments;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var cart = await _cartService.BuildAsync(HttpContext, ct);
        if (cart.IsEmpty)
        {
            // The legacy version threw a NullReferenceException here when there was no cart cookie.
            return RedirectToAction("Index", "Cart");
        }

        ViewData["Title"] = "Checkout";
        return View(new CheckoutViewModel
        {
            Cart = cart,
            ContactEmail = User.Identity?.IsAuthenticated == true ? User.Identity.Name ?? "" : ""
        });
    }

    [HttpPost]
    public async Task<IActionResult> Index(CheckoutViewModel model, CancellationToken ct)
    {
        var cart = await _cartService.BuildAsync(HttpContext, ct);
        if (cart.IsEmpty)
        {
            return RedirectToAction("Index", "Cart");
        }

        // Totals are always recomputed from the stored cart — never taken from the form.
        model.Cart = cart;
        ViewData["Title"] = "Checkout";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var trackingNumber = Guid.NewGuid().ToString()[..8].ToUpperInvariant();

        var payment = await _payments.AuthorizeAsync(
            new PaymentAuthorization(
                model.CreditCardNumber,
                model.CardholderName,
                model.CVV,
                model.ExpirationMonth,
                model.ExpirationYear,
                cart.Total,
                cart.Tax,
                trackingNumber,
                model.ContactEmail),
            ct);

        if (!payment.Approved)
        {
            ModelState.AddModelError(nameof(model.CreditCardNumber), payment.DeclineReason ?? "That payment couldn't be processed.");
            return View(model);
        }

        var now = DateTime.UtcNow;
        var order = new Order
        {
            TrackingNumber = trackingNumber,
            Email = model.ContactEmail,
            PurchaserName = model.ContactName,
            ShippingAddress = model.ShippingAddress,
            ShippingCity = model.ShippingCity,
            ShippingState = model.ShippingState,
            ShippingPostalCode = model.ShippingPostalCode,
            SubTotal = cart.SubTotal,
            ShippingAndHandling = cart.ShippingAndHandling,
            Tax = cart.Tax,
            CardLastFour = payment.CardLastFour,
            LineItems = cart.Lines
                .Select(line => new OrderLineItem
                {
                    ProductId = line.Product.Id,
                    ProductName = line.Product.DisplayName,
                    UnitPrice = line.Product.Price,
                    Quantity = line.Quantity
                })
                .ToList(),
            DateCreated = now,
            DateModified = now
        };

        await _orders.SaveAsync(order, ct);

        await _carts.ClearAsync(cart.CartId, ct);
        CartCookie.Clear(HttpContext);

        return RedirectToAction("Index", "Receipt", new { id = trackingNumber });
    }
}
