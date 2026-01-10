# Configuration Guide

This guide covers all methods to configure Bikiran.Payment.Bkash across different platforms and environments.

## Table of Contents

- [Configuration Options](#configuration-options)
- [Local Development](#local-development)
- [Platform-Specific Setup](#platform-specific-setup)
- [Security Best Practices](#security-best-practices)
- [Troubleshooting](#troubleshooting)

## Configuration Options

### Required Settings

All configuration methods must include these required settings:

| Setting | Description | Example |
|---------|-------------|---------|
| `AppKey` | bKash application key | `4f6o0cjiki2rfm34kfdadl1eqq` |
| `AppSecret` | bKash application secret | `2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b` |
| `Username` | bKash merchant username | `sandboxTokenizedUser02` |
| `Password` | bKash merchant password | `sandboxTokenizedUser02@12345` |
| `Environment` | Target environment | `Sandbox` or `Production` |

### Optional Settings

| Setting | Default | Description |
|---------|---------|-------------|
| `TimeoutSeconds` | 30 | HTTP request timeout in seconds |
| `TokenRefreshBufferSeconds` | 300 | Time before token expiry to trigger refresh |
| `BaseUrl` | Auto | Custom base URL (auto-selected based on Environment) |

## Local Development

### Method 1: appsettings.json (Simple Projects)

Create environment-specific configuration files:

**appsettings.Development.json**
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

**appsettings.Production.json**
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

⚠️ **Security Warning**: Never commit `appsettings.Production.json` to source control!

### Method 2: Environment Variables

**Format**: Use double underscore `__` to represent nested configuration.

```env
BKASH__APPKEY=your-app-key
BKASH__APPSECRET=your-app-secret
BKASH__USERNAME=your-username
BKASH__PASSWORD=your-password
BKASH__ENVIRONMENT=Sandbox
BKASH__TIMEOUTSECONDS=30
BKASH__TOKENREFRESHBUFFERSECONDS=300
```

**Windows (PowerShell)**
```powershell
$env:BKASH__APPKEY = "your-key"
$env:BKASH__APPSECRET = "your-secret"
$env:BKASH__USERNAME = "your-username"
$env:BKASH__PASSWORD = "your-password"
$env:BKASH__ENVIRONMENT = "Sandbox"
```

**Linux/macOS**
```bash
export BKASH__APPKEY="your-key"
export BKASH__APPSECRET="your-secret"
export BKASH__USERNAME="your-username"
export BKASH__PASSWORD="your-password"
export BKASH__ENVIRONMENT="Sandbox"
```

### Method 3: Using .env Files (Recommended)

1. **Install DotNetEnv package**:
```bash
dotnet add package DotNetEnv
```

2. **Create `.env` file** in project root:
```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

3. **Load in Program.cs**:
```csharp
using DotNetEnv;

// Load .env file at startup
Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBkashPayment(builder.Configuration);
```

4. **Add to .gitignore**:
```gitignore
.env
.env.local
.env.production
.env.*.local
```

### Method 4: Visual Studio launchSettings.json

**Properties/launchSettings.json**
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

## Platform-Specific Setup

### Azure App Service

**Option 1: Azure Portal**
1. Navigate to your App Service → Configuration → Application settings
2. Add new settings:
   - `BKASH__APPKEY` = your-key
   - `BKASH__APPSECRET` = your-secret
   - `BKASH__USERNAME` = your-username
   - `BKASH__PASSWORD` = your-password
   - `BKASH__ENVIRONMENT` = Production

**Option 2: Azure CLI**
```bash
az webapp config appsettings set \
  --name your-app-name \
  --resource-group your-resource-group \
  --settings \
    BKASH__APPKEY="your-key" \
    BKASH__APPSECRET="your-secret" \
    BKASH__USERNAME="your-username" \
    BKASH__PASSWORD="your-password" \
    BKASH__ENVIRONMENT="Production"
```

**Option 3: Azure Key Vault (Recommended for Production)**
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

Store in Key Vault as:
- `Bkash--AppKey`
- `Bkash--AppSecret`
- `Bkash--Username`
- `Bkash--Password`

### AWS Elastic Beanstalk

**Option 1: EB CLI**
```bash
eb setenv \
  BKASH__APPKEY=your-key \
  BKASH__APPSECRET=your-secret \
  BKASH__USERNAME=your-username \
  BKASH__PASSWORD=your-password \
  BKASH__ENVIRONMENT=Production
```

**Option 2: .ebextensions Configuration**

Create `.ebextensions/bkash.config`:
```yaml
option_settings:
  aws:elasticbeanstalk:application:environment:
    BKASH__APPKEY: "your-key"
    BKASH__APPSECRET: "your-secret"
    BKASH__USERNAME: "your-username"
    BKASH__PASSWORD: "your-password"
    BKASH__ENVIRONMENT: "Production"
```

### Docker

**Dockerfile**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["YourApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV BKASH__ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

**docker-compose.yml**
```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - BKASH__APPKEY=${BKASH_APPKEY}
      - BKASH__APPSECRET=${BKASH_APPSECRET}
      - BKASH__USERNAME=${BKASH_USERNAME}
      - BKASH__PASSWORD=${BKASH_PASSWORD}
      - BKASH__ENVIRONMENT=Production
    env_file:
      - .env.production
```

**Run with environment variables**:
```bash
docker run \
  -e BKASH__APPKEY=your-key \
  -e BKASH__APPSECRET=your-secret \
  -e BKASH__USERNAME=your-username \
  -e BKASH__PASSWORD=your-password \
  -e BKASH__ENVIRONMENT=Production \
  your-image
```

### Kubernetes

**Create Secret**:
```bash
kubectl create secret generic bkash-credentials \
  --from-literal=appkey='your-app-key' \
  --from-literal=appsecret='your-app-secret' \
  --from-literal=username='your-username' \
  --from-literal=password='your-password' \
  --namespace=your-namespace
```

**Deployment YAML**:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: bkash-api
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: api
        image: your-registry/bkash-api:latest
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

### Heroku

```bash
heroku config:set BKASH__APPKEY=your-key
heroku config:set BKASH__APPSECRET=your-secret
heroku config:set BKASH__USERNAME=your-username
heroku config:set BKASH__PASSWORD=your-password
heroku config:set BKASH__ENVIRONMENT=Production

# View current config
heroku config
```

## Security Best Practices

### ✅ DO

1. **Use Secret Management Services in Production**
   - Azure Key Vault
   - AWS Secrets Manager
   - HashiCorp Vault
   - Google Cloud Secret Manager

2. **Rotate Credentials Regularly**
   - Establish a rotation schedule (e.g., every 90 days)
   - Update credentials in all environments simultaneously
   - Test after rotation

3. **Use Different Credentials Per Environment**
   - Sandbox credentials for development/staging
   - Production credentials only in production
   - Separate credentials for each team/service if possible

4. **Restrict Access**
   - Limit who can view production credentials
   - Use role-based access control (RBAC)
   - Audit access logs regularly

### ❌ DON'T

1. **Never Commit Credentials to Source Control**
   ```gitignore
   # Always in .gitignore
   .env
   .env.local
   .env.*.local
   appsettings.Production.json
   ```

2. **Don't Use Production Credentials Locally**
   - Always use sandbox for development
   - Never test with production API in dev environment

3. **Don't Share Credentials via Email/Slack**
   - Use secure secret sharing tools
   - Use temporary access links that expire

4. **Don't Log Credentials**
   ```csharp
   // BAD
   _logger.LogInformation($"Using AppKey: {options.AppKey}");
   
   // GOOD
   _logger.LogInformation("bKash configured for {Environment}", options.Environment);
   ```

5. **Don't Hardcode Credentials**
   ```csharp
   // NEVER DO THIS
   options.AppKey = "hardcoded-key";
   ```

## Troubleshooting

### Environment Variables Not Loading

**Check Configuration Priority** (highest to lowest):
1. Command line arguments
2. Environment variables
3. User secrets (development only)
4. appsettings.{Environment}.json
5. appsettings.json

**Verify Syntax**:
- ✅ Correct: `BKASH__APPKEY` (double underscore)
- ❌ Wrong: `BKASH_APPKEY` (single underscore)

**Test Configuration**:
```csharp
var config = builder.Configuration
    .GetSection("Bkash")
    .Get<BkashOptions>();

Console.WriteLine($"AppKey: {config?.AppKey?.Substring(0, 5)}...");
Console.WriteLine($"Environment: {config?.Environment}");
```

### Authentication Fails

**Checklist**:
- [ ] Verify credentials match the environment (Sandbox vs Production)
- [ ] Check that `BKASH__ENVIRONMENT` is set correctly
- [ ] Ensure no typos in credentials
- [ ] Verify credentials are not expired
- [ ] Test with sandbox credentials first

### Credentials Working Locally But Not in Production

**Check**:
1. Environment variables are set in production environment
2. Application has permission to read secrets (if using secret manager)
3. Environment name is correct (case-sensitive)
4. No typos in variable names
5. Configuration provider is properly configured

## Configuration Examples

### Complete Example with Validation

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

// Add bKash services with validation
builder.Services.AddBkashPayment(options =>
{
    builder.Configuration.GetSection("Bkash").Bind(options);
    
    // Log configuration (without sensitive data)
    Console.WriteLine($"bKash Configuration:");
    Console.WriteLine($"  Environment: {options.Environment}");
    Console.WriteLine($"  AppKey Length: {options.AppKey?.Length ?? 0}");
    Console.WriteLine($"  BaseUrl: {options.GetBaseUrl()}");
    
    // Validate
    try
    {
        options.Validate();
        Console.WriteLine("  ✅ Configuration valid");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ❌ Configuration invalid: {ex.Message}");
        throw;
    }
});

var app = builder.Build();
app.Run();
```

## Getting Production Credentials

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

## Support

- **Detailed Guide**: See this complete configuration guide
- **Quick Reference**: See [04-quick-reference.md](04-quick-reference.md)
- **GitHub Issues**: https://github.com/bikirandev/Bikiran.Payment.Bkash/issues
- **bKash Support**: merchant.service@bka.sh
