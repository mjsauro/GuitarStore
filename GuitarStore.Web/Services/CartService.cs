using GuitarStore.Web.Data;
using GuitarStore.Web.Models;

namespace GuitarStore.Web.Services;

/// <summary>
/// Builds the priced view of a visitor's cart. Both the cart page and checkout need this,
/// so it lives in one place rather than being copied into each controller.
/// </summary>
public class CartService
{
    private readonly ICartRepository _carts;
    private readonly IProductRepository _products;

    public CartService(ICartRepository carts, IProductRepository products)
    {
        _carts = carts;
        _products = products;
    }

    /// <summary>
    /// Joins the stored cart lines to current catalog products. Lines whose product has
    /// since been deleted are dropped rather than throwing.
    /// </summary>
    public async Task<CartViewModel> BuildAsync(HttpContext context, CancellationToken ct = default)
    {
        var cartId = CartCookie.Read(context);
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
