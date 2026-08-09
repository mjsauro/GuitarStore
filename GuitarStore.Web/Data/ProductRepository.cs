using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public class ProductRepository : IProductRepository
{
    private readonly IDynamoDBContext _context;

    public ProductRepository(IDynamoDBContext context) => _context = context;

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        // A scan is the right call here: the catalog is a handful of items and there is no
        // partition key that spans all of them.
        var products = await _context.ScanAsync<Product>([]).GetRemainingAsync(ct);
        return products.OrderBy(p => p.Id).ToList();
    }

    public async Task<IReadOnlyList<Product>> GetByTypeAsync(string productTypeName, CancellationToken ct = default)
    {
        var query = _context.QueryAsync<Product>(
            productTypeName,
            new QueryConfig { IndexName = "ProductTypeName-index" });

        var products = await query.GetRemainingAsync(ct);
        return products.OrderBy(p => p.Id).ToList();
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _context.LoadAsync<Product?>(id, ct);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var distinctIds = ids.Distinct().ToList();
        if (distinctIds.Count == 0)
        {
            return [];
        }

        var batch = _context.CreateBatchGet<Product>();
        foreach (var id in distinctIds)
        {
            batch.AddKey(id);
        }

        await batch.ExecuteAsync(ct);
        return batch.Results;
    }

    public Task SaveAsync(Product product, CancellationToken ct = default) =>
        _context.SaveAsync(product, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default) =>
        _context.DeleteAsync<Product>(id, ct);
}
