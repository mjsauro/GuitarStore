using System.Net;
using System.Text;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using GuitarStore.Web.Models;

namespace GuitarStore.Web.Services;

public class SesEmailSender : IEmailSender
{
    private readonly IAmazonSimpleEmailServiceV2 _ses;
    private readonly ILogger<SesEmailSender> _logger;
    private readonly string _fromAddress;

    public SesEmailSender(IAmazonSimpleEmailServiceV2 ses, IConfiguration configuration, ILogger<SesEmailSender> logger)
    {
        _ses = ses;
        _logger = logger;
        _fromAddress = configuration["Email:FromAddress"] ?? "";
    }

    public async Task SendOrderReceiptAsync(Order order, CancellationToken ct = default)
    {
        var request = new SendEmailRequest
        {
            FromEmailAddress = _fromAddress,
            Destination = new Destination { ToAddresses = [order.Email] },
            Content = new EmailContent
            {
                Simple = new Message
                {
                    Subject = new Content { Data = $"Your order from Matt's Guitar Store ({order.TrackingNumber})" },
                    Body = new Body
                    {
                        Html = new Content { Data = BuildHtml(order) },
                        Text = new Content { Data = BuildText(order) }
                    }
                }
            }
        };

        try
        {
            await _ses.SendEmailAsync(request, ct);
            _logger.LogInformation("Sent receipt for order {TrackingNumber}", order.TrackingNumber);
        }
        catch (AccountSuspendedException ex)
        {
            LogFailure(order, ex);
        }
        catch (MessageRejectedException ex)
        {
            // In the SES sandbox this is what an unverified recipient looks like.
            LogFailure(order, ex);
        }
        catch (AmazonSimpleEmailServiceV2Exception ex)
        {
            LogFailure(order, ex);
        }
    }

    /// <summary>
    /// The order is already paid for and stored by the time this runs, so a mail failure
    /// must never surface as a failed checkout. It's logged and swallowed.
    /// </summary>
    private void LogFailure(Order order, Exception ex) =>
        _logger.LogWarning(
            ex,
            "Could not send the receipt for order {TrackingNumber} to {Email}. The order itself is unaffected.",
            order.TrackingNumber, order.Email);

    private static string BuildHtml(Order order)
    {
        var rows = new StringBuilder();
        foreach (var line in order.LineItems)
        {
            rows.Append($"""
                <tr>
                  <td style="padding:6px 12px;border-bottom:1px solid #eee;">{Encode(line.ProductName)}</td>
                  <td style="padding:6px 12px;border-bottom:1px solid #eee;text-align:center;">{line.Quantity}</td>
                  <td style="padding:6px 12px;border-bottom:1px solid #eee;text-align:right;">{line.UnitPrice:C}</td>
                  <td style="padding:6px 12px;border-bottom:1px solid #eee;text-align:right;">{line.LineTotal:C}</td>
                </tr>
                """);
        }

        return $"""
            <html>
              <body style="font-family:Arial,Helvetica,sans-serif;color:#2b2420;">
                <h1 style="font-size:22px;">Thank you for your order, {Encode(order.PurchaserName)}!</h1>
                <p>Your tracking number is <strong>{Encode(order.TrackingNumber)}</strong>.</p>
                <table style="border-collapse:collapse;width:100%;max-width:600px;">
                  <thead>
                    <tr style="background:#f4f1ea;">
                      <th style="padding:8px 12px;text-align:left;">Item</th>
                      <th style="padding:8px 12px;">Qty</th>
                      <th style="padding:8px 12px;text-align:right;">Price</th>
                      <th style="padding:8px 12px;text-align:right;">Total</th>
                    </tr>
                  </thead>
                  <tbody>{rows}</tbody>
                  <tfoot>
                    <tr><td colspan="3" style="padding:6px 12px;text-align:right;">Subtotal</td><td style="padding:6px 12px;text-align:right;">{order.SubTotal:C}</td></tr>
                    <tr><td colspan="3" style="padding:6px 12px;text-align:right;">Shipping and Handling</td><td style="padding:6px 12px;text-align:right;">{order.ShippingAndHandling:C}</td></tr>
                    <tr><td colspan="3" style="padding:6px 12px;text-align:right;">Tax</td><td style="padding:6px 12px;text-align:right;">{order.Tax:C}</td></tr>
                    <tr><td colspan="3" style="padding:6px 12px;text-align:right;"><strong>Total</strong></td><td style="padding:6px 12px;text-align:right;"><strong>{order.Total:C}</strong></td></tr>
                  </tfoot>
                </table>
                <p>Shipping to: {Encode(order.ShippingAddress)}, {Encode(order.ShippingCity)}, {Encode(order.ShippingState)} {Encode(order.ShippingPostalCode)}</p>
                <p>Paid with the card ending {Encode(order.CardLastFour)}.</p>
                <p style="color:#8a7d68;font-size:12px;">This is a portfolio demo store. No payment was actually processed and nothing will ship.</p>
              </body>
            </html>
            """;
    }

    private static string BuildText(Order order)
    {
        var text = new StringBuilder();
        text.AppendLine($"Thank you for your order, {order.PurchaserName}!");
        text.AppendLine($"Tracking number: {order.TrackingNumber}");
        text.AppendLine();

        foreach (var line in order.LineItems)
        {
            text.AppendLine($"{line.Quantity} x {line.ProductName} @ {line.UnitPrice:C} = {line.LineTotal:C}");
        }

        text.AppendLine();
        text.AppendLine($"Subtotal: {order.SubTotal:C}");
        text.AppendLine($"Shipping and Handling: {order.ShippingAndHandling:C}");
        text.AppendLine($"Tax: {order.Tax:C}");
        text.AppendLine($"Total: {order.Total:C}");
        text.AppendLine();
        text.AppendLine($"Shipping to: {order.ShippingAddress}, {order.ShippingCity}, {order.ShippingState} {order.ShippingPostalCode}");
        text.AppendLine($"Paid with the card ending {order.CardLastFour}.");
        text.AppendLine();
        text.AppendLine("This is a portfolio demo store. No payment was actually processed and nothing will ship.");

        return text.ToString();
    }

    /// <summary>Order details come from a form, so they're encoded before going into HTML.</summary>
    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
