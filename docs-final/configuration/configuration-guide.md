# Configuration Guide

Complete guide to configuring Bikiran.Payment.Bkash for all environments and scenarios.

## Quick Reference

### Environment Variables

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

### JSON Format (appsettings.json)

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

### Sandbox Credentials

Default bKash sandbox credentials for testing:

```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

## Configuration Options

### Required Settings

| Setting       | Description                   | Example                      |
| ------------- | ----------------------------- | ---------------------------- |
| `AppKey`      | Your bKash application key    | `4f6o0cjiki2rfm34kfdadl1eqq` |
| `AppSecret`   | Your bKash application secret | `2is7hdkt...`                |
| `Username`    | Your bKash merchant username  | `sandboxTokenizedUser02`     |
| `Password`    | Your bKash merchant password  | `sandboxToken...@12345`      |
| `Environment` | Target environment            | `Sandbox` or `Production`    |

### Optional Settings

| Setting                     | Description                                    | Default                            |
| --------------------------- | ---------------------------------------------- | ---------------------------------- |
| `TimeoutSeconds`            | HTTP request timeout in seconds                | `30`                               |
| `TokenRefreshBufferSeconds` | Refresh tokens this many seconds before expiry | `300` (5 minutes)                  |
| `BaseUrl`                   | Custom API base URL (rarely needed)            | Auto-selected based on Environment |

## Configuration Methods

Bikiran.Payment.Bkash supports multiple configuration methods to fit different deployment scenarios:

1. **appsettings.json** - Simple and built-in to .NET
2. **Environment Variables** - Recommended for containerized and cloud deployments
3. **.env Files** - Great for local development
4. **Secret Managers** - Recommended for production (Azure Key Vault, AWS Secrets Manager, etc.)

### Method 1: appsettings.json

#### Basic Configuration

**appsettings.json:**

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

**Program.cs:**

```csharp
builder.Services.AddBkashPayment(builder.Configuration);
```

#### Environment-Specific Configuration

**appsettings.Development.json:**

```json
{
  "Bkash": {
    "AppKey": "sandbox-key",
    "AppSecret": "sandbox-secret",
    "Username": "sandbox-user",
    "Password": "sandbox-password",
    "Environment": "Sandbox",
    "TimeoutSeconds": 60
  }
}
```

**appsettings.Production.json:**

```json
{
  "Bkash": {
    "AppKey": "production-key",
    "AppSecret": "production-secret",
    "Username": "production-user",
    "Password": "production-password",
    "Environment": "Production",
    "TimeoutSeconds": 30
  }
}
```

#### Pros and Cons

✅ **Pros:**

- Built into .NET, no additional packages needed
- Easy to manage multiple environments
- Strongly typed with IConfiguration

❌ **Cons:**

- Credentials in files can be accidentally committed
- Not ideal for production deployments
- Requires separate files for each environment

### Method 2: Environment Variables

#### Format

Environment variables use double underscore `__` for nested configuration:

```bash
BKASH__APPKEY=your-app-key
BKASH__APPSECRET=your-app-secret
BKASH__USERNAME=your-username
BKASH__PASSWORD=your-password
BKASH__ENVIRONMENT=Sandbox
```

#### Setting Environment Variables

**Windows (PowerShell):**

```powershell
$env:BKASH__APPKEY = "your-key"
$env:BKASH__APPSECRET = "your-secret"
$env:BKASH__USERNAME = "your-username"
$env:BKASH__PASSWORD = "your-password"
$env:BKASH__ENVIRONMENT = "Sandbox"
```

**Linux/macOS (Bash):**

```bash
export BKASH__APPKEY="your-key"
export BKASH__APPSECRET="your-secret"
export BKASH__USERNAME="your-username"
export BKASH__PASSWORD="your-password"
export BKASH__ENVIRONMENT="Sandbox"
```

#### Using in Code

No code changes needed - .NET automatically loads environment variables:

```csharp
builder.Services.AddBkashPayment(builder.Configuration);
```

#### Pros and Cons

✅ **Pros:**

- No files to manage
- Cloud-friendly and container-ready
- Harder to accidentally commit credentials

❌ **Cons:**

- Can be difficult to manage many variables
- Different methods for each OS

### Method 3: .env Files (Local Development)

#### Setup

1. Install the DotNetEnv package:

```bash
dotnet add package DotNetEnv
```

2. Create a `.env` file in your project root:

```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

3. Load in Program.cs:

```csharp
using DotNetEnv;

// Load .env file at startup
Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBkashPayment(builder.Configuration);
```

4. **Important:** Add `.env` to `.gitignore`:

```gitignore
.env
.env.local
.env.*.local
```

#### Multiple Environment Files

You can maintain separate .env files:

