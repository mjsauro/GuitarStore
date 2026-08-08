namespace GuitarStore.Web.Services;

/// <summary>What the store hands a payment processor to authorize a sale.</summary>
public record PaymentAuthorization(
    string CardNumber,
    string CardholderName,
    string Cvv,
    string ExpirationMonth,
    string ExpirationYear,
    decimal Amount,
    decimal Tax,
    string OrderReference,
    string Email);

/// <summary>
/// The outcome of an authorization attempt. <see cref="CardLastFour"/> is the only piece of
/// card data that ever leaves this boundary — nothing else is safe to keep.
/// </summary>
public record PaymentResult(bool Approved, string CardLastFour, string? DeclineReason = null);

public interface IPaymentService
{
    Task<PaymentResult> AuthorizeAsync(PaymentAuthorization authorization, CancellationToken ct = default);
}
