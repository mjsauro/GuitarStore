using GuitarStore.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

public class ReceiptController : Controller
{
    private readonly IOrderRepository _orders;

    public ReceiptController(IOrderRepository orders) => _orders = orders;

    /// <summary>
    /// Looks up a receipt by tracking number. That's the table's partition key, so this is a
    /// direct read — the legacy version scanned with .First() and threw on a bad number.
    /// </summary>
    public async Task<IActionResult> Index(string? id, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var order = await _orders.GetByTrackingNumberAsync(id, ct);
        if (order is null)
        {
            return NotFound();
        }

        ViewData["Title"] = $"Receipt {order.TrackingNumber}";
        return View(order);
    }
}
