# API Usage Guide

Comprehensive guide for using the Bikiran.Payment.Bkash library to integrate bKash payment functionality.

## Table of Contents

- [Payment Flow](#payment-flow)
- [Refund Operations](#refund-operations)
- [Webhook Handling](#webhook-handling)
- [Health Monitoring](#health-monitoring)
- [Error Handling](#error-handling)
- [Complete Examples](#complete-examples)

## Payment Flow

### Creating a Payment

```csharp
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;

public class PaymentController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IBkashPaymentService bkashService, ILogger<PaymentController> logger)
    {
        _bkashService = bkashService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        try
        {
            var request = new BkashCreatePaymentRequest
            {
                Amount = dto.Amount,
                MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
                Intent = "sale",
                Currency = "BDT",
                Mode = "0001" // Checkout mode
            };

            var response = await _bkashService.CreatePaymentAsync(request);

            return Ok(new
            {
                success = true,
                paymentId = response.PaymentID,
                bkashUrl = response.BkashURL,
                message = "Payment created successfully. Redirect user to bkashUrl"
            });
        }
        catch (BkashPaymentException ex)
        {
            _logger.LogError(ex, "Payment creation failed");
            return BadRequest(new { success = false, errorCode = ex.ErrorCode, message = ex.Message });
        }
    }
}

public record CreatePaymentDto(decimal Amount);
```

### Executing a Payment

After the user completes payment authorization on bKash:

```csharp
[HttpPost("execute/{paymentId}")]
public async Task<IActionResult> ExecutePayment(string paymentId)
{
    try
    {
        var response = await _bkashService.ExecutePaymentAsync(paymentId);

        if (response.IsCompleted)
        {
            return Ok(new
            {
                success = true,
                transactionId = response.TrxID,
                paymentId = response.PaymentID,
                amount = response.Amount,
                status = response.TransactionStatus,
                message = "Payment executed successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            status = response.TransactionStatus,
            message = "Payment execution failed"
        });
    }
    catch (BkashPaymentException ex)
    {
        _logger.LogError(ex, "Payment execution failed for {PaymentId}", paymentId);
        return BadRequest(new { success = false, errorCode = ex.ErrorCode, message = ex.Message });
    }
}
```

### Querying Payment Status

```csharp
[HttpGet("status/{paymentId}")]
public async Task<IActionResult> GetPaymentStatus(string paymentId)
{
    try
    {
        var response = await _bkashService.QueryPaymentAsync(paymentId);

        return Ok(new
        {
            success = true,
            paymentId = response.PaymentID,
            transactionId = response.TrxID,
            status = response.TransactionStatus,
            amount = response.Amount,
            completedTime = response.PaymentExecuteTime
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to query payment {PaymentId}", paymentId);
        return BadRequest(new { success = false, message = ex.Message });
    }
}
```

### Frontend Integration Example

```javascript
// Step 1: Create Payment
async function initiatePayment(amount) {
    const response = await fetch('/api/payment/create', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ amount })
    });
    
    const data = await response.json();
    
    if (data.success) {
        // Redirect user to bKash payment page
        window.location.href = data.bkashUrl;
    }
}

// Step 2: Handle Callback (after user completes payment on bKash)
async function handleCallback() {
    const urlParams = new URLSearchParams(window.location.search);
    const paymentId = urlParams.get('paymentID');
    const status = urlParams.get('status');
    
    if (status === 'success') {
        const response = await fetch(`/api/payment/execute/${paymentId}`, {
            method: 'POST'
        });
        
        const data = await response.json();
        
        if (data.success) {
            alert(`Payment successful! Transaction ID: ${data.transactionId}`);
        }
    } else {
        alert('Payment cancelled or failed');
    }
}
```

### Payment Modes

| Mode | Description | Use Case |
|------|-------------|----------|
| `0001` | Checkout (one-time payment) | Standard payments |
| `0011` | Agreement (subscription) | Recurring payments |
| `0002` | Pre-Authorization | Hold and capture |
| `0021` | Disbursement | Payouts |
| `0031` | Refund | Return funds |

## Refund Operations

### Processing a Refund

```csharp
[ApiController]
[Route("api/[controller]")]
public class RefundController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;
    private readonly ILogger<RefundController> _logger;

    public RefundController(IBkashPaymentService bkashService, ILogger<RefundController> logger)
    {
        _bkashService = bkashService;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessRefund([FromBody] RefundRequestDto dto)
    {
        try
        {
            var request = new BkashRefundPaymentRequest
            {
                PaymentId = dto.PaymentId,
                TrxId = dto.TransactionId,
                RefundAmount = dto.Amount,
                Sku = dto.OrderId,
                Reason = dto.Reason
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
        catch (BkashPaymentException ex)
        {
            _logger.LogError(ex, "Refund processing failed");
            return BadRequest(new { success = false, errorCode = ex.ErrorCode, message = ex.Message });
        }
    }
}

public record RefundRequestDto(
    string PaymentId,
    string TransactionId,
    decimal Amount,
    string OrderId,
    string Reason);
```

### Checking Refund Status

```csharp
[HttpGet("status/{paymentId}/{transactionId}")]
public async Task<IActionResult> GetRefundStatus(string paymentId, string transactionId)
{
    try
    {
        var response = await _bkashService.QueryRefundStatusAsync(paymentId, transactionId);

        return Ok(new
        {
            success = true,
            originalTrxId = response.OriginalTrxId,
            originalAmount = response.OriginalTrxAmount,
            refunds = response.RefundTransactions
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to query refund status");
        return BadRequest(new { success = false, message = ex.Message });
    }
}
```

## Webhook Handling

### Webhook Controller with Signature Verification

```csharp
using Bikiran.Payment.Bkash.Models.Webhooks;
using Bikiran.Payment.Bkash.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class BkashWebhookController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BkashWebhookController> _logger;

    public BkashWebhookController(IConfiguration configuration, ILogger<BkashWebhookController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveWebhook()
    {
        try
        {
            // Read raw body
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();

            // Get signature from header
            var signature = Request.Headers["X-Signature"].ToString();
            
            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Webhook received without signature");
                return BadRequest("Missing signature");
            }

            // Verify signature
            var appSecret = _configuration["Bkash:AppSecret"];
            if (!BkashWebhookHelper.VerifyWebhookSignature(payload, signature, appSecret))
            {
                _logger.LogWarning("Invalid webhook signature");
                return Unauthorized("Invalid signature");
            }

            // Parse webhook
            var notification = JsonConvert.DeserializeObject<BkashWebhookNotification>(payload);
            
            if (notification == null)
            {
                return BadRequest("Invalid payload");
            }

            // Verify timestamp to prevent replay attacks
            if (!BkashWebhookHelper.IsTimestampValid(notification.Timestamp))
            {
                _logger.LogWarning("Webhook timestamp is too old");
                return BadRequest("Invalid timestamp");
            }

            // Process webhook based on event type
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
            "Processing webhook: Type={EventType}, PaymentId={PaymentId}, Status={Status}",
            notification.EventType,
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
                
            default:
                _logger.LogWarning("Unknown transaction status: {Status}", notification.TransactionStatus);
                break;
        }
    }

    private async Task HandlePaymentCompleted(BkashWebhookNotification notification)
    {
        // Your business logic here
        // Example: Update database, send confirmation email, etc.
        await Task.CompletedTask;
    }

    private async Task HandlePaymentCancelled(BkashWebhookNotification notification)
    {
        // Your business logic here
        await Task.CompletedTask;
    }

    private async Task HandlePaymentFailed(BkashWebhookNotification notification)
    {
        // Your business logic here
        await Task.CompletedTask;
    }
}
```

## Health Monitoring

### Setting Up Health Checks

```csharp
// In Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddBkashHealthCheck("bkash", "payment", "external");

var app = builder.Build();

// Map health check endpoint with custom response
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        
        await context.Response.WriteAsync(result);
    }
});

app.Run();
```

### Testing Health Check

```bash
curl http://localhost:5000/health
```

Expected response:
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "bkash",
      "status": "Healthy",
      "description": "bKash payment service is available",
      "duration": 150.5
    }
  ],
  "totalDuration": 150.5
}
```

## Error Handling

### Available Exception Types

```csharp
using Bikiran.Payment.Bkash.Exceptions;

// Specific exceptions
BkashAuthenticationException    // Authentication failures
BkashPaymentException           // Payment-specific errors
BkashConfigurationException     // Configuration issues
BkashException                  // Base exception
```

### Global Exception Handler

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            var exception = contextFeature.Error;
            
            var (statusCode, errorResponse) = exception switch
            {
                BkashAuthenticationException authEx => (401, new
                {
                    success = false,
                    errorCode = authEx.ErrorCode,
                    message = "Authentication failed with bKash"
                }),
                
                BkashPaymentException paymentEx => (400, new
                {
                    success = false,
                    errorCode = paymentEx.ErrorCode,
                    message = "Payment operation failed",
                    httpStatusCode = paymentEx.HttpStatusCode
                }),
                
                BkashException bkashEx => (500, new
                {
                    success = false,
                    errorCode = bkashEx.ErrorCode,
                    message = "bKash service error"
                }),
                
                _ => (500, new
                {
                    success = false,
                    message = "An unexpected error occurred"
                })
            };
            
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    });
});
```

### Try-Catch Example

```csharp
try
{
    var response = await _bkashService.CreatePaymentAsync(request);
    // Process response
}
catch (BkashAuthenticationException ex)
{
    // Handle authentication errors
    _logger.LogError(ex, "Authentication failed");
    return Unauthorized(new { message = "Authentication failed" });
}
catch (BkashPaymentException ex)
{
    // Handle payment-specific errors
    _logger.LogError(ex, "Payment failed");
    return BadRequest(new { errorCode = ex.ErrorCode, message = ex.Message });
}
catch (BkashException ex)
{
    // Handle general bKash errors
    _logger.LogError(ex, "bKash error");
    return StatusCode(500, new { message = "Service error" });
}
```

## Complete Examples

### Minimal API Example

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Bikiran.Payment.Bkash.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapPost("/payment/create", async (
    CreatePaymentRequest request,
    IBkashPaymentService bkashService) =>
{
    try
    {
        var paymentRequest = new BkashCreatePaymentRequest
        {
            Amount = request.Amount,
            MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
            Intent = "sale"
        };

        var response = await bkashService.CreatePaymentAsync(paymentRequest);
        return Results.Ok(new { success = true, bkashUrl = response.BkashURL });
    }
    catch (BkashException ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.MapPost("/payment/execute/{paymentId}", async (
    string paymentId,
    IBkashPaymentService bkashService) =>
{
    try
    {
        var response = await bkashService.ExecutePaymentAsync(paymentId);
        return Results.Ok(new
        {
            success = response.IsCompleted,
            transactionId = response.TrxID
        });
    }
    catch (BkashException ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.Run();

record CreatePaymentRequest(decimal Amount);
```

### Full Controller Example

```csharp
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Bikiran.Payment.Bkash.Exceptions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IBkashPaymentService bkashService, ILogger<PaymentController> logger)
    {
        _bkashService = bkashService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
    {
        try
        {
            var request = new BkashCreatePaymentRequest
            {
                Amount = dto.Amount,
                MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
                Intent = "sale",
                Currency = "BDT"
            };

            var response = await _bkashService.CreatePaymentAsync(request);

            return Ok(new
            {
                success = true,
                paymentId = response.PaymentID,
                bkashUrl = response.BkashURL
            });
        }
        catch (BkashPaymentException ex)
        {
            _logger.LogError(ex, "Payment creation failed");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("execute/{paymentId}")]
    public async Task<IActionResult> ExecutePayment(string paymentId)
    {
        try
        {
            var response = await _bkashService.ExecutePaymentAsync(paymentId);
            return Ok(new
            {
                success = response.IsCompleted,
                transactionId = response.TrxID,
                amount = response.Amount
            });
        }
        catch (BkashPaymentException ex)
        {
            _logger.LogError(ex, "Payment execution failed");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("status/{paymentId}")]
    public async Task<IActionResult> GetPaymentStatus(string paymentId)
    {
        try
        {
            var response = await _bkashService.QueryPaymentAsync(paymentId);
            return Ok(new
            {
                success = true,
                status = response.TransactionStatus,
                amount = response.Amount,
                transactionId = response.TrxID
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Query failed");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public record CreatePaymentDto(decimal Amount);
```

## API Reference

### IBkashPaymentService Methods

| Method | Parameters | Returns | Description |
|--------|-----------|---------|-------------|
| `CreatePaymentAsync` | `BkashCreatePaymentRequest` | `BkashCreatePaymentResponse` | Create a new payment |
| `ExecutePaymentAsync` | `string paymentId` | `BkashExecutePaymentResponse` | Execute a payment |
| `QueryPaymentAsync` | `string paymentId` | `BkashQueryPaymentResponse` | Query payment status |
| `RefundPaymentAsync` | `BkashRefundPaymentRequest` | `BkashRefundPaymentResponse` | Process a refund |
| `QueryRefundStatusAsync` | `string paymentId, string trxId` | `BkashRefundStatusResponse` | Query refund status |

### IBkashTokenService Methods

| Method | Parameters | Returns | Description |
|--------|-----------|---------|-------------|
| `GetValidTokenAsync` | - | `string` | Get valid access token (auto-refreshes) |
| `GrantTokenAsync` | - | `BkashGrantTokenResponse` | Request new token |
| `RefreshTokenAsync` | `string refreshToken` | `BkashRefreshTokenResponse` | Refresh existing token |

## Support

- **Getting Started**: See [01-getting-started.md](01-getting-started.md)
- **Configuration**: See [02-configuration.md](02-configuration.md)
- **Quick Reference**: See [04-quick-reference.md](04-quick-reference.md)
- **GitHub Issues**: https://github.com/bikirandev/Bikiran.Payment.Bkash/issues
