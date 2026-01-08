# Usage Examples

This document provides comprehensive examples of using the Bikiran.Payment.Bkash library.

## Table of Contents
- [Basic Setup](#basic-setup)
- [Installation](#installation)
- [Environment Variables Configuration](#environment-variables-configuration)
- [Payment Flow](#payment-flow)
- [Refund Operations](#refund-operations)
- [Webhook Handling](#webhook-handling)
- [Health Monitoring](#health-monitoring)
- [Error Handling](#error-handling)
- [Advanced Configuration](#advanced-configuration)

---

## Installation

### From NuGet.org (Recommended)

```powershell
dotnet add package Bikiran.Payment.Bkash
```

### From Local Package Source (For Testing)

```powershell
# Add local source
dotnet nuget add source "D:\Path\To\Package\bin\Release" -n LocalBkash

# Install package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0 --source LocalBkash
```

**Troubleshooting Local Installation**:

If you encounter `NU1301: The local source doesn't exist` error:

```powershell
# Solution 1: Remove and re-add source
dotnet nuget remove source LocalBkash
dotnet nuget add source "D:\Path\To\Package\bin\Release" -n LocalBkash

# Solution 2: Create local nuget.config
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="LocalBkash" value="D:\Path\To\Package\bin\Release" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath nuget.config -Encoding utf8

# Then restore
dotnet restore
```

See [LOCAL_PACKAGE_TEST.md](LOCAL_PACKAGE_TEST.md) for detailed local testing guide.

---

## Basic Setup

### Configure in Program.cs (Minimal API)

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

### Configure in Program.cs (MVC/API)

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
```

### appsettings.json

```json
{
  "Bkash": {
    "AppKey": "4f6o0cjiki2rfm34kfdadl1eqq",
    "AppSecret": "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b",
    "Username": "sandboxTokenizedUser02",
    "Password": "sandboxTokenizedUser02@12345",
    "Environment": "Sandbox",
    "TimeoutSeconds": 30,
    "TokenRefreshBufferSeconds": 300
  }
}
```

---

## Environment Variables Configuration

### Using .env Files (Recommended for Local Development)

**Step 1: Install DotNetEnv Package** (Optional but Recommended)

```bash
dotnet add package DotNetEnv
```

**Step 2: Create `.env` file in your project root**

```env
# Copy from .env.example provided in the package
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
BKASH__TIMEOUTSECONDS=30
BKASH__TOKENREFRESHBUFFERSECONDS=300
```

**Step 3: Load .env in Program.cs**

```csharp
using DotNetEnv;
using Bikiran.Payment.Bkash;

// Load .env file at startup
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Environment variables are automatically loaded into configuration
builder.Services.AddBkashPayment(builder.Configuration);

var app = builder.Build();
app.Run();
```

### Without DotNetEnv Package

You can also set environment variables directly in your system or use launchSettings.json:

**launchSettings.json**
```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BKASH__APPKEY": "4f6o0cjiki2rfm34kfdadl1eqq",
        "BKASH__APPSECRET": "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b",
        "BKASH__USERNAME": "sandboxTokenizedUser02",
        "BKASH__PASSWORD": "sandboxTokenizedUser02@12345",
        "BKASH__ENVIRONMENT": "Sandbox"
      }
    }
  }
}
```

### Docker Environment Variables

**docker-compose.yml**
```yaml
version: '3.8'
services:
  api:
    image: your-api-image
    environment:
      - BKASH__APPKEY=${BKASH_APPKEY}
      - BKASH__APPSECRET=${BKASH_APPSECRET}
      - BKASH__USERNAME=${BKASH_USERNAME}
      - BKASH__PASSWORD=${BKASH_PASSWORD}
      - BKASH__ENVIRONMENT=Production
    env_file:
      - .env.production
```

### Azure App Service Configuration

For Azure deployments, set environment variables in App Service Configuration:

```bash
# Using Azure CLI
az webapp config appsettings set \
  --name your-app-name \
  --resource-group your-resource-group \
  --settings \
    BKASH__APPKEY="your-app-key" \
    BKASH__APPSECRET="your-app-secret" \
    BKASH__USERNAME="your-username" \
    BKASH__PASSWORD="your-password" \
    BKASH__ENVIRONMENT="Production"
```

### AWS Elastic Beanstalk Configuration

**In .ebextensions/environment.config**
```yaml
option_settings:
  aws:elasticbeanstalk:application:environment:
    BKASH__APPKEY: "your-app-key"
    BKASH__APPSECRET: "your-app-secret"
    BKASH__USERNAME: "your-username"
    BKASH__PASSWORD: "your-password"
    BKASH__ENVIRONMENT: "Production"
```

### Kubernetes Secrets

**Create Secret**
```bash
kubectl create secret generic bkash-credentials \
  --from-literal=appkey='your-app-key' \
  --from-literal=appsecret='your-app-secret' \
  --from-literal=username='your-username' \
  --from-literal=password='your-password'
```

**Use in Deployment**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: your-api
spec:
  template:
    spec:
      containers:
      - name: api
        image: your-api-image
        env:
        - name: BKASH__APPKEY
          valueFrom:
            secretKeyRef:
              name: bkash-credentials
              key: appkey
        - name: BKASH__APPSECRET
          valueFrom:
            secretKeyRef:
              name: bkash-credentials
              key: appsecret
        - name: BKASH__USERNAME
          valueFrom:
            secretKeyRef:
              name: bkash-credentials
              key: username
        - name: BKASH__PASSWORD
          valueFrom:
            secretKeyRef:
              name: bkash-credentials
              key: password
        - name: BKASH__ENVIRONMENT
          value: "Production"
```

### Important Security Notes

?? **DO NOT** commit `.env` files with actual credentials to source control!

**Add to .gitignore:**
```gitignore
# Environment files with credentials
.env
.env.local
.env.production
.env.*.local

# Keep example files
!.env.example
!.env.sandbox
!.env.production.template
```

**Recommended practices:**
1. Use `.env.example` or `.env.template` files for documentation
2. Use Azure Key Vault, AWS Secrets Manager, or HashiCorp Vault in production
3. Rotate credentials regularly
4. Use different credentials for each environment
5. Limit access to production credentials

---

## Basic Setup

### Configure in Program.cs (Minimal API)

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

### Configure in Program.cs (MVC/API)

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
```

### appsettings.json

```json
{
  "Bkash": {
    "AppKey": "4f6o0cjiki2rfm34kfdadl1eqq",
    "AppSecret": "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b",
    "Username": "sandboxTokenizedUser02",
    "Password": "sandboxTokenizedUser02@12345",
    "Environment": "Sandbox",
    "TimeoutSeconds": 30,
    "TokenRefreshBufferSeconds": 300
  }
}
```

---

## Payment Flow

### Complete Payment Controller Example

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

    public PaymentController(
        IBkashPaymentService bkashService,
        ILogger<PaymentController> logger)
    {
        _bkashService = bkashService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
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
            return BadRequest(new
            {
                success = false,
                errorCode = ex.ErrorCode,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Execute payment after user authorization
    /// </summary>
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
            return BadRequest(new
            {
                success = false,
                errorCode = ex.ErrorCode,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Query payment status
    /// </summary>
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
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }
}

public record CreatePaymentDto(decimal Amount);
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
// bKash will redirect to your callback URL with paymentID and status
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

---

## Refund Operations

### Refund Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class RefundController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;
    private readonly ILogger<RefundController> _logger;

    public RefundController(
        IBkashPaymentService bkashService,
        ILogger<RefundController> logger)
    {
        _bkashService = bkashService;
        _logger = logger;
    }

    /// <summary>
    /// Process a refund
    /// </summary>
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
            return BadRequest(new
            {
                success = false,
                errorCode = ex.ErrorCode,
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Check refund status
    /// </summary>
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
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
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

---

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

    public BkashWebhookController(
        IConfiguration configuration,
        ILogger<BkashWebhookController> logger)
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
                // Update order status to paid
                await HandlePaymentCompleted(notification);
                break;
                
            case "cancelled":
                // Update order status to cancelled
                await HandlePaymentCancelled(notification);
                break;
                
            case "failed":
                // Update order status to failed
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

---

## Health Monitoring

### Health Check Endpoint

```csharp
// In Program.cs
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
```

### Custom Health Check UI (Optional)

```html
<!DOCTYPE html>
<html>
<head>
    <title>bKash Service Health</title>
</head>
<body>
    <h1>bKash Payment Service Health</h1>
    <div id="health-status"></div>
    
    <script>
        async function checkHealth() {
            const response = await fetch('/health');
            const data = await response.json();
            
            const statusDiv = document.getElementById('health-status');
            statusDiv.innerHTML = `
                <h2>Status: ${data.status}</h2>
                <ul>
                    ${data.checks.map(check => `
                        <li>
                            <strong>${check.name}</strong>: ${check.status}
                            <br>Duration: ${check.duration}ms
                        </li>
                    `).join('')}
                </ul>
            `;
        }
        
        checkHealth();
        setInterval(checkHealth, 30000); // Check every 30 seconds
    </script>
</body>
</html>
```

---

## Error Handling

### Global Exception Handler

```csharp
using Bikiran.Payment.Bkash.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

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
                    message = "Authentication failed with bKash",
                    details = authEx.Message
                }),
                
                BkashPaymentException paymentEx => (400, new
                {
                    success = false,
                    errorCode = paymentEx.ErrorCode,
                    message = "Payment operation failed",
                    details = paymentEx.Message,
                    httpStatusCode = paymentEx.HttpStatusCode
                }),
                
                BkashException bkashEx => (500, new
                {
                    success = false,
                    errorCode = bkashEx.ErrorCode,
                    message = "bKash service error",
                    details = bkashEx.Message
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

---

## Advanced Configuration

### Environment-Specific Configuration

```csharp
// appsettings.Development.json
{
  "Bkash": {
    "Environment": "Sandbox",
    "TimeoutSeconds": 60
  }
}

// appsettings.Production.json
{
  "Bkash": {
    "Environment": "Production",
    "TimeoutSeconds": 30,
    "TokenRefreshBufferSeconds": 600
  }
}
```

### Using Azure Key Vault

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = builder.Configuration["KeyVaultEndpoint"];
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential());
}

builder.Services.AddBkashPayment(options =>
{
    options.AppKey = builder.Configuration["Bkash--AppKey"];
    options.AppSecret = builder.Configuration["Bkash--AppSecret"];
    options.Username = builder.Configuration["Bkash--Username"];
    options.Password = builder.Configuration["Bkash--Password"];
    options.Environment = BkashEnvironment.Production;
});
```

### Custom HttpClient Configuration

```csharp
builder.Services.AddBkashPayment(builder.Configuration);

// Override HttpClient configuration if needed
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(); // Add resilience (Polly)
});
```

---

## Complete Minimal API Example

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

---

## Testing

### Unit Test Example (Using Moq)

```csharp
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Moq;
using Xunit;

public class PaymentServiceTests
{
    [Fact]
    public async Task CreatePayment_ValidRequest_ReturnsPaymentUrl()
    {
        // Arrange
        var mockBkashService = new Mock<IBkashPaymentService>();
        mockBkashService
            .Setup(x => x.CreatePaymentAsync(It.IsAny<BkashCreatePaymentRequest>(), default))
            .ReturnsAsync(new BkashCreatePaymentResponse
            {
                PaymentID = "test-payment-id",
                BkashURL = "https://sandbox.bka.sh/checkout/test",
                StatusCode = "0000"
            });

        // Act
        var request = new BkashCreatePaymentRequest
        {
            Amount = 100,
            MerchantInvoiceNumber = "INV-001",
            Intent = "sale"
        };

        var result = await mockBkashService.Object.CreatePaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("test-payment-id", result.PaymentID);
    }
}
```

---

This covers the most common use cases for the Bikiran.Payment.Bkash library. For more information, refer to the README.md and API documentation.
