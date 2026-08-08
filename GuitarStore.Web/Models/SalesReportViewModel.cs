namespace GuitarStore.Web.Models;

public class TopSeller
{
    public int ProductId { get; init; }

    public string ProductName { get; init; } = "";

    public int UnitsSold { get; init; }

    public decimal Revenue { get; init; }
}

public class SalesReportViewModel
{
    public int OrderCount { get; init; }

    public decimal GrossRevenue { get; init; }

    public IReadOnlyList<TopSeller> TopSellers { get; init; } = [];
}
