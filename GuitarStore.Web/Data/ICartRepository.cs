using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public interface ICartRepository
{
    /// <summary>Every line in a cart, in one Query.</summary>
    Task<IReadOnlyList<CartItem>> GetItemsAsync(string cartId, CancellationToken ct = default);

    /// <summary>Adds a product to the cart, or bumps its quantity if it's already there.</summary>
    Task AddOrIncrementAsync(string cartId, int productId, int quantity, CancellationToken ct = default);

    /// <summary>Sets an exact quantity. A quantity of zero or less removes the line.</summary>
    Task SetQuantityAsync(string cartId, int productId, int quantity, CancellationToken ct = default);

    /// <summary>Empties the cart.</summary>
    Task ClearAsync(string cartId, CancellationToken ct = default);
}
