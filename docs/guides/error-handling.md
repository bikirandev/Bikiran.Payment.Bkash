# Error Handling Guide

Comprehensive guide to handling errors and exceptions in bKash payment integration.

## Exception Hierarchy

```
Exception
└── BkashException (base for all bKash errors)
    ├── BkashAuthenticationException (authentication failures)
    ├── BkashPaymentException (payment operation failures)
    └── BkashConfigurationException (configuration issues)
```

## Exception Types

### BkashException

Base exception for all bKash-related errors.

**Properties:**

- `ErrorCode` (string) - bKash error code
- `HttpStatusCode` (int?) - HTTP status code if applicable
- `Message` (string) - Error message

**When thrown:**

- General bKash service errors
- Network connectivity issues
- Unexpected API responses

**Example:**

```csharp
catch (BkashException ex)
{
    _logger.LogError(ex, "bKash error: {ErrorCode}", ex.ErrorCode);
    return StatusCode(500, new
    {
        error = "Payment service unavailable",
        errorCode = ex.ErrorCode,
        message = "Please try again later"
    });
}
```

### BkashAuthenticationException

Thrown when authentication with bKash API fails.

**Common causes:**

- Invalid credentials (AppKey, AppSecret, Username, Password)
- Expired or invalid token
- Incorrect environment configuration
- Token refresh failures

**Example:**

```csharp
catch (BkashAuthenticationException ex)
{
    _logger.LogError(ex, "Authentication failed");

    // Check configuration
    var config = _configuration.GetSection("Bkash");
    _logger.LogWarning("Using environment: {Environment}", config["Environment"]);

    return Unauthorized(new {
        error = "Authentication failed",
        message = "Please check your bKash credentials"
    });
}
```

### BkashPaymentException

Thrown when payment operations fail.

**Properties:**

- Inherits from `BkashException`
- Contains specific payment error codes from bKash API

**Common causes:**

- Invalid payment amount
- Payment already executed
- Payment expired (15-minute timeout)
- Insufficient balance
- Payment not found
- Invalid merchant invoice number

**Example:**

```csharp
catch (BkashPaymentException ex)
{
    _logger.LogError(ex, "Payment failed: {ErrorCode}", ex.ErrorCode);

    var userMessage = ex.ErrorCode switch
    {
        "2001" => "Invalid payment amount",
        "2014" => "Payment has already been processed",
        "2029" => "Payment has expired. Please create a new payment.",
        "2062" => "Payment not found",
        _ => "Payment processing failed"
    };

    return BadRequest(new
    {
        error = "Payment operation failed",
        errorCode = ex.ErrorCode,
        message = userMessage
    });
}
```

### BkashConfigurationException

Thrown when configuration is invalid or missing.

**Common causes:**

- Missing required configuration values
- Invalid environment value
- Invalid timeout settings

**Example:**

```csharp
catch (BkashConfigurationException ex)
{
    _logger.LogCritical(ex, "Configuration error");

    return StatusCode(500, new
    {
        error = "Service configuration error",
        message = "Please contact support"
    });
}
```

## Common Error Codes

### Authentication Errors

| Code | Description  | Solution                                    |
| ---- | ------------ | ------------------------------------------- |
| 401  | Unauthorized | Check AppKey, AppSecret, Username, Password |
| 403  | Forbidden    | Verify account permissions                  |

### Payment Errors

| Code | Description              | Solution                                 |
| ---- | ------------------------ | ---------------------------------------- |
| 2001 | Invalid amount           | Ensure amount > 0 and properly formatted |
| 2002 | Invalid merchant invoice | Use unique invoice numbers               |
| 2014 | Payment already executed | Implement idempotency checks             |
| 2027 | Configuration error      | Verify bKash merchant configuration      |
| 2029 | Payment expired          | Payments expire after 15 minutes         |
| 2033 | Invalid payment ID       | Verify payment ID format                 |
| 2062 | Payment not found        | Check payment ID exists                  |

