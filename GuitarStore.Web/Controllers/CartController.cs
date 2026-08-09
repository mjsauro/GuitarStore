using GuitarStore.Web.Data;
using GuitarStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartRepository _carts;
    private readonly IProductRepository _products;
    private readonly CartService _cartService;

    public CartController(ICartRepository carts, IProductRepository products, CartService cartService)
    {
        _carts = carts;
        _products = products;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewData["Title"] = "Your Cart";
        return View(await _cartService.BuildAsync(HttpContext, ct));
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity, CancellationToken ct)
    {
        var product = await _products.GetByIdAsync(productId, ct);
        if (product is null)
        {
            return NotFound();
        }

        var cartId = CartCookie.ReadOrCreate(HttpContext);
        await _carts.AddOrIncrementAsync(cartId, productId, Math.Max(1, quantity), ct);

        TempData["NewItem"] = $"{product.DisplayName} has been added to your cart.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Applies quantity edits from the cart table. Quantity 0 removes the line.</summary>
    [HttpPost]
    public async Task<IActionResult> UpdateQuantities(int[] productIds, int[] quantities, CancellationToken ct)
    {
        var cartId = CartCookie.Read(HttpContext);
        if (cartId is null)
        {
            return RedirectToAction(nameof(Index));
        }

        for (var i = 0; i < productIds.Length && i < quantities.Length; i++)
        {
            await _carts.SetQuantityAsync(cartId, productIds[i], quantities[i], ct);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ResetCart(CancellationToken ct)
    {
        var cartId = CartCookie.Read(HttpContext);
        if (cartId is not null)
        {
            await _carts.ClearAsync(cartId, ct);
            CartCookie.Clear(HttpContext);
        }

        return RedirectToAction(nameof(Index));
    }
}
