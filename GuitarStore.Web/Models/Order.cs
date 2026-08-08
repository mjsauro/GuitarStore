using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace GuitarStore.Web.Models;

/// <summary>
/// A line on a placed order. Name and price are snapshots taken at purchase time — the
/// legacy schema did the same thing with OrderProduct.PlacedName/PlacedUnitName — so
/// later catalog edits never rewrite someone's receipt.
/// </summary>
public class OrderLineItem
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = "";

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}

/// <summary>
/// A placed order, keyed by its tracking number — which is what the receipt page looks up,
/// so no secondary index is needed.
/// </summary>
[DynamoDBTable("GuitarStore-Orders")]
public class Order
{
    [DynamoDBHashKey]
    public string TrackingNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public string PurchaserName { get; set; } = "";

    public string ShippingAddress { get; set; } = "";

    public string ShippingCity { get; set; } = "";

    public string ShippingState { get; set; } = "";

    public string ShippingPostalCode { get; set; } = "";

    public decimal SubTotal { get; set; }

    public decimal ShippingAndHandling { get; set; }

    public decimal Tax { get; set; }

    public decimal Total => SubTotal + ShippingAndHandling + Tax;

    /// <summary>Last four digits only. The full number is never stored.</summary>
    public string CardLastFour { get; set; } = "";

    [DynamoDBProperty(typeof(OrderLineItemsConverter))]
    public List<OrderLineItem> LineItems { get; set; } = [];

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public DateTime? ShipDate { get; set; }
}

/// <summary>Stores the embedded line items as a JSON document attribute.</summary>
public class OrderLineItemsConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        var items = value as List<OrderLineItem> ?? [];
        return new Primitive(JsonSerializer.Serialize(items));
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        var json = entry?.AsString();
        return string.IsNullOrEmpty(json)
            ? new List<OrderLineItem>()
            : JsonSerializer.Deserialize<List<OrderLineItem>>(json) ?? [];
    }
}
