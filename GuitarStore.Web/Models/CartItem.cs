using Amazon.DynamoDBv2.DataModel;

namespace GuitarStore.Web.Models;

/// <summary>
/// One line of a shopping cart. All lines for a cart share the CartId partition, so a single
/// Query returns the whole cart — the equivalent of the old Cart/CartProducts join.
/// The cart exists exactly as long as it has lines; there's no separate cart record.
/// </summary>
[DynamoDBTable("GuitarStore-Carts")]
public class CartItem
{
    public const string ItemKeyPrefix = "ITEM#";

    [DynamoDBHashKey]
    public string CartId { get; set; } = "";

    /// <summary>"ITEM#{ProductId}" — see <see cref="KeyFor"/>.</summary>
    [DynamoDBRangeKey]
    public string SortKey { get; set; } = "";

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

    public static string KeyFor(int productId) => $"{ItemKeyPrefix}{productId}";
}
