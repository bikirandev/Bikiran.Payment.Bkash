# Webhook Handling API Reference

Secure webhook signature verification and notification processing.

## Overview

bKash sends webhooks to notify you of payment events. Bikiran.Payment.Bkash provides utilities to verify webhook authenticity.

## BkashWebhookHelper

Static helper class for webhook operations.

### VerifyWebhookSignature

Verify the authenticity of a webhook using HMAC-SHA256.

```csharp
public static bool VerifyWebhookSignature(
    string payload,
    string signature,
    string appSecret)
```

#### Parameters

- `payload` (string): Raw webhook request body
- `signature` (string): Value from `X-Signature` header
- `appSecret` (string): Your bKash app secret

#### Returns

- `true` if signature is valid
- `false` if signature is invalid

#### Example Usage

```csharp
using Bikiran.Payment.Bkash.Utilities;

[HttpPost("webhook")]
public async Task<IActionResult> ReceiveWebhook()
{
    // Read raw body
    using var reader = new StreamReader(Request.Body);
    var payload = await reader.ReadToEndAsync();
    
    // Get signature from header
    var signature = Request.Headers["X-Signature"].ToString();
    
    if (string.IsNullOrEmpty(signature))
    {
        return BadRequest("Missing signature");
    }
    
    // Verify signature
    var appSecret = _configuration["Bkash:AppSecret"];
    if (!BkashWebhookHelper.VerifyWebhookSignature(payload, signature, appSecret))
    {
        return Unauthorized("Invalid signature");
    }
    
    // Process webhook
    var notification = JsonConvert.DeserializeObject<BkashWebhookNotification>(payload);
    await ProcessWebhook(notification);
    
    return Ok();
}
```

### IsTimestampValid

Validate webhook timestamp to prevent replay attacks.

```csharp
public static bool IsTimestampValid(
    string timestamp,
    int toleranceSeconds = 300)
```

#### Parameters

- `timestamp` (string): ISO 8601 timestamp from webhook
- `toleranceSeconds` (int): Maximum age in seconds (default: 300 = 5 minutes)

#### Returns

- `true` if timestamp is within tolerance
- `false` if timestamp is too old

#### Example Usage

```csharp
var notification = JsonConvert.DeserializeObject<BkashWebhookNotification>(payload);

if (!BkashWebhookHelper.IsTimestampValid(notification.Timestamp))
{
    return BadRequest("Timestamp too old - possible replay attack");
}
```

### ComputeSignature

Manually compute HMAC-SHA256 signature.

```csharp
public static string ComputeSignature(
    string payload,
    string appSecret)
```

## BkashWebhookNotification Model

```csharp
public class BkashWebhookNotification
{
    public string EventType { get; set; }              // Event type
    public string PaymentID { get; set; }              // Payment identifier
    public string TrxID { get; set; }                  // Transaction ID
    public string TransactionStatus { get; set; }      // "Completed", "Cancelled", etc.
    public decimal Amount { get; set; }                // Transaction amount
    public string Currency { get; set; }               // Currency code
    public string MerchantInvoiceNumber { get; set; }  // Your invoice number
    public string CustomerMsisdn { get; set; }         // Customer phone
    public string Timestamp { get; set; }              // ISO 8601 timestamp
}
```

## Complete Webhook Example

```csharp
[ApiController]
[Route("api/webhooks")]
public class BkashWebhookController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BkashWebhookController> _logger;

    public BkashWebhookController(
        IConfiguration configuration,
        ILogger<BkashWebhookController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("bkash")]
    public async Task<IActionResult> ReceiveWebhook()
    {
        try
        {
            // 1. Read raw body
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            // 2. Get and verify signature
            var signature = Request.Headers["X-Signature"].ToString();
            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Webhook received without signature");
                return BadRequest("Missing signature");
            }

            var appSecret = _configuration["Bkash:AppSecret"];
            if (!BkashWebhookHelper.VerifyWebhookSignature(payload, signature, appSecret))
            {
                _logger.LogWarning("Invalid webhook signature");
                return Unauthorized("Invalid signature");
            }

            // 3. Parse webhook
            var notification = JsonConvert.DeserializeObject<BkashWebhookNotification>(payload);
            if (notification == null)
            {
                return BadRequest("Invalid payload");
            }

            // 4. Verify timestamp
            if (!BkashWebhookHelper.IsTimestampValid(notification.Timestamp))
            {
                _logger.LogWarning("Webhook timestamp too old");
                return BadRequest("Invalid timestamp");
            }

            // 5. Process webhook
            await ProcessWebhookNotification(notification);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process webhook");
            return StatusCode(500, "Internal error");
        }
    }

    private async Task ProcessWebhookNotification(BkashWebhookNotification notification)
    {
        _logger.LogInformation(
            "Processing webhook: PaymentId={PaymentId}, Status={Status}",
            notification.PaymentID,
            notification.TransactionStatus);

        switch (notification.TransactionStatus.ToLower())
        {
            case "completed":
                await HandlePaymentCompleted(notification);
                break;
            case "cancelled":
                await HandlePaymentCancelled(notification);
                break;
            case "failed":
                await HandlePaymentFailed(notification);
                break;
        }
    }

    private async Task HandlePaymentCompleted(BkashWebhookNotification notification)
    {
        // Update order status to paid
        // Send confirmation email
        // etc.
        await Task.CompletedTask;
    }

    private async Task HandlePaymentCancelled(BkashWebhookNotification notification)
    {
        // Update order status to cancelled
        await Task.CompletedTask;
    }

    private async Task HandlePaymentFailed(BkashWebhookNotification notification)
    {
        // Update order status to failed
        await Task.CompletedTask;
    }
}
```

## Security Best Practices

### ✅ DO

1. **Always verify signatures** before processing
2. **Check timestamps** to prevent replay attacks
3. **Use HTTPS** for webhook endpoints
4. **Log webhook events** for auditing
5. **Return 200 OK** quickly (process asynchronously if needed)
6. **Implement idempotency** to handle duplicate webhooks

### ❌ DON'T

1. **Don't skip signature verification**
2. **Don't process webhooks without timestamp validation**
3. **Don't expose webhook URLs without authentication**
4. **Don't log sensitive data** from webhooks
5. **Don't assume webhook order** (out-of-order delivery possible)

## Webhook Events

| Event | TransactionStatus | Description |
|-------|-------------------|-------------|
| Payment Completed | `Completed` | Payment successfully processed |
| Payment Cancelled | `Cancelled` | Payment cancelled by customer |
| Payment Failed | `Failed` | Payment processing failed |

## Testing Webhooks Locally

Use tools like ngrok to expose local development server:

```bash
# Start ngrok
ngrok http 5000

# Update bKash merchant portal with ngrok URL
# https://abc123.ngrok.io/api/webhooks/bkash
```

## Next Steps

- 📖 [Payment Operations](payment-operations.md)
- 📖 [Refund Operations](refund-operations.md)
- 📘 [Webhook Integration Guide](../guides/webhook-integration.md)
- 🔒 [Security Best Practices](../guides/security-best-practices.md)
