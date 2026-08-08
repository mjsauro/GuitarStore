using GuitarStore.Web.Models;

namespace GuitarStore.Web.Data;

/// <summary>
/// The original catalog, carried over from GuitarStoreDB/Script.Products.sql. Idempotent:
/// products are written by id, so re-running just overwrites with the same values.
/// </summary>
public static class SeedData
{
    private static readonly DateTime SeededAt = new(2017, 11, 14, 0, 0, 0, DateTimeKind.Utc);

    public static IReadOnlyList<Product> Products =>
    [
        new Product
        {
            Id = 1,
            ProductTypeName = "Guitar",
            MakeName = "Fender",
            ManufacturerName = "Fender",
            Model = "Stratocaster",
            Description = "A true classic from the 1950s.",
            Image = "/images/strat.jpg",
            Price = 599.00m,
            Properties = new() { ["Color"] = "White", ["PickupStyle"] = "Single Coil" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 38,
            ProductTypeName = "Guitar",
            MakeName = "Gibson",
            ManufacturerName = "Gibson",
            Model = "Les Paul",
            Description = "A perfect balance of clean and distorted tones.",
            Image = "/images/lespaul.jpg",
            Price = 699.00m,
            Properties = new() { ["Color"] = "Sunburst" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 39,
            ProductTypeName = "Guitar",
            MakeName = "Gibson",
            ManufacturerName = "Gibson",
            Model = "Explorer",
            Description = "Featuring two humbucking pickups with a serious crunch.",
            Image = "/images/explorer.jpg",
            Price = 499.00m,
            Properties = new() { ["Color"] = "Black" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 40,
            ProductTypeName = "Amplifier",
            MakeName = "Marshall",
            ManufacturerName = "Marshall",
            Model = "JCM2000",
            Description = "Trademark Marshall sounds.",
            Image = "/images/marshall.jpg",
            Price = 199.00m,
            Properties = new() { ["Size"] = "24 x 24", ["Watts"] = "100" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 41,
            ProductTypeName = "Amplifier",
            MakeName = "Fender",
            ManufacturerName = "Fender",
            Model = "Twinspeaker",
            Description = "Packs a double punch!",
            Image = "/images/fender.jpg",
            Price = 349.00m,
            Properties = new() { ["Size"] = "10 x 10", ["Watts"] = "50" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 42,
            ProductTypeName = "Amplifier",
            MakeName = "Vox",
            ManufacturerName = "Vox",
            Model = "Valvetronix",
            Description = "Tube emulation at an affordable price.",
            Image = "/images/vox.jpg",
            Price = 499.00m,
            Properties = new() { ["Size"] = "15 x 15", ["Watts"] = "75" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 44,
            ProductTypeName = "Effect",
            MakeName = "Dunlop",
            ManufacturerName = "Dunlop",
            Model = "CryBaby Wah",
            Description = "Add great wah effects to your guitar solos.",
            Image = "/images/wah.jpg",
            Price = 79.00m,
            Properties = new() { ["Effect Type"] = "Wah" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 45,
            ProductTypeName = "Effect",
            MakeName = "Ibanez",
            ManufacturerName = "Ibanez",
            Model = "Tube Screamer",
            Description = "Make your tone soar!",
            Image = "/images/tubescreamer.jpg",
            Price = 129.00m,
            Properties = new() { ["Effect Type"] = "Overdrive" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        },
        new Product
        {
            Id = 54,
            ProductTypeName = "Effect",
            MakeName = "Boss",
            ManufacturerName = "Boss",
            Model = "DS-1",
            Description = "Classic distortion sound.",
            Image = "/images/distortionpedal.jpg",
            Price = 39.00m,
            Properties = new() { ["Effect Type"] = "Distortion" },
            DateCreated = SeededAt,
            DateModified = SeededAt
        }
    ];

    public static async Task SeedProductsAsync(IProductRepository repository, CancellationToken ct = default)
    {
        var existing = await repository.GetAllAsync(ct);
        if (existing.Count > 0)
        {
            return;
        }

        foreach (var product in Products)
        {
            await repository.SaveAsync(product, ct);
        }
    }
}
