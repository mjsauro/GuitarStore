using Amazon.DynamoDBv2.DataModel;
using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public class OrderRepository : IOrderRepository
{
    private readonly IDynamoDBContext _context;

    public OrderRepository(IDynamoDBContext context) => _context = context;

    public Task<Order?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default) =>
        _context.LoadAsync<Order?>(trackingNumber, ct);

    public Task SaveAsync(Order order, CancellationToken ct = default) =>
        _context.SaveAsync(order, ct);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _context.ScanAsync<Order>([]).GetRemainingAsync(ct);
        return orders.OrderByDescending(o => o.DateCreated).ToList();
    }
}
