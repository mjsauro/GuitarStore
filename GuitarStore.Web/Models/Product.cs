using Amazon.DynamoDBv2.DataModel;

namespace GuitarStore.Web.Models;

/// <summary>
/// A catalog item. Make/Manufacturer/ProductType names are denormalized onto the item
/// (they were separate joined tables in the SQL Server version), and the old
/// ProductTypeProperty/ProductTypePropertyValue EAV tables collapse into <see cref="Properties"/>.
/// </summary>
[DynamoDBTable("GuitarStore-Products")]
public class Product
{
    [DynamoDBHashKey]
    public int Id { get; set; }

    /// <summary>Guitar, Amplifier, or Effect. Partition key of the ProductTypeName-index GSI.</summary>
    [DynamoDBGlobalSecondaryIndexHashKey("ProductTypeName-index")]
    public string ProductTypeName { get; set; } = "";

    public string MakeName { get; set; } = "";

    public string ManufacturerName { get; set; } = "";

    /// <summary>Model name — "Mod" in the legacy schema.</summary>
    public string Model { get; set; } = "";

    public string Description { get; set; } = "";

    public string Image { get; set; } = "";

    public decimal Price { get; set; }

    /// <summary>Type-specific attributes, e.g. Color/PickupStyle for guitars, Watts/Size for amps.</summary>
    public Dictionary<string, string> Properties { get; set; } = new();

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    /// <summary>Display name used across the catalog and cart, e.g. "Fender Stratocaster".</summary>
    [DynamoDBIgnore]
    public string DisplayName => $"{MakeName} {Model}".Trim();
}
