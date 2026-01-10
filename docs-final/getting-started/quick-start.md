# Quick Start Guide

Get up and running with Bikiran.Payment.Bkash in less than 5 minutes!

## Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022, VS Code, or Rider (recommended)
- bKash sandbox credentials (see [Getting Credentials](#getting-credentials))

## Installation

Install the NuGet package:

```bash
dotnet add package Bikiran.Payment.Bkash
```

### Other Installation Methods

**Package Manager Console (Visual Studio):**

```powershell
Install-Package Bikiran.Payment.Bkash
```

**Manual Package Reference (.csproj):**

```xml
<ItemGroup>
  <PackageReference Include="Bikiran.Payment.Bkash" Version="1.0.0" />
</ItemGroup>
```

Then restore:

```bash
dotnet restore
```

## Basic Setup

### 1. Configure Services in Program.cs

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

// Add bKash payment services
builder.Services.AddBkashPayment(builder.Configuration);

// Optional: Add health checks
builder.Services.AddHealthChecks()
    .AddBkashHealthCheck("bkash");

var app = builder.Build();

// Optional: Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
```

### 2. Add Configuration in appsettings.json

```json
{
  "Bkash": {
    "AppKey": "your-app-key",
    "AppSecret": "your-app-secret",
    "Username": "your-username",
    "Password": "your-password",
    "Environment": "Sandbox"
  }
}
```

### 3. Create Your First Payment

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
    public async Task<IActionResult> CreatePayment([FromBody] decimal amount)
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
            Intent = "sale"
        };

        var response = await _bkashService.CreatePaymentAsync(request);

        // Redirect user to response.BkashURL for payment
        return Ok(new {
            bkashUrl = response.BkashURL,
            paymentId = response.PaymentID
        });
    }

    [HttpPost("execute/{paymentId}")]
    public async Task<IActionResult> ExecutePayment(string paymentId)
    {
        var response = await _bkashService.ExecutePaymentAsync(paymentId);

        if (response.IsCompleted)
        {
            return Ok(new {
                success = true,
                transactionId = response.TrxID,
                message = "Payment successful"
            });
        }

        return BadRequest(new {
            success = false,
            message = "Payment failed"
        });
    }
}
```

## Getting Credentials

### Sandbox Credentials (for Testing)

You can use the default sandbox credentials for testing:

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

### Production Credentials

For production:

1. Visit [https://www.bka.sh/](https://www.bka.sh/)
2. Apply for a merchant account
3. Complete the KYC process
4. Receive your production credentials

**⚠️ Never commit production credentials to source control!**

## Payment Flow

1. **Customer initiates payment** → Your application calls `CreatePaymentAsync()`
2. **Redirect to bKash** → Customer completes payment on bKash payment page
3. **Customer returns** → bKash redirects back to your callback URL with status
4. **Execute payment** → Your application calls `ExecutePaymentAsync()` to complete
5. **Payment confirmed** → Transaction complete with TrxID

## Testing Your Integration

Run your application:

```bash
dotnet run
```

Test the health check endpoint:

```bash
curl http://localhost:5000/health
```

Expected response:

```
Healthy
```

## Verifying Installation

Check installed packages:

```bash
dotnet list package
```

You should see:

```
Bikiran.Payment.Bkash    1.0.0
```

## Common Issues

### Issue: "Authentication failed"

**Solution:** Verify your credentials match your environment (Sandbox vs Production)

### Issue: "Configuration section 'Bkash' not found"

**Solution:** Ensure your appsettings.json has the correct structure

### Issue: "Payment creation succeeds but execution fails"

**Solution:** Ensure you're using the correct PaymentID and the payment hasn't expired (15-minute timeout)

### Issue: Package not found

**Solution:**

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

## Project Types

### ASP.NET Core Web API

```bash
dotnet new webapi -n MyBkashApi
cd MyBkashApi
dotnet add package Bikiran.Payment.Bkash
dotnet run
```

### ASP.NET Core MVC

```bash
dotnet new mvc -n MyBkashWeb
cd MyBkashWeb
dotnet add package Bikiran.Payment.Bkash
dotnet run
```

### Minimal API

```bash
dotnet new web -n MyBkashMinimal
cd MyBkashMinimal
dotnet add package Bikiran.Payment.Bkash
dotnet run
```

### Console Application

```bash
dotnet new console -n MyBkashConsole
cd MyBkashConsole
dotnet add package Bikiran.Payment.Bkash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging.Console
dotnet run
```

## Next Steps

- 📖 [Code Examples](examples.md) - More comprehensive code examples
- ⚙️ [Configuration Guide](../configuration/configuration-guide.md) - Advanced configuration options
- 🔒 [Security Best Practices](../guides/security-best-practices.md) - Secure your integration
- 📖 [API Reference](../api-reference/) - Detailed API documentation

## Need Help?

- 📚 Full documentation: [Documentation Index](../README.md)
- 🐛 Report issues: [GitHub Issues](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues)
- 💬 Ask questions: [GitHub Discussions](https://github.com/bikirandev/Bikiran.Payment.Bkash/discussions)

---

**Congratulations!** 🎉 You've set up Bikiran.Payment.Bkash and are ready to process payments.

        return BadRequest(new { success = false, message = "Payment failed" });
    }

}

````

## Getting Credentials

### Sandbox Credentials (for Testing)

You can use the default sandbox credentials for testing:

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
````

### Production Credentials

For production:

1. Visit [https://www.bka.sh/](https://www.bka.sh/)
2. Apply for a merchant account
3. Complete the KYC process
4. Receive your production credentials

**⚠️ Never commit production credentials to source control!**

## Payment Flow

1. **Customer initiates payment** → Your application calls `CreatePaymentAsync()`
2. **Redirect to bKash** → Customer completes payment on bKash payment page (BkashURL)
3. **Customer returns** → bKash redirects back to your callback URL with status
4. **Execute payment** → Your application calls `ExecutePaymentAsync()` to complete the transaction
5. **Payment confirmed** → Transaction complete with TrxID

## Next Steps

- 📖 [Complete Installation Guide](installation.md) - More installation options
- 🎯 [Basic Examples](basic-examples.md) - More code examples
- ⚙️ [Configuration Guide](../configuration/overview.md) - Advanced configuration
- 🔒 [Security Best Practices](../guides/security-best-practices.md) - Secure your integration

## Testing Your Integration

To test your integration:

```bash
# Run your application
dotnet run

# Test the health check endpoint
curl http://localhost:5000/health

# Expected response:
# {"status":"Healthy"}
```

## Common Issues

### Issue: "Authentication failed"

**Solution**: Verify your credentials match your environment (Sandbox vs Production)

### Issue: "Configuration section 'Bkash' not found"

**Solution**: Ensure your appsettings.json has the correct structure and is being loaded

### Issue: "Payment creation succeeds but execution fails"

**Solution**: Make sure you're using the correct PaymentID and the payment hasn't expired (15-minute timeout)

## Need Help?

- 📚 Full documentation: [Documentation Index](../README.md)
- 🐛 Report issues: [GitHub Issues](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues)
- 💬 Ask questions: [GitHub Discussions](https://github.com/bikirandev/Bikiran.Payment.Bkash/discussions)

---

**Congratulations!** 🎉 You've set up Bikiran.Payment.Bkash and created your first payment integration.
