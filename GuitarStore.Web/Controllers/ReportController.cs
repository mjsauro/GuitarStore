using GuitarStore.Web.Data;
using GuitarStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuitarStore.Web.Controllers;

/// <summary>
/// Sales reporting. This replaces the MVC 5 ReportHome page, which queried the unrelated
/// AdventureWorks sample database by concatenating a query-string value straight into SQL.
/// This one aggregates the store's own orders, and takes no user input to query on at all.
/// </summary>
[Authorize(Roles = Roles.Admin)]
public class ReportController : Controller
{
    private readonly IOrderRepository _orders;

    public ReportController(IOrderRepository orders) => _orders = orders;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var orders = await _orders.GetAllAsync(ct);

        var topSellers = orders
            .SelectMany(order => order.LineItems)
            .GroupBy(line => new { line.ProductId, line.ProductName })
            .Select(group => new TopSeller
            {
                ProductId = group.Key.ProductId,
                ProductName = group.Key.ProductName,
                UnitsSold = group.Sum(line => line.Quantity),
                Revenue = group.Sum(line => line.LineTotal)
            })
            .OrderByDescending(seller => seller.UnitsSold)
            .ThenByDescending(seller => seller.Revenue)
            .Take(5)
            .ToList();

        ViewData["Title"] = "Sales Report";
        return View(new SalesReportViewModel
        {
            OrderCount = orders.Count,
            GrossRevenue = orders.Sum(order => order.Total),
            TopSellers = topSellers
        });
    }
}
