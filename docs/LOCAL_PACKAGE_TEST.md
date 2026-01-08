# Local Package Testing Guide

## Quick Test of Bikiran.Payment.Bkash.1.0.0.nupkg

### Step 1: Create a Test Console Application

```powershell
# Navigate to a test directory
cd D:\
mkdir BkashPackageTest
cd BkashPackageTest

# Create new console app
dotnet new console -n BkashTestApp
cd BkashTestApp
```

### Step 2: Add Local Package Source

```powershell
# First, check if LocalBkash source already exists
dotnet nuget list source

# If LocalBkash exists, remove it first
dotnet nuget remove source LocalBkash

# Add the local package source with absolute path
dotnet nuget add source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" -n LocalBkash

# Verify the source was added correctly
dotnet nuget list source
```

**Important**: The package source path must be an absolute path and must exist before adding the package.

### Step 3: Add the Package

```powershell
# Add the package using the source name
dotnet add package Bikiran.Payment.Bkash --version 1.0.0 --source LocalBkash

# Alternative: Specify all sources (recommended if you have other dependencies)
dotnet add package Bikiran.Payment.Bkash --version 1.0.0
```

### Step 4: Create Test Code

Replace the contents of `Program.cs` with:

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Configuration;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Configure bKash (use sandbox credentials)
services.AddBkashPayment(options =>
{
    options.AppKey = "4f6o0cjiki2rfm34kfdadl1eqq";
    options.AppSecret = "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b";
    options.Username = "sandboxTokenizedUser02";
    options.Password = "sandboxTokenizedUser02@12345";
    options.Environment = BkashEnvironment.Sandbox;
});

// Add health checks
services.AddHealthChecks()
    .AddBkashHealthCheck("bkash");

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("=== Bikiran.Payment.Bkash Package Test ===\n");

// Test 1: Token Service
Console.WriteLine("Test 1: Getting bKash Token...");
var tokenService = serviceProvider.GetRequiredService<IBkashTokenService>();
try
{
    var token = await tokenService.GetValidTokenAsync();
    Console.WriteLine($"? Token obtained successfully (length: {token.Length})\n");
}
catch (Exception ex)
{
    Console.WriteLine($"? Token test failed: {ex.Message}\n");
}

// Test 2: Create Payment
Console.WriteLine("Test 2: Creating Payment...");
var paymentService = serviceProvider.GetRequiredService<IBkashPaymentService>();
try
{
    var createRequest = new BkashCreatePaymentRequest
    {
        Amount = 100,
        MerchantInvoiceNumber = $"TEST-{DateTime.UtcNow.Ticks}",
        Intent = "sale"
    };

    var createResponse = await paymentService.CreatePaymentAsync(createRequest);
    Console.WriteLine($"? Payment created successfully");
    Console.WriteLine($"   Payment ID: {createResponse.PaymentID}");
    Console.WriteLine($"   bKash URL: {createResponse.BkashURL}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"? Payment creation failed: {ex.Message}\n");
}

// Test 3: Health Check
Console.WriteLine("Test 3: Health Check...");
var healthCheckService = serviceProvider.GetRequiredService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>();
try
{
    var healthResult = await healthCheckService.CheckHealthAsync();
    Console.WriteLine($"? Health check status: {healthResult.Status}");
    foreach (var entry in healthResult.Entries)
    {
        Console.WriteLine($"   {entry.Key}: {entry.Value.Status}");
    }
    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"? Health check failed: {ex.Message}\n");
}

// Test 4: Webhook Helper
Console.WriteLine("Test 4: Webhook Signature Verification...");
try
{
    var testPayload = "{\"paymentID\":\"test123\"}";
    var testSecret = "test-secret";
    var signature = Bikiran.Payment.Bkash.Utilities.BkashWebhookHelper.ComputeSignature(testPayload, testSecret);
    var isValid = Bikiran.Payment.Bkash.Utilities.BkashWebhookHelper.VerifyWebhookSignature(testPayload, signature, testSecret);
    
    Console.WriteLine($"? Webhook signature verification: {(isValid ? "PASSED" : "FAILED")}");
    Console.WriteLine($"   Generated signature: {signature.Substring(0, 20)}...\n");
}
catch (Exception ex)
{
    Console.WriteLine($"? Webhook test failed: {ex.Message}\n");
}

