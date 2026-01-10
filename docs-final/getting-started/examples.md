# Code Examples

Comprehensive, practical code examples to help you integrate bKash payment processing.

## Table of Contents

- [Setup](#setup)
- [Creating a Payment](#creating-a-payment)
- [Executing a Payment](#executing-a-payment)
- [Querying Payment Status](#querying-payment-status)
- [Processing a Refund](#processing-a-refund)
- [Checking Refund Status](#checking-refund-status)
- [Minimal API Example](#minimal-api-example)
- [Console Application Example](#console-application-example)

## Setup

### Service Configuration

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

// Configure bKash services
builder.Services.AddBkashPayment(builder.Configuration);

// Optional: Add health checks
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();
app.MapHealthChecks("/health");
app.Run();
```

### Configuration File (appsettings.json)

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

## Creating a Payment

```csharp
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;

    public PaymentController(IBkashPaymentService bkashService)
    {
        _bkashService = bkashService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] double amount)
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
            PayerReference = "01712345678",  // Customer phone number
            Intent = "sale",
            Currency = "BDT"
        };

        var response = await _bkashService.CreatePaymentAsync(request);

        return Ok(new
        {
            success = true,
            paymentId = response.PaymentID,
            bkashUrl = response.BkashURL,
            message = "Redirect user to bkashUrl to complete payment"
        });
    }
}
```

## Executing a Payment

After the customer completes payment on bKash and returns to your callback URL:

```csharp
[HttpPost("execute/{paymentId}")]
public async Task<IActionResult> ExecutePayment(string paymentId)
{
    var response = await _bkashService.ExecutePaymentAsync(paymentId);

    if (response.IsCompleted)
    {
        // Payment successful - update your database
        return Ok(new
        {
            success = true,
            transactionId = response.TrxID,
            amount = response.Amount,
            customerMsisdn = response.CustomerMsisdn,
            message = "Payment completed successfully"
        });
    }

    return BadRequest(new
    {
        success = false,
        message = "Payment execution failed"
    });
}
```

## Querying Payment Status

Check the current status of a payment:

```csharp
[HttpGet("status/{paymentId}")]
public async Task<IActionResult> GetPaymentStatus(string paymentId)
{
    var response = await _bkashService.QueryPaymentAsync(paymentId);

    return Ok(new
    {
        paymentId = response.PaymentID,
        transactionId = response.TrxID,
        status = response.TransactionStatus,
        amount = response.Amount,
        createTime = response.CreateTime,
        updateTime = response.UpdateTime
    });
}
```

## Processing a Refund

Issue a full or partial refund:

```csharp
[HttpPost("refund")]
public async Task<IActionResult> ProcessRefund(
    [FromBody] RefundRequest refundRequest)
{
    var request = new BkashRefundPaymentRequest
    {
        PaymentId = refundRequest.PaymentId,
        TrxId = refundRequest.TransactionId,
        RefundAmount = refundRequest.Amount,
        Sku = refundRequest.OrderId,
        Reason = refundRequest.Reason
    };

    var response = await _bkashService.RefundPaymentAsync(request);

    if (response.IsCompleted)
    {
        return Ok(new
        {
            success = true,
            refundTrxId = response.RefundTrxID,
            originalTrxId = response.OriginalTrxID,
            message = "Refund processed successfully"
        });
    }

    return BadRequest(new
    {
        success = false,
        errorCode = response.ExternalCode,
        message = response.ErrorMessageEn
    });
}

public record RefundRequest(
    string PaymentId,
    string TransactionId,
    double Amount,
    string OrderId,
    string Reason
);
```

## Checking Refund Status

Query the status of a refund:

```csharp
[HttpGet("refund/status/{paymentId}/{transactionId}")]
public async Task<IActionResult> GetRefundStatus(
    string paymentId,
    string transactionId)
{
    var response = await _bkashService.QueryRefundStatusAsync(
        paymentId,
        transactionId);

    return Ok(new
    {
        originalTransactionId = response.OriginalTrxId,
        originalAmount = response.OriginalTrxAmount,
        refunds = response.RefundTransactions.Select(r => new
        {
            refundTrxId = r.RefundTrxId,
            amount = r.Amount,
            completionTime = r.CompletionTime
        })
    });
}
```

## Minimal API Example

Complete example using .NET Minimal APIs:

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();

app.MapHealthChecks("/health");

// Create payment endpoint
app.MapPost("/payment/create", async (
    double amount,
    IBkashPaymentService bkashService) =>
{
    var request = new BkashCreatePaymentRequest
    {
        Amount = amount,
        MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
        PayerReference = "01712345678",  // Customer phone number
        Intent = "sale"
    };

    var response = await bkashService.CreatePaymentAsync(request);
    return Results.Ok(new { bkashUrl = response.BkashURL, paymentId = response.PaymentID });
});

// Execute payment endpoint
app.MapPost("/payment/execute/{paymentId}", async (
    string paymentId,
    IBkashPaymentService bkashService) =>
{
    var response = await bkashService.ExecutePaymentAsync(paymentId);
    return response.IsCompleted
        ? Results.Ok(new { success = true, transactionId = response.TrxID })
        : Results.BadRequest(new { success = false });
});

// Query payment status
app.MapGet("/payment/status/{paymentId}", async (
    string paymentId,
    IBkashPaymentService bkashService) =>
{
    var response = await bkashService.QueryPaymentAsync(paymentId);
    return Results.Ok(response);
});

app.Run();
```

## Console Application Example

Example for console applications:

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Setup DI container
var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Add bKash services
services.AddBkashPayment(options =>
{
    options.AppKey = "4f6o0cjiki2rfm34kfdadl1eqq";
    options.AppSecret = "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b";
    options.Username = "sandboxTokenizedUser02";
    options.Password = "sandboxTokenizedUser02@12345";
    options.Environment = BkashEnvironment.Sandbox;
});

var serviceProvider = services.BuildServiceProvider();

// Use the service
var bkashService = serviceProvider.GetRequiredService<IBkashPaymentService>();

Console.WriteLine("Creating payment...");

var request = new BkashCreatePaymentRequest
{
    Amount = 100,
    MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
    PayerReference = "01712345678",  // Customer phone number
    Intent = "sale"
};

var response = await bkashService.CreatePaymentAsync(request);

Console.WriteLine($"Payment created successfully!");
Console.WriteLine($"Payment ID: {response.PaymentID}");
Console.WriteLine($"bKash URL: {response.BkashURL}");
Console.WriteLine($"\nRedirect user to the bKash URL to complete payment.");
```

## Error Handling Example

Proper error handling with specific exception types:

```csharp
using Bikiran.Payment.Bkash.Exceptions;

[HttpPost("create")]
public async Task<IActionResult> CreatePayment([FromBody] double amount)
{
    try
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
            PayerReference = "01712345678",  // Customer phone number
            Intent = "sale"
        };

        var response = await _bkashService.CreatePaymentAsync(request);
        return Ok(response);
    }
    catch (BkashAuthenticationException ex)
    {
        // Authentication failed (invalid credentials)
        return Unauthorized(new { error = "Authentication failed", details = ex.Message });
    }
    catch (BkashPaymentException ex)
    {
        // Payment operation failed
        return BadRequest(new
        {
            error = "Payment operation failed",
            errorCode = ex.ErrorCode,
            message = ex.Message
        });
    }
    catch (BkashException ex)
    {
        // General bKash error
        return StatusCode(500, new
        {
            error = "bKash service error",
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        // Unexpected error
        return StatusCode(500, new { error = "Unexpected error", message = ex.Message });
    }
}
```

## Next Steps

- 📖 [Payment Flow Guide](../guides/payment-flow.md) - Detailed payment flow walkthrough
- 🔒 [Security Best Practices](../guides/security-best-practices.md) - Secure your integration
- 🔧 [API Reference](../api-reference/) - Complete API documentation
- 🌐 [Production Deployment](../guides/production-deployment.md) - Deploy your application

---

For more examples, check the [API Reference](../api-reference/) section.
