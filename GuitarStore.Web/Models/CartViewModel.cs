namespace GuitarStore.Web.Models;

/// <summary>A cart line joined to its product, priced at current catalog prices.</summary>
public class CartLine
{
    public required Product Product { get; init; }

    public int Quantity { get; init; }

    public decimal LineTotal => Product.Price * Quantity;
}

/// <summary>
/// The cart as shown to the customer. Totals use the same rules as the original store:
/// 10.25% tax and $1 per item shipping.
/// </summary>
public class CartViewModel
{
    public const decimal TaxRate = 0.1025m;

    public string CartId { get; init; } = "";

    public IReadOnlyList<CartLine> Lines { get; init; } = [];

    public bool IsEmpty => Lines.Count == 0;

    public decimal SubTotal => Lines.Sum(l => l.LineTotal);

    public decimal ShippingAndHandling => Lines.Sum(l => l.Quantity);

    public decimal Tax => decimal.Round(SubTotal * TaxRate, 2, MidpointRounding.AwayFromZero);

    public decimal Total => SubTotal + Tax + ShippingAndHandling;
}