Console.WriteLine("=== All Tests Completed ===");
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
```

### Step 5: Run the Test

```powershell
dotnet run
```

### Expected Output

You should see something like:

```
=== Bikiran.Payment.Bkash Package Test ===

Test 1: Getting bKash Token...
? Token obtained successfully (length: XXX)

Test 2: Creating Payment...
? Payment created successfully
   Payment ID: TR0011abc123...
   bKash URL: https://tokenized.sandbox.bka.sh/...

Test 3: Health Check...
? Health check status: Healthy
   bkash: Healthy

Test 4: Webhook Signature Verification...
? Webhook signature verification: PASSED
   Generated signature: abc123...

=== All Tests Completed ===
```

---

## Alternative: Test with ASP.NET Core Web API

### Create Web API Test Project

```powershell
cd D:\BkashPackageTest
dotnet new webapi -n BkashWebApiTest
cd BkashWebApiTest

# Add local package source
dotnet nuget add source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" -n LocalBkash

# Add package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0 --source LocalBkash
```

### Update Program.cs

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add bKash services
builder.Services.AddBkashPayment(options =>
{
    options.AppKey = "4f6o0cjiki2rfm34kfdadl1eqq";
    options.AppSecret = "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b";
    options.Username = "sandboxTokenizedUser02";
    options.Password = "sandboxTokenizedUser02@12345";
    options.Environment = BkashEnvironment.Sandbox;
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddBkashHealthCheck("bkash", "payment", "external");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
```

### Run and Test

```powershell
dotnet run
```

Then navigate to:
- Swagger UI: https://localhost:5001/swagger
- Health Check: https://localhost:5001/health

---

## Verification Checklist

After running the tests, verify:

- [ ] Package installs without errors
- [ ] Token service obtains valid token
- [ ] Payment creation works (gets payment URL)
- [ ] Health check returns Healthy status
- [ ] Webhook helper functions work
- [ ] No runtime exceptions
- [ ] IntelliSense shows proper documentation
- [ ] All required dependencies are included

---

## Troubleshooting

### Issue: Package not found or NU1301 error

**Error**: `error: NU1301: The local source 'D:\...\LocalBkash' doesn't exist.`

**Cause**: The package source path is being resolved relative to the project directory instead of using the absolute path.

**Solution 1 - Remove and Re-add Source Globally**:
```powershell
# Remove the problematic source
dotnet nuget remove source LocalBkash

# Add it back with absolute path
dotnet nuget add source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" -n LocalBkash

# Verify it was added correctly
dotnet nuget list source

# Try adding the package again
dotnet add package Bikiran.Payment.Bkash --version 1.0.0
```

**Solution 2 - Use nuget.config in Project Directory**:
```powershell
# Create a nuget.config in your test project directory
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="LocalBkash" value="D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath nuget.config -Encoding utf8

# Now add the package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0
```

**Solution 3 - Direct Package Reference**:
```powershell
# Edit the .csproj file directly and add:
```xml
<ItemGroup>
  <PackageReference Include="Bikiran.Payment.Bkash" Version="1.0.0">
    <IncludeAssets>all</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

# Then restore with explicit source
dotnet restore --source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" --source https://api.nuget.org/v3/index.json
```

**Solution 4 - Use Local Folder Path Directly**:
```powershell
# Install directly from the .nupkg file
dotnet add package Bikiran.Payment.Bkash --version 1.0.0 --source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release"
```

### Issue: Source name already exists

**Solution**: Remove the existing source first:
```powershell
dotnet nuget remove source LocalBkash
dotnet nuget add source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" -n LocalBkash
```

### Issue: Missing dependencies

**Solution**: Clear NuGet cache and restore:
```powershell
dotnet nuget locals all --clear
dotnet restore --source LocalBkash --source https://api.nuget.org/v3/index.json
```

### Issue: Authentication fails

**Solution**: Verify you're using sandbox credentials and sandbox environment:
```csharp
options.Environment = BkashEnvironment.Sandbox;
```

---

## Next Steps

If all tests pass:
1. ? Package is working correctly
2. ? Ready to test with real sandbox credentials
3. ? Can proceed to publish on NuGet.org

If any tests fail:
1. Check error messages
2. Verify package contents
3. Check dependencies
4. Review build output for warnings

---

## Clean Up

After testing, you can remove the test projects:

```powershell
cd D:\
Remove-Item -Recurse -Force BkashPackageTest
```

And remove the local package source if needed:

```powershell
dotnet nuget remove source LocalBkash
