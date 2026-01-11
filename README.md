# Bikiran.Payment.Bkash

[![NuGet](https://img.shields.io/nuget/v/Bikiran.Payment.Bkash.svg)](https://www.nuget.org/packages/Bikiran.Payment.Bkash)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Bikiran.Payment.Bkash.svg)](https://www.nuget.org/packages/Bikiran.Payment.Bkash)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)

A comprehensive .NET library for integrating **bKash Payment Gateway** with automatic token management, payment processing, and refund capabilities. Built for production use with .NET 9.0.

> **⚠️ Important:** This library is aligned with **bKash Tokenized Checkout API v1.2.0-beta**. All code examples include the mandatory `payerReference` field. See [Important API Updates](docs/IMPORTANT_API_UPDATES.md) for details.

## 📑 Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [Webhook Verification](#webhook-verification)
- [Payment Modes](#payment-modes)
- [Error Handling](#error-handling)
- [Documentation](#documentation)
- [Links](#-links)
- [Support](#-support)

## Features

- ✅ **Automatic Token Management** - No need to manually handle token refresh (55-minute ID tokens, 28-day refresh tokens)
- ✅ **Complete Payment Workflow** - Create, execute, and query payments with ease
- ✅ **Refund Support** - Full and partial refund processing with status tracking
- ✅ **Multi-Environment** - Seamless switching between sandbox and production
- ✅ **Production-Ready** - Built-in error handling, logging, and retry logic
- ✅ **Type-Safe** - Strongly-typed request/response models with IntelliSense support
- ✅ **Standardized Responses** - Generic endpoint wrapper for consistent API responses
- ✅ **Modern .NET** - Built for .NET 9.0 with async/await patterns
- ✅ **Dependency Injection** - Native DI support for ASP.NET Core
- ✅ **Health Checks** - Integration with ASP.NET Core health monitoring
- ✅ **Webhook Security** - Built-in signature verification for webhooks
- ✅ **Well Documented** - Comprehensive documentation with code examples

## Why Choose This Library?

- 🎯 **Official API Compliance** - Aligned with bKash Tokenized Checkout API v1.2.0-beta
- 🚀 **Zero Configuration Hassle** - Smart defaults with flexible configuration options
- 🔒 **Security First** - HMAC-SHA256 webhook verification and secure credential handling
- 📊 **Observable** - Built-in logging and health check support for monitoring
- 🧪 **Tested** - Comprehensive examples and testing guides included
- 📚 **Complete Documentation** - Step-by-step guides for every scenario

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

    public async Task<string> CreatePaymentAsync(double amount, string invoiceNumber, string customerPhone)
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = invoiceNumber,
            PayerReference = customerPhone,  // Required: Customer phone number
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

- [Configuration Guide](docs/configuration/configuration-guide.md) - All configuration methods and options
- [Environment Setup](docs/configuration/environment-setup.md) - Platform-specific deployment setup

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

- [Payment Operations](docs/api-reference/payment-operations.md)
- [Refund Operations](docs/api-reference/refund-operations.md)
- [Token Management](docs/api-reference/token-management.md)
- [Webhook Handling](docs/api-reference/webhook-handling.md)
- [Health Checks](docs/api-reference/health-checks.md)
- [Endpoint Wrapper](docs/api-reference/endpoint-wrapper.md)

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

- `0011` - Tokenized Checkout (URL-based, one-time payment) - **Default for this library**
- `0001` - Non-tokenized Checkout (legacy)
- `0002` - Pre-Authorization
- `0021` - Disbursement
- `0031` - Refund

> **Note:** This library is designed for Tokenized Checkout (Mode 0011) which is the recommended approach for modern integrations.

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

- [Error Handling Guide](docs/guides/error-handling.md)

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

📚 **Complete documentation is available in the [docs](docs/) folder:**

- **[Quick Start Guide](docs/getting-started/quick-start.md)** - Get started in 5 minutes
- **[Code Examples](docs/getting-started/examples.md)** - Comprehensive examples
- **[Configuration Guide](docs/configuration/configuration-guide.md)** - All configuration options
- **[API Reference](docs/api-reference/)** - Complete API documentation
- **[Guides](docs/guides/)** - Payment flow, error handling, security
- **[Development](docs/development/)** - Contributing and project structure

## 🔗 Links

- **NuGet Package**: [Bikiran.Payment.Bkash](https://www.nuget.org/packages/Bikiran.Payment.Bkash)
- **GitHub Repository**: [bikirandev/Bikiran.Payment.Bkash](https://github.com/bikirandev/Bikiran.Payment.Bkash)
- **Documentation**: [GitHub Pages](https://github.com/bikirandev/Bikiran.Payment.Bkash/tree/main/docs)
- **Issue Tracker**: [GitHub Issues](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues)
- **Discussions**: [GitHub Discussions](https://github.com/bikirandev/Bikiran.Payment.Bkash/discussions)

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](docs/development/contributing.md) before submitting pull requests.

## 💬 Support

- **Issues**: Report bugs or request features via [GitHub Issues](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues)
- **Discussions**: Ask questions or share ideas on [GitHub Discussions](https://github.com/bikirandev/Bikiran.Payment.Bkash/discussions)
- **bKash Support**: For bKash-specific questions, contact merchant.service@bka.sh

## ⭐ Show Your Support

If this library helps you, please give it a ⭐ on [GitHub](https://github.com/bikirandev/Bikiran.Payment.Bkash)!

---

**Made with ❤️ for the .NET community**
