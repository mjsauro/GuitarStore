using Amazon.DynamoDBv2.DataModel;
using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public class CartRepository : ICartRepository
{
    private readonly IDynamoDBContext _context;

    public CartRepository(IDynamoDBContext context) => _context = context;

    public async Task<IReadOnlyList<CartItem>> GetItemsAsync(string cartId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(cartId))
        {
            return [];
        }

        var items = await _context.QueryAsync<CartItem>(cartId).GetRemainingAsync(ct);
        return items.OrderBy(i => i.ProductId).ToList();
    }

    public async Task AddOrIncrementAsync(string cartId, int productId, int quantity, CancellationToken ct = default)
    {
        var key = CartItem.KeyFor(productId);
        var existing = await _context.LoadAsync<CartItem?>(cartId, key, ct);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            await _context.SaveAsync(
                new CartItem
                {
                    CartId = cartId,
                    SortKey = key,
                    ProductId = productId,
                    Quantity = quantity,
                    DateCreated = now,
                    DateModified = now
                },
                ct);
            return;
        }

        existing.Quantity += quantity;
        existing.DateModified = now;
        await _context.SaveAsync(existing, ct);
    }

    public async Task SetQuantityAsync(string cartId, int productId, int quantity, CancellationToken ct = default)
    {
        var key = CartItem.KeyFor(productId);

        if (quantity <= 0)
        {
            await _context.DeleteAsync<CartItem>(cartId, key, ct);
            return;
        }

        var existing = await _context.LoadAsync<CartItem?>(cartId, key, ct);
        if (existing is null)
        {
            return;
        }

        existing.Quantity = quantity;
        existing.DateModified = DateTime.UtcNow;
        await _context.SaveAsync(existing, ct);
    }

    public async Task ClearAsync(string cartId, CancellationToken ct = default)
    {
        var items = await GetItemsAsync(cartId, ct);
        foreach (var item in items)
        {
            await _context.DeleteAsync<CartItem>(item.CartId, item.SortKey, ct);
        }
    }
}
