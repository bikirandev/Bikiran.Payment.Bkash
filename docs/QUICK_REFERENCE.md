# ?? bKash Configuration Quick Reference

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

?? **Note**: Use double underscore `__` to represent nested configuration

---

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

---

## Default bKash Sandbox Credentials

```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

---

## Quick Setup Commands

### Using .env File

```bash
# 1. Copy template
cp .env.example .env

# 2. Edit credentials
nano .env  # or use your preferred editor

# 3. Run application
dotnet run
```

### Using PowerShell

```powershell
$env:BKASH__APPKEY = "your-key"
$env:BKASH__APPSECRET = "your-secret"
$env:BKASH__USERNAME = "your-username"
$env:BKASH__PASSWORD = "your-password"
$env:BKASH__ENVIRONMENT = "Sandbox"
dotnet run
```

### Using Bash/Linux

```bash
export BKASH__APPKEY="your-key"
export BKASH__APPSECRET="your-secret"
export BKASH__USERNAME="your-username"
export BKASH__PASSWORD="your-password"
export BKASH__ENVIRONMENT="Sandbox"
dotnet run
```

---

## Configuration by Platform

| Platform | Command/Location |
|----------|------------------|
| **Local Dev** | `.env` file in project root |
| **Azure App Service** | Portal ? Configuration ? Application Settings |
| **AWS Elastic Beanstalk** | `eb setenv KEY=value` |
| **Docker** | `docker run -e KEY=value` |
| **Kubernetes** | `kubectl create secret generic bkash-credentials` |
| **Heroku** | `heroku config:set KEY=value` |

---

## Environment Values

| Value | Description | URL |
|-------|-------------|-----|
| `Sandbox` | Testing environment | https://tokenized.sandbox.bka.sh |
| `Production` | Live environment | https://tokenized.pay.bka.sh |

---

## Required Fields

? **Always Required:**
- `AppKey`
- `AppSecret`
- `Username`
- `Password`
- `Environment`

?? **Optional (with defaults):**
- `TimeoutSeconds` (default: 30)
- `TokenRefreshBufferSeconds` (default: 300)
- `BaseUrl` (default: auto-selected based on Environment)

---

## Security Checklist

- [ ] Never commit `.env` files with credentials
- [ ] Add `.env` to `.gitignore`
- [ ] Use different credentials for each environment
- [ ] Use secret management in production (Key Vault, Secrets Manager)
- [ ] Rotate credentials regularly
- [ ] Restrict access to production credentials

---

## Verification

### Test Configuration

```csharp
// In Program.cs - Add before app.Run()
var config = app.Services.GetRequiredService<IOptions<BkashOptions>>();
Console.WriteLine($"Environment: {config.Value.Environment}");
Console.WriteLine($"BaseUrl: {config.Value.GetBaseUrl()}");
```

### Test Health Check

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
      "status": "Healthy"
    }
  ]
}
```

---

## Common Issues

| Issue | Solution |
|-------|----------|
| Variables not loading | Check double underscore `__` syntax |
| Authentication fails | Verify credentials are correct for environment |
| Wrong environment | Check `BKASH__ENVIRONMENT` value |
| Timeout errors | Increase `BKASH__TIMEOUTSECONDS` |

---

## Files to Keep/Exclude

### ? Commit to Git

- `.env.example`
- `.env.sandbox` (template only)
- `.env.production` (template only)
- `appsettings.json` (without credentials)

### ? Never Commit

- `.env`
- `.env.local`
- `.env.production` (with actual credentials)
- `appsettings.Production.json` (with credentials)

---

## Production Checklist

- [ ] Environment set to `Production`
- [ ] Using production credentials from bKash
- [ ] Credentials stored in secret manager (not .env)
- [ ] Health checks configured
- [ ] Monitoring and alerting set up
- [ ] Credentials rotation plan in place
- [ ] Disaster recovery plan documented

---

## Getting Credentials

### Sandbox
Contact bKash for sandbox merchant account:
- Website: https://www.bka.sh/
- Email: merchant.service@bka.sh

### Production
Complete merchant onboarding:
1. Visit https://www.bka.sh/
2. Apply for merchant account
3. Complete KYC process
4. Receive production credentials

---

## Support

- **Documentation**: See ENVIRONMENT_SETUP.md for detailed guide
- **Issues**: https://github.com/bikiran/bkash-dotnet/issues
- **bKash Support**: merchant.service@bka.sh

---

**Last Updated**: January 2025  
**Package Version**: 1.0.0
