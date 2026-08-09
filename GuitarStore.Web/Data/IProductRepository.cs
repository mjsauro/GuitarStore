using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public interface IProductRepository
{
    /// <summary>Every product in the catalog.</summary>
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Products of a single type (Guitar, Amplifier, Effect), via the ProductTypeName GSI.</summary>
    Task<IReadOnlyList<Product>> GetByTypeAsync(string productTypeName, CancellationToken ct = default);

    /// <summary>A single product, or null when the id doesn't exist.</summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Several products in one round trip — used to price up a cart.</summary>
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);

    Task SaveAsync(Product product, CancellationToken ct = default);

    Task DeleteAsync(int id, CancellationToken ct = default);
}
