# Getting Started with Bikiran.Payment.Bkash

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

Install via NuGet Package Manager:

```bash
dotnet add package Bikiran.Payment.Bkash
```

Or via Package Manager Console:

```powershell
Install-Package Bikiran.Payment.Bkash
```

## Quick Start

### 1. Configure Services in Program.cs

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

// Add bKash payment services
builder.Services.AddBkashPayment(builder.Configuration);

// Optional: Add health checks
builder.Services.AddHealthChecks()
    .AddBkashHealthCheck("bkash", "payment", "external");

var app = builder.Build();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
```

### 2. Configure Settings in appsettings.json

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

### 3. Use in Your Code

```csharp
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;

public class PaymentController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;

    public PaymentController(IBkashPaymentService bkashService)
    {
        _bkashService = bkashService;
    }

    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment(decimal amount, string invoiceNumber)
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = invoiceNumber,
            Intent = "sale"
        };

        var response = await _bkashService.CreatePaymentAsync(request);
        return Ok(new { paymentUrl = response.BkashURL, paymentId = response.PaymentID });
    }

    [HttpPost("execute-payment/{paymentId}")]
    public async Task<IActionResult> ExecutePayment(string paymentId)
    {
        var response = await _bkashService.ExecutePaymentAsync(paymentId);
        return Ok(new { success = response.IsCompleted, transactionId = response.TrxID });
    }
}
```

## Sandbox Credentials

For testing, you can use the following sandbox credentials:

```json
{
  "Bkash": {
    "AppKey": "4f6o0cjiki2rfm34kfdadl1eqq",
    "AppSecret": "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b",
    "Username": "sandboxTokenizedUser02",
    "Password": "sandboxTokenizedUser02@12345",
    "Environment": "Sandbox"
  }
}
```

⚠️ **Note**: These are public sandbox credentials for testing only. Never use production credentials in your code or commit them to source control.

## Next Steps

- **Configuration**: See [02-configuration.md](02-configuration.md) for detailed configuration options
- **API Usage**: See [03-api-usage.md](03-api-usage.md) for comprehensive examples
- **Quick Reference**: See [04-quick-reference.md](04-quick-reference.md) for quick lookup

## Support

- **Documentation**: Complete documentation available in this docs-final folder
- **GitHub Issues**: https://github.com/bikirandev/Bikiran.Payment.Bkash/issues
- **License**: MIT