### Refund Errors

| Code | Description                    | Solution                       |
| ---- | ------------------------------ | ------------------------------ |
| 2023 | Insufficient balance           | Wait or contact bKash support  |
| 2053 | Invalid payment for refund     | Verify payment is completed    |
| 2054 | Refund amount exceeds payment  | Check refund amount ≤ original |
| 2055 | Payment already fully refunded | Track refund history           |

### Token Errors

| Code | Description   | Solution                           |
| ---- | ------------- | ---------------------------------- |
| 401  | Token expired | Library auto-refreshes; check logs |
| 403  | Invalid token | Check credentials configuration    |

## Error Handling Strategies

### 1. Layered Exception Handling

```csharp
[HttpPost("create")]
public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
{
    try
    {
        // Validate request
        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "Amount must be greater than 0" });
        }

        var bkashRequest = new BkashCreatePaymentRequest
        {
            Amount = request.Amount,
            MerchantInvoiceNumber = request.InvoiceNumber,
            PayerReference = request.CustomerPhone,  // Required field
            Intent = "sale"
        };

        var response = await _bkashService.CreatePaymentAsync(bkashRequest);
        return Ok(response);
    }
    catch (BkashAuthenticationException ex)
    {
        _logger.LogError(ex, "Authentication failed");
        return Unauthorized(new { error = "Authentication failed" });
    }
    catch (BkashPaymentException ex)
    {
        _logger.LogError(ex, "Payment failed: {ErrorCode}", ex.ErrorCode);
        return BadRequest(new
        {
            error = "Payment failed",
            errorCode = ex.ErrorCode,
            message = GetUserFriendlyMessage(ex.ErrorCode)
        });
    }
    catch (BkashException ex)
    {
        _logger.LogError(ex, "bKash service error");
        return StatusCode(500, new { error = "Service temporarily unavailable" });
    }
    catch (Exception ex)
    {
        _logger.LogCritical(ex, "Unexpected error");
        return StatusCode(500, new { error = "An unexpected error occurred" });
    }
}

private string GetUserFriendlyMessage(string errorCode)
{
    return errorCode switch
    {
        "2001" => "Invalid payment amount",
        "2014" => "Payment already processed",
        "2029" => "Payment expired. Please try again.",
        "2062" => "Payment not found",
        _ => "Payment processing failed. Please try again."
    };
}
```

### 2. Global Exception Handler

```csharp
public class BkashExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BkashExceptionMiddleware> _logger;

    public BkashExceptionMiddleware(RequestDelegate next, ILogger<BkashExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BkashAuthenticationException ex)
        {
            _logger.LogError(ex, "bKash authentication failed");
            await HandleExceptionAsync(context, StatusCodes.Status401Unauthorized, "Authentication failed", ex.ErrorCode);
        }
        catch (BkashPaymentException ex)
        {
            _logger.LogError(ex, "bKash payment failed: {ErrorCode}", ex.ErrorCode);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Payment failed", ex.ErrorCode);
        }
        catch (BkashException ex)
        {
            _logger.LogError(ex, "bKash service error: {ErrorCode}", ex.ErrorCode);
            await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "Service error", ex.ErrorCode);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message, string errorCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new
        {
            error = message,
            errorCode = errorCode,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

// Register in Program.cs
app.UseMiddleware<BkashExceptionMiddleware>();
```

### 3. Retry Logic with Polly

