namespace GuitarStore.Web.Services;

/// <summary>
/// The cart id lives in a cookie, same as the original store. Checkout needs the same
/// lookup the cart pages use, so it lives here rather than in a single controller.
/// </summary>
public static class CartCookie
{
    public const string Name = "cartID";

    /// <summary>The current cart id, or null when the visitor has no cart yet.</summary>
    public static string? Read(HttpContext context) =>
        context.Request.Cookies.TryGetValue(Name, out var value) && Guid.TryParse(value, out var id)
            ? id.ToString()
            : null;

    /// <summary>The current cart id, creating and issuing one if the visitor doesn't have it.</summary>
    public static string ReadOrCreate(HttpContext context)
    {
        var existing = Read(context);
        if (existing is not null)
        {
            return existing;
        }

        var cartId = Guid.NewGuid().ToString();
        context.Response.Cookies.Append(Name, cartId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return cartId;
    }

    public static void Clear(HttpContext context) => context.Response.Cookies.Delete(Name);
}
