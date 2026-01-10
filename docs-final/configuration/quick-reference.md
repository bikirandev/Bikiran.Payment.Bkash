# Configuration Quick Reference

Quick cheat sheet for configuring Bikiran.Payment.Bkash.

## Environment Variable Format

```env
BKASH__APPKEY=your-app-key
BKASH__APPSECRET=your-app-secret
BKASH__USERNAME=your-username
BKASH__PASSWORD=your-password
BKASH__ENVIRONMENT=Sandbox
BKASH__TIMEOUTSECONDS=30
BKASH__TOKENREFRESHBUFFERSECONDS=300
```

⚠️ **Note:** Use double underscore `__` for nested configuration

## JSON Format (appsettings.json)

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

Default bKash sandbox credentials for testing:

```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

## Quick Setup by Platform

| Platform | Command |
|----------|---------|
| **Local (.env)** | `cp .env.example .env` then edit |
| **Azure** | `az webapp config appsettings set --settings KEY=value` |
| **AWS** | `eb setenv KEY=value` |
| **Docker** | `docker run -e KEY=value` |
| **Kubernetes** | `kubectl create secret generic bkash-credentials` |
| **Heroku** | `heroku config:set KEY=value` |

## Environment Values

| Value | Base URL | Description |
|-------|----------|-------------|
| `Sandbox` | https://tokenized.sandbox.bka.sh | Testing environment |
| `Production` | https://tokenized.pay.bka.sh | Live environment |

## Required vs Optional Settings

### ✅ Required
- `AppKey`
- `AppSecret`
- `Username`
- `Password`
- `Environment`

### ⚙️ Optional (with defaults)
- `TimeoutSeconds` (default: 30)
- `TokenRefreshBufferSeconds` (default: 300)
- `BaseUrl` (auto-selected based on Environment)

## Quick Commands

### PowerShell (Windows)
```powershell
$env:BKASH__APPKEY = "your-key"
$env:BKASH__APPSECRET = "your-secret"
dotnet run
```

### Bash (Linux/macOS)
```bash
export BKASH__APPKEY="your-key"
export BKASH__APPSECRET="your-secret"
dotnet run
```

### Using .env File
```bash
cp .env.example .env
nano .env  # Edit with your credentials
dotnet run
```

## Configuration Priority

From lowest to highest (last wins):
1. appsettings.json
2. appsettings.{Environment}.json
3. User secrets (Development only)
4. Environment variables
5. Command-line arguments

## Verification

Test your configuration:

```csharp
var config = app.Services.GetRequiredService<IOptions<BkashOptions>>();
Console.WriteLine($"Environment: {config.Value.Environment}");
Console.WriteLine($"BaseUrl: {config.Value.GetBaseUrl()}");
```

Check health endpoint:
```bash
curl http://localhost:5000/health
```

## Common Issues

| Issue | Solution |
|-------|----------|
| Variables not loading | Check double underscore `__` syntax |
| Authentication fails | Verify credentials match environment |
| Wrong environment | Check `BKASH__ENVIRONMENT` value |
| Timeout errors | Increase `BKASH__TIMEOUTSECONDS` |

## Security Checklist

- [ ] Never commit `.env` files with credentials
- [ ] Add `.env` to `.gitignore`
- [ ] Use different credentials per environment
- [ ] Use secret managers in production
- [ ] Rotate credentials regularly
- [ ] Restrict access to production credentials

## Files to Exclude from Git

```gitignore
.env
.env.local
.env.production
.env.*.local
appsettings.Production.json
```

## Getting Credentials

### Sandbox
Contact bKash for sandbox merchant account:
- Website: https://www.bka.sh/
- Email: merchant.service@bka.sh

### Production
1. Visit https://www.bka.sh/
2. Apply for merchant account
3. Complete KYC process
4. Receive production credentials

## Next Steps

- 📖 [Configuration Overview](overview.md) - Detailed configuration guide
- 🌐 [Environment Setup](environment-setup.md) - Platform-specific setup
- 🚀 [Quick Start](../getting-started/quick-start.md) - Get started quickly

---

**Need more details?** See the [full configuration guide](overview.md).
