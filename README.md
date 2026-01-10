# Bikiran.Payment.Bkash

A comprehensive .NET library for integrating bKash payment gateway with automatic token management, payment processing, and refund capabilities.

## Features

- ✅ Automatic token management with refresh (55-minute ID tokens, 28-day refresh tokens)
- ✅ Payment creation and execution
- ✅ Payment status queries
- ✅ Refund processing with status tracking
- ✅ Multi-environment support (sandbox/production)
- ✅ Built-in error handling and logging
- ✅ Strongly-typed request/response models
- ✅ Dependency injection support
- ✅ Health check integration
- ✅ Webhook signature verification

## Installation

```bash
dotnet add package Bikiran.Payment.Bkash
```

## Quick Start

### 1. Configure Services

```csharp
services.AddBkashPayment(options =>
{
    options.AppKey = Configuration["Bkash:AppKey"];
    options.AppSecret = Configuration["Bkash:AppSecret"];
    options.Username = Configuration["Bkash:Username"];
    options.Password = Configuration["Bkash:Password"];
    options.Environment = BkashEnvironment.Sandbox; // or Production
});
```

### 2. Add Health Checks (Optional)

```csharp
services.AddHealthChecks()
    .AddBkashHealthCheck("bkash", "payment", "external");
```

### 3. Use in Your Code

```csharp
public class PaymentService
{
    private readonly IBkashPaymentService _bkashService;

    public PaymentService(IBkashPaymentService bkashService)
    {
        _bkashService = bkashService;
    }

    public async Task<string> CreatePaymentAsync(decimal amount, string invoiceNumber)
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = invoiceNumber,
            Intent = "sale"
        };

        var response = await _bkashService.CreatePaymentAsync(request);
        return response.BkashURL;
    }

    public async Task<bool> ExecutePaymentAsync(string paymentId)
    {
        var response = await _bkashService.ExecutePaymentAsync(paymentId);
        return response.TransactionStatus == "Completed";
    }
}
```

## Configuration

### appsettings.json

```json
{
  "Bkash": {
    "AppKey": "your-app-key",
    "AppSecret": "your-app-secret",
    "Username": "your-username",
    "Password": "your-password",
    "Environment": "Sandbox",
    "TimeoutSeconds": 30,
    "TokenRefreshBufferSeconds": 300
  }
}
```

### Environment Variables

You can also configure using environment variables:

```env
BKASH__APPKEY=your-app-key
BKASH__APPSECRET=your-app-secret
BKASH__USERNAME=your-username
BKASH__PASSWORD=your-password
BKASH__ENVIRONMENT=Sandbox
```

📚 **For detailed configuration guides:**

- [Configuration Guide](docs-final/configuration/configuration-guide.md) - All configuration methods and options
- [Environment Setup](docs-final/configuration/environment-setup.md) - Platform-specific deployment setup

⚠️ **Security Note**: Never commit credentials to source control!

## API Reference

### IBkashPaymentService

- `CreatePaymentAsync(BkashCreatePaymentRequest)` - Create a new payment
- `ExecutePaymentAsync(string paymentId)` - Execute a payment
- `QueryPaymentAsync(string paymentId)` - Query payment status
- `RefundPaymentAsync(BkashRefundPaymentRequest)` - Process a refund
- `QueryRefundStatusAsync(string paymentId, string trxId)` - Query refund status

### IBkashTokenService

- `GetValidTokenAsync()` - Get a valid access token (auto-refreshes)
- `GrantTokenAsync()` - Request a new token
- `RefreshTokenAsync(string refreshToken)` - Refresh an existing token

📚 **For complete API documentation, see:**

- [Payment Operations](docs-final/api-reference/payment-operations.md)
- [Refund Operations](docs-final/api-reference/refund-operations.md)
- [Token Management](docs-final/api-reference/token-management.md)
- [Webhook Handling](docs-final/api-reference/webhook-handling.md)
- [Health Checks](docs-final/api-reference/health-checks.md)

## Webhook Verification

Verify webhook signatures to ensure authenticity:

```csharp
using Bikiran.Payment.Bkash.Utilities;

public IActionResult ReceiveWebhook([FromBody] string payload, [FromHeader(Name = "X-Signature")] string signature)
{
    var appSecret = Configuration["Bkash:AppSecret"];

    if (!BkashWebhookHelper.VerifyWebhookSignature(payload, signature, appSecret))
    {
        return Unauthorized("Invalid signature");
    }

    var notification = JsonConvert.DeserializeObject<BkashWebhookNotification>(payload);
    // Process webhook...

    return Ok();
}
```

## Payment Modes

- `0001` - Checkout (one-time payment)
- `0011` - Agreement (subscription/auto-debit)
- `0002` - Pre-Authorization
- `0021` - Disbursement
- `0031` - Refund

## Error Handling

```csharp
try
{
    var response = await _bkashService.CreatePaymentAsync(request);
}
catch (BkashAuthenticationException ex)
{
    // Handle authentication errors
    Console.WriteLine($"Auth Error: {ex.Message}");
}
catch (BkashPaymentException ex)
{
    // Handle payment-specific errors
    Console.WriteLine($"Payment Error: {ex.ErrorCode} - {ex.Message}");
}
catch (BkashException ex)
{
    // Handle general bKash errors
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
}
```

📚 **For comprehensive error handling guide:**

- [Error Handling Guide](docs-final/guides/error-handling.md)

## Advanced Configuration

### Custom Timeout and Token Refresh

```csharp
services.AddBkashPayment(options =>
{
    options.AppKey = "your-app-key";
    options.AppSecret = "your-app-secret";
    options.Username = "your-username";
    options.Password = "your-password";
    options.Environment = BkashEnvironment.Production;
    options.TimeoutSeconds = 60; // Custom timeout
    options.TokenRefreshBufferSeconds = 600; // Refresh 10 minutes before expiry
});
```

### Custom Base URL

```csharp
services.AddBkashPayment(options =>
{
    options.AppKey = "your-app-key";
    options.AppSecret = "your-app-secret";
    options.Username = "your-username";
    options.Password = "your-password";
    options.BaseUrl = "https://custom-bkash-api.example.com";
});
```

## Documentation

📚 **Complete documentation is available in the [docs-final](docs-final/) folder:**

- **[Quick Start Guide](docs-final/getting-started/quick-start.md)** - Get started in 5 minutes
- **[Code Examples](docs-final/getting-started/examples.md)** - Comprehensive examples
- **[Configuration Guide](docs-final/configuration/configuration-guide.md)** - All configuration options
- **[API Reference](docs-final/api-reference/)** - Complete API documentation
- **[Guides](docs-final/guides/)** - Payment flow, error handling, security
- **[Development](docs-final/development/)** - Contributing and project structure

## License

MIT

## Support

For issues and feature requests, please visit [GitHub Issues](https://github.com/bikiran/bkash-dotnet/issues).