```csharp
using Polly;
using Polly.Extensions.Http;

// In Program.cs
builder.Services.AddBkashPayment(builder.Configuration);

builder.Services.AddHttpClient<IBkashPaymentService, BkashPaymentService>()
    .AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

## Validation Best Practices

### Request Validation

```csharp
public class BkashCreatePaymentRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public double Amount { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Invoice number too long")]
    public string MerchantInvoiceNumber { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Payer reference required")]
    public string PayerReference { get; set; }  // Customer phone or ID

    [Required]
    [RegularExpression("sale|authorization")]
    public string Intent { get; set; }
}
```

### Pre-flight Checks

```csharp
public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
{
    // Validate request
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Check for duplicate invoice
    if (await _orderService.InvoiceExistsAsync(request.InvoiceNumber))
    {
        return Conflict(new { error = "Invoice number already exists" });
    }

    // Business logic validation
    if (!await _orderService.IsValidOrderAsync(request.OrderId))
    {
        return BadRequest(new { error = "Invalid order" });
    }

    // Proceed with payment
    // ...
}
```

## Logging Best Practices

### Structured Logging

```csharp
_logger.LogInformation(
    "Payment created - PaymentID: {PaymentID}, InvoiceNumber: {InvoiceNumber}, Amount: {Amount}",
    response.PaymentID,
    request.MerchantInvoiceNumber,
    request.Amount
);

_logger.LogError(
    ex,
    "Payment execution failed - PaymentID: {PaymentID}, ErrorCode: {ErrorCode}",
    paymentId,
    ex.ErrorCode
);
```

### What to Log

✅ **DO log:**

- Payment IDs
- Transaction IDs
- Invoice numbers
- Error codes
- Timestamps
- Request/response status

❌ **DON'T log:**

- Customer PINs
- Full card numbers
- Passwords or secrets
- AppSecret or credentials

## Monitoring and Alerting

### Key Metrics

```csharp
// Track success/failure rates
_metrics.IncrementCounter("payments.created.success");
_metrics.IncrementCounter("payments.created.failure", tags: new[] { $"error:{ex.ErrorCode}" });

// Track latency
var stopwatch = Stopwatch.StartNew();
await _bkashService.CreatePaymentAsync(request);
_metrics.RecordValue("payment.creation.duration", stopwatch.ElapsedMilliseconds);
```

### Alert Conditions

Set up alerts for:

- Authentication failure rate > 5% in 5 minutes
- Payment failure rate > 10% in 10 minutes
- API response time > 5 seconds
- Error rate spike (2x normal)

## Testing Error Scenarios

### Unit Tests

```csharp
[Fact]
public async Task CreatePayment_InvalidAmount_ThrowsBkashPaymentException()
{
    // Arrange
    var request = new BkashCreatePaymentRequest { Amount = -10 };

    // Act & Assert
    await Assert.ThrowsAsync<BkashPaymentException>(
        () => _bkashService.CreatePaymentAsync(request)
    );
}
```

### Integration Tests

```csharp
[Fact]
public async Task CreatePayment_InvalidCredentials_Returns401()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new PaymentRequest { Amount = 100 };

    // Act
    var response = await client.PostAsJsonAsync("/api/payment/create", request);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

## Troubleshooting Checklist

When errors occur, check:

- [ ] Configuration values are correct
- [ ] Credentials match environment (Sandbox vs Production)
- [ ] Network connectivity to bKash API
- [ ] Request data is valid
- [ ] Timeout settings are appropriate
- [ ] Token refresh is working
- [ ] Logs for detailed error messages
- [ ] bKash service status

## Production Considerations

1. **Never expose internal error details** to customers
2. **Log all errors** with full context for debugging
3. **Implement circuit breakers** for repeated failures
4. **Monitor error rates** and set up alerts
5. **Have a fallback plan** when bKash is unavailable
6. **Test error scenarios** in sandbox before production
7. **Document error handling procedures** for your team

## Next Steps

- 📖 [Payment Operations API](../api-reference/payment-operations.md) - API reference
- 📖 [Payment Flow Guide](payment-flow.md) - Complete payment flow
- 🔒 [Security Best Practices](security-best-practices.md) - Secure your integration
- 📊 [Health Checks](../api-reference/health-checks.md) - Monitor service health

---

Need help? Check the [Documentation Index](../README.md) or [open an issue](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues).
