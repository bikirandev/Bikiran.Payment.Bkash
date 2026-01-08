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

### 2. Use in Your Code

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
    "Environment": "Sandbox"
  }
}
```

### Environment Variables

```
BKASH_APP_KEY=your-app-key
BKASH_APP_SECRET=your-app-secret
BKASH_USERNAME=your-username
BKASH_PASSWORD=your-password
```

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
catch (BkashException ex)
{
    // Handle bKash-specific errors
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
}
```

## License

MIT

## Support

For issues and feature requests, please visit [GitHub Issues](https://github.com/bikiran/bkash-dotnet/issues).
