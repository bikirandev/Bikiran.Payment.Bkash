# Quick Reference

Quick reference card for Bikiran.Payment.Bkash configuration and common operations.

## Environment Variables Format

```env
BKASH__APPKEY=your-app-key
BKASH__APPSECRET=your-app-secret
BKASH__USERNAME=your-username
BKASH__PASSWORD=your-password
BKASH__ENVIRONMENT=Sandbox
BKASH__TIMEOUTSECONDS=30
BKASH__TOKENREFRESHBUFFERSECONDS=300
```

⚠️ **Note**: Use double underscore `__` to represent nested configuration

## JSON Configuration Format

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

## Sandbox Credentials

```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

## Quick Setup Commands

### Local Development (.env file)

```bash
# 1. Copy template
cp .env.example .env

# 2. Edit credentials
nano .env

# 3. Run application
dotnet run
```

### PowerShell

```powershell
$env:BKASH__APPKEY = "your-key"
$env:BKASH__APPSECRET = "your-secret"
$env:BKASH__USERNAME = "your-username"
$env:BKASH__PASSWORD = "your-password"
$env:BKASH__ENVIRONMENT = "Sandbox"
dotnet run
```

### Linux/macOS

```bash
export BKASH__APPKEY="your-key"
export BKASH__APPSECRET="your-secret"
export BKASH__USERNAME="your-username"
export BKASH__PASSWORD="your-password"
export BKASH__ENVIRONMENT="Sandbox"
dotnet run
```

## Platform Configuration

| Platform | Command/Method |
|----------|----------------|
| **Azure App Service** | Portal → Configuration → Application Settings |
| **Azure CLI** | `az webapp config appsettings set --settings KEY=value` |
| **AWS EB** | `eb setenv KEY=value` |
| **Docker** | `docker run -e KEY=value` |
| **Kubernetes** | `kubectl create secret generic bkash-credentials` |
| **Heroku** | `heroku config:set KEY=value` |

## Environment Values

| Value | Description | API Base URL |
|-------|-------------|--------------|
| `Sandbox` | Testing environment | https://tokenized.sandbox.bka.sh |
| `Production` | Live environment | https://tokenized.pay.bka.sh |

## Required Configuration

✅ **Always Required:**
- `AppKey`
- `AppSecret`
- `Username`
- `Password`
- `Environment`

⚙️ **Optional (with defaults):**
- `TimeoutSeconds` (default: 30)
- `TokenRefreshBufferSeconds` (default: 300)
- `BaseUrl` (default: auto-selected)

## Common Code Snippets

### Setup in Program.cs

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();
app.MapHealthChecks("/health");
app.Run();
```

### Create Payment

```csharp
var request = new BkashCreatePaymentRequest
{
    Amount = 100,
    MerchantInvoiceNumber = "INV-123",
    Intent = "sale"
};
var response = await _bkashService.CreatePaymentAsync(request);
// Redirect user to: response.BkashURL
```

### Execute Payment

```csharp
var response = await _bkashService.ExecutePaymentAsync(paymentId);
if (response.IsCompleted)
{
    // Payment successful: response.TrxID
}
```

### Query Payment

```csharp
var response = await _bkashService.QueryPaymentAsync(paymentId);
// Check: response.TransactionStatus
```

### Process Refund

```csharp
var request = new BkashRefundPaymentRequest
{
    PaymentId = "paymentId",
    TrxId = "transactionId",
    RefundAmount = 50,
    Sku = "order-123",
    Reason = "Customer request"
};
var response = await _bkashService.RefundPaymentAsync(request);
```

### Verify Webhook

```csharp
using Bikiran.Payment.Bkash.Utilities;

var isValid = BkashWebhookHelper.VerifyWebhookSignature(
    payload, 
    signature, 
    appSecret
);

if (!isValid)
{
    return Unauthorized("Invalid signature");
}
```

## Payment Modes

| Mode | Description |
|------|-------------|
| `0001` | Checkout (one-time payment) |
| `0011` | Agreement (subscription) |
| `0002` | Pre-Authorization |
| `0021` | Disbursement |
| `0031` | Refund |

## Exception Types

```csharp
try
{
    var response = await _bkashService.CreatePaymentAsync(request);
}
catch (BkashAuthenticationException) { /* Auth failed */ }
catch (BkashPaymentException ex) { /* Payment error: ex.ErrorCode */ }
catch (BkashConfigurationException) { /* Config issue */ }
catch (BkashException) { /* General error */ }
```

## Health Check

```bash
# Test endpoint
curl http://localhost:5000/health
```

Expected response:
```json
{
  "status": "Healthy",
  "checks": [
    {"name": "bkash", "status": "Healthy"}
  ]
}
```

## Security Checklist

- [ ] Never commit `.env` files with credentials
- [ ] Add `.env` to `.gitignore`
- [ ] Use different credentials per environment
- [ ] Use secret managers in production (Key Vault, Secrets Manager)
- [ ] Rotate credentials regularly
- [ ] Don't log credentials
- [ ] Verify webhook signatures
- [ ] Use HTTPS only

## Files to Keep/Exclude

### ✅ Commit to Git

- `.env.example`
- `appsettings.json` (without credentials)
- `appsettings.Development.json` (sandbox credentials OK)

### ❌ Never Commit

- `.env`
- `.env.local`
- `.env.production`
- `appsettings.Production.json` (with real credentials)

## Common Issues

| Issue | Solution |
|-------|----------|
| Variables not loading | Check double underscore `__` syntax |
| Authentication fails | Verify credentials match environment |
| Wrong environment | Check `BKASH__ENVIRONMENT` value |
| Timeout errors | Increase `BKASH__TIMEOUTSECONDS` |

## API Method Reference

### IBkashPaymentService

```csharp
CreatePaymentAsync(BkashCreatePaymentRequest)
ExecutePaymentAsync(string paymentId)
QueryPaymentAsync(string paymentId)
RefundPaymentAsync(BkashRefundPaymentRequest)
QueryRefundStatusAsync(string paymentId, string trxId)
```

### IBkashTokenService

```csharp
GetValidTokenAsync()              // Get valid token (auto-refreshes)
GrantTokenAsync()                 // Request new token
RefreshTokenAsync(string token)   // Refresh existing token
```

## Configuration Priority

Highest to lowest:
1. Command line arguments
2. Environment variables
3. User secrets (development)
4. `appsettings.{Environment}.json`
5. `appsettings.json`

## Testing Configuration

```csharp
// In Program.cs - before app.Run()
var config = app.Services.GetRequiredService<IOptions<BkashOptions>>();
Console.WriteLine($"Environment: {config.Value.Environment}");
Console.WriteLine($"BaseUrl: {config.Value.GetBaseUrl()}");
```

## Getting Credentials

### Sandbox
- Website: https://www.bka.sh/
- Email: merchant.service@bka.sh

### Production
1. Visit https://www.bka.sh/
2. Apply for merchant account
3. Complete KYC process
4. Receive production credentials

## Support

- **Documentation**: See [docs-final](.) folder
- **Issues**: https://github.com/bikirandev/Bikiran.Payment.Bkash/issues
- **bKash Support**: merchant.service@bka.sh

---

**Package Version**: 1.0.0  
**Last Updated**: January 2025
