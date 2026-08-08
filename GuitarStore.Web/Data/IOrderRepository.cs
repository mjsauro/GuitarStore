using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

public interface IOrderRepository
{
    /// <summary>A single order by tracking number, or null when there's no such order.</summary>
    Task<Order?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);

    Task SaveAsync(Order order, CancellationToken ct = default);

    /// <summary>Every order — used by the sales report to aggregate units sold.</summary>
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
}
