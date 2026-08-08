using GuitarStore.Web.Data;
using GuitarStore.Web.Models;
using GuitarStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartRepository _carts;
    private readonly IProductRepository _products;

    public CartController(ICartRepository carts, IProductRepository products)
    {
        _carts = carts;
        _products = products;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewData["Title"] = "Your Cart";
        return View(await BuildCartAsync(ct));
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

    /// <summary>
    /// Joins cart lines to current catalog products. Lines whose product has since been
    /// deleted are dropped rather than throwing.
    /// </summary>
    private async Task<CartViewModel> BuildCartAsync(CancellationToken ct)
    {
        var cartId = CartCookie.Read(HttpContext);
        if (cartId is null)
        {
            return new CartViewModel();
        }

        var items = await _carts.GetItemsAsync(cartId, ct);
        if (items.Count == 0)
        {
            return new CartViewModel { CartId = cartId };
        }

        var products = await _products.GetByIdsAsync(items.Select(i => i.ProductId), ct);
        var byId = products.ToDictionary(p => p.Id);

        var lines = items
            .Where(item => byId.ContainsKey(item.ProductId))
            .Select(item => new CartLine { Product = byId[item.ProductId], Quantity = item.Quantity })
            .ToList();

        return new CartViewModel { CartId = cartId, Lines = lines };
    }
}
