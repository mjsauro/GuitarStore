using GuitarStore.Web.Models;

namespace GuitarStore.Web.Services;

/// <summary>
/// Transactional email. The MVC 5 app had this logic copy-pasted inline in three places
/// across two controllers, plus a fourth commented-out copy in a dead file; account email
/// now belongs to Cognito, so the order receipt is all that's left for the app to send.
/// </summary>
public interface IEmailSender
{
    Task SendOrderReceiptAsync(Order order, CancellationToken ct = default);
}

/// <summary>Used when no sender is configured — records what would have been sent.</summary>
public class NullEmailSender : IEmailSender
{
    private readonly ILogger<NullEmailSender> _logger;

    public NullEmailSender(ILogger<NullEmailSender> logger) => _logger = logger;

    public Task SendOrderReceiptAsync(Order order, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Email not configured; skipping receipt for order {TrackingNumber} to {Email}",
            order.TrackingNumber, order.Email);
        return Task.CompletedTask;
    }
}
