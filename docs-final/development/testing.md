# Testing Guide

Guide for testing Bikiran.Payment.Bkash locally before publishing.

## Local Package Testing

### Step 1: Build Package

```bash
# Build in Release mode
dotnet build --configuration Release

# Create NuGet package
dotnet pack --configuration Release --no-build
```

Package will be created at:
```
bin/Release/Bikiran.Payment.Bkash.{version}.nupkg
```

### Step 2: Create Test Project

```bash
cd /tmp
mkdir BkashTest
cd BkashTest
dotnet new console -n TestApp
cd TestApp
```

### Step 3: Add Local Package Source

```bash
# Add local package source
dotnet nuget add source "/path/to/package/bin/Release" -n LocalBkash

# Verify source was added
dotnet nuget list source
```

### Step 4: Install Package

```bash
dotnet add package Bikiran.Payment.Bkash --version {version} --source LocalBkash
```

### Step 5: Test the Package

Create test code in `Program.cs`:

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddBkashPayment(options =>
{
    options.AppKey = "4f6o0cjiki2rfm34kfdadl1eqq";
    options.AppSecret = "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b";
    options.Username = "sandboxTokenizedUser02";
    options.Password = "sandboxTokenizedUser02@12345";
    options.Environment = BkashEnvironment.Sandbox;
});

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Testing bKash package...");

// Test token service
var tokenService = serviceProvider.GetRequiredService<IBkashTokenService>();
var token = await tokenService.GetValidTokenAsync();
Console.WriteLine($"✓ Token obtained (length: {token.Length})");

// Test payment creation
var paymentService = serviceProvider.GetRequiredService<IBkashPaymentService>();
var request = new BkashCreatePaymentRequest
{
    Amount = 100,
    MerchantInvoiceNumber = $"TEST-{DateTime.UtcNow.Ticks}",
    Intent = "sale"
};

var response = await paymentService.CreatePaymentAsync(request);
Console.WriteLine($"✓ Payment created: {response.PaymentID}");

Console.WriteLine("\nAll tests passed!");
```

### Step 6: Run Tests

```bash
dotnet run
```

Expected output:
```
Testing bKash package...
✓ Token obtained (length: XXX)
✓ Payment created: TR001...
All tests passed!
```

## Troubleshooting Local Testing

### Issue: Package Not Found

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Remove and re-add source
dotnet nuget remove source LocalBkash
dotnet nuget add source "/absolute/path/to/package/bin/Release" -n LocalBkash
```

### Issue: Wrong Package Version

```bash
# List available versions
dotnet nuget search Bikiran.Payment.Bkash --source LocalBkash

# Install specific version
dotnet add package Bikiran.Payment.Bkash --version 1.0.0
```

## Unit Testing (TODO)

The project currently lacks comprehensive unit tests. Contributions welcome!

### Suggested Test Structure

```
Bikiran.Payment.Bkash.Tests/
├── Services/
│   ├── BkashPaymentServiceTests.cs
│   └── BkashTokenServiceTests.cs
├── Utilities/
│   └── BkashWebhookHelperTests.cs
└── Configuration/
    └── BkashOptionsTests.cs
```

## Integration Testing

For integration testing with sandbox:

1. Use sandbox credentials
2. Test all payment flows
3. Test refund operations
4. Test webhook signatures
5. Test error scenarios

## Next Steps

- 📖 [Local Development Setup](local-setup.md)
- 📖 [Contributing Guidelines](contributing.md)
- 📖 [Changelog](changelog.md)
