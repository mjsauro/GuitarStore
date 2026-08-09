namespace GuitarStore.Web.Services;

/// <summary>
/// A stand-in payment processor so the checkout flow is fully demoable without a merchant
/// account. It moves no money and talks to nothing. Card numbers are validated for shape
/// only, and nothing but the last four digits is ever returned, logged, or stored.
///
/// When a real processor goes in, it should tokenize the card in the browser (Stripe
/// Elements, Braintree Drop-in, etc.) so raw card data stops reaching the server at all —
/// at which point <see cref="PaymentAuthorization"/> carries a token instead of a PAN.
/// </summary>
public class SimulatedPaymentService : IPaymentService
{
    /// <summary>Any card ending in these digits declines, so the failure path stays demoable.</summary>
    private static readonly string[] DecliningSuffixes = ["0002", "0341"];

    private readonly ILogger<SimulatedPaymentService> _logger;

    public SimulatedPaymentService(ILogger<SimulatedPaymentService> logger) => _logger = logger;

    public Task<PaymentResult> AuthorizeAsync(PaymentAuthorization authorization, CancellationToken ct = default)
    {
        var digits = new string(authorization.CardNumber.Where(char.IsDigit).ToArray());

        if (digits.Length is < 13 or > 19)
        {
            return Task.FromResult(new PaymentResult(false, "", "That card number doesn't look right. Check the digits and try again."));
        }

        if (!PassesLuhn(digits))
        {
            return Task.FromResult(new PaymentResult(false, "", "That card number didn't validate. Check the digits and try again."));
        }

        if (!IsFutureExpiry(authorization.ExpirationMonth, authorization.ExpirationYear))
        {
            return Task.FromResult(new PaymentResult(false, "", "That card has expired. Try a different card."));
        }

        var lastFour = digits[^4..];

        if (DecliningSuffixes.Contains(lastFour))
        {
            _logger.LogInformation("Simulated decline for order {OrderReference}", authorization.OrderReference);
            return Task.FromResult(new PaymentResult(false, lastFour, "Your card was declined. Try a different card."));
        }

        _logger.LogInformation(
            "Simulated authorization of {Amount:C} for order {OrderReference} (card ending {LastFour})",
            authorization.Amount, authorization.OrderReference, lastFour);

        return Task.FromResult(new PaymentResult(true, lastFour));
    }

    private static bool PassesLuhn(string digits)
    {
        var sum = 0;
        var isSecond = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var value = digits[i] - '0';
            if (isSecond)
            {
                value *= 2;
                if (value > 9)
                {
                    value -= 9;
                }
            }

            sum += value;
            isSecond = !isSecond;
        }

        return sum % 10 == 0;
    }

    private static bool IsFutureExpiry(string month, string year)
    {
        if (!int.TryParse(month, out var m) || !int.TryParse(year, out var y) || m is < 1 or > 12)
        {
            return false;
        }

        // A card is good through the last day of its expiry month.
        var expiresAfter = new DateTime(y, m, 1).AddMonths(1);
        return expiresAfter > DateTime.UtcNow;
    }
}