- `.env.example` - Template file (commit this)
- `.env` - Local development (don't commit)
- `.env.sandbox` - Sandbox credentials
- `.env.production` - Production credentials (never commit)

#### Pros and Cons

✅ **Pros:**

- Easy to manage locally
- Can have multiple files for different environments
- Popular pattern in the industry

❌ **Cons:**

- Requires additional package (DotNetEnv)
- Must remember to exclude from version control
- Not automatically available in deployed environments

### Method 4: Secret Managers (Production)

For production deployments, use dedicated secret management services.

#### Azure Key Vault

**Setup:**

```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

**Program.cs:**

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = builder.Configuration["KeyVaultEndpoint"];
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        new DefaultAzureCredential());
}

builder.Services.AddBkashPayment(builder.Configuration);
```

Store secrets in Key Vault as:

- `Bkash--AppKey`
- `Bkash--AppSecret`
- `Bkash--Username`
- `Bkash--Password`

#### AWS Secrets Manager

**Setup:**

```bash
dotnet add package Amazon.Extensions.Configuration.SystemsManager
```

**Program.cs:**

```csharp
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddSecretsManager(
        configurator: options =>
        {
            options.SecretFilter = secret =>
                secret.Name.StartsWith("bkash/");
        });
}
```

## Environment-Specific Configuration

### Sandbox Configuration

For testing and development, use bKash sandbox:

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

**Sandbox URLs:**

- API Base: `https://tokenized.sandbox.bka.sh`
- Payment Page: `https://sandbox.bka.sh/checkout`

### Production Configuration

For live transactions:

```json
{
  "Bkash": {
    "AppKey": "your-production-key",
    "AppSecret": "your-production-secret",
    "Username": "your-production-username",
    "Password": "your-production-password",
    "Environment": "Production"
  }
}
```

**Production URLs:**

- API Base: `https://tokenized.pay.bka.sh`
- Payment Page: `https://pay.bka.sh/checkout`

**⚠️ Important:**

- Never use sandbox credentials in production
- Never commit production credentials to source control
- Use secret managers (Key Vault, Secrets Manager) in production

### Programmatic Configuration

You can also configure programmatically:

```csharp
builder.Services.AddBkashPayment(options =>
{
    options.AppKey = builder.Configuration["CustomAppKey"];
    options.AppSecret = builder.Configuration["CustomAppSecret"];
    options.Username = builder.Configuration["CustomUsername"];
    options.Password = builder.Configuration["CustomPassword"];
    options.Environment = BkashEnvironment.Sandbox;
    options.TimeoutSeconds = 60;
    options.TokenRefreshBufferSeconds = 600; // 10 minutes
});
```

## Configuration Priority

.NET loads configuration in this order (last wins):

1. appsettings.json
2. appsettings.{Environment}.json
3. User secrets (Development only)
4. Environment variables
5. Command-line arguments

This means environment variables will override appsettings.json.

## Verifying Configuration

Add this code to verify your configuration is loaded correctly:

```csharp
using Microsoft.Extensions.Options;

var app = builder.Build();

// Log configuration (without sensitive data)
var config = app.Services.GetRequiredService<IOptions<BkashOptions>>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("bKash Configuration:");
logger.LogInformation("  Environment: {Environment}", config.Value.Environment);
logger.LogInformation("  BaseUrl: {BaseUrl}", config.Value.GetBaseUrl());
logger.LogInformation("  AppKey Length: {Length}", config.Value.AppKey?.Length ?? 0);
logger.LogInformation("  Timeout: {Timeout}s", config.Value.TimeoutSeconds);
```

Test your configuration via health endpoint:

```bash
curl http://localhost:5000/health
```

## Configuration Best Practices

### ✅ DO

1. **Use different credentials for each environment**
2. **Store production credentials in secret managers**
3. **Add `.env` to `.gitignore`**
4. **Rotate credentials regularly (every 90 days)**
5. **Use the lowest timeout that works for your needs**
6. **Test configuration in each environment**
7. **Validate configuration at startup**

### ❌ DON'T

1. **Never commit credentials to source control**
2. **Don't use production credentials locally**
3. **Don't share credentials via email or chat**
4. **Don't log sensitive configuration values**
5. **Don't hardcode credentials in code**

## Common Issues

| Issue                           | Solution                             |
| ------------------------------- | ------------------------------------ |
| Variables not loading           | Check double underscore `__` syntax  |
| Authentication fails            | Verify credentials match environment |
| Wrong environment               | Check `BKASH__ENVIRONMENT` value     |
| Timeout errors                  | Increase `BKASH__TIMEOUTSECONDS`     |
| Configuration section not found | Check appsettings.json structure     |

## Security Checklist

- [ ] Never commit `.env` files with credentials
- [ ] Add `.env` to `.gitignore`
- [ ] Use different credentials per environment
- [ ] Use secret managers in production
- [ ] Rotate credentials regularly
- [ ] Restrict access to production credentials
- [ ] Validate configuration at startup
- [ ] Monitor failed authentication attempts

## Files to Exclude from Git

```gitignore
.env
.env.local
.env.production
.env.*.local
appsettings.Production.json  # if it contains credentials
```

## Next Steps

- 📖 [Environment Setup](environment-setup.md) - Platform-specific deployment setup
- 🚀 [Quick Start Guide](../getting-started/quick-start.md) - Get started with the library
- 🔒 [Security Best Practices](../guides/security-best-practices.md) - Secure your configuration

---

Need help? Check the [Documentation Index](../README.md) or [open an issue](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues).
