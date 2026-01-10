# Environment Configuration Guide

This guide explains how to configure Bikiran.Payment.Bkash using environment variables across different platforms and scenarios.

## Table of Contents
- [Quick Start](#quick-start)
- [Configuration Methods](#configuration-methods)
- [Platform-Specific Setup](#platform-specific-setup)
- [Security Best Practices](#security-best-practices)
- [Troubleshooting](#troubleshooting)

---

## Quick Start

### 1. Copy Environment Template

```bash
# Copy the example file
cp .env.example .env

# Edit with your credentials
nano .env
```

### 2. Update Your Credentials

```env
BKASH__APPKEY=your-app-key-here
BKASH__APPSECRET=your-app-secret-here
BKASH__USERNAME=your-username-here
BKASH__PASSWORD=your-password-here
BKASH__ENVIRONMENT=Sandbox
```

### 3. Add to .gitignore

```gitignore
# Never commit these files
.env
.env.local
.env.production
.env.*.local
```

---

## Configuration Methods

### Method 1: Using .env File (Local Development)

**Install DotNetEnv:**
```bash
dotnet add package DotNetEnv
```

**Program.cs:**
```csharp
using DotNetEnv;
using Bikiran.Payment.Bkash;

// Load .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBkashPayment(builder.Configuration);

var app = builder.Build();
app.Run();
```

**Advantages:**
- ? Easy to manage locally
- ? Different files for different environments
- ? Easy to share templates

**Disadvantages:**
- ?? Must not be committed to source control
- ?? Need to manage file distribution

---

### Method 2: appsettings.json (Simple Projects)

**appsettings.Development.json:**
```json
{
  "Bkash": {
    "AppKey": "sandbox-app-key",
    "AppSecret": "sandbox-app-secret",
    "Username": "sandbox-username",
    "Password": "sandbox-password",
    "Environment": "Sandbox"
  }
}
```

**appsettings.Production.json:**
```json
{
  "Bkash": {
    "AppKey": "production-app-key",
    "AppSecret": "production-app-secret",
    "Username": "production-username",
    "Password": "production-password",
    "Environment": "Production"
  }
}
```

**Advantages:**
- ? Built into .NET
- ? Environment-specific files
- ? No additional packages

**Disadvantages:**
- ?? Credentials in files can be accidentally committed
- ?? Less secure for production

---

### Method 3: launchSettings.json (Development Only)

**Properties/launchSettings.json:**
```json
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BKASH__APPKEY": "sandbox-key",
        "BKASH__APPSECRET": "sandbox-secret",
        "BKASH__USERNAME": "sandbox-user",
        "BKASH__PASSWORD": "sandbox-pass",
        "BKASH__ENVIRONMENT": "Sandbox"
      }
    },
    "IIS Express": {
      "commandName": "IISExpress",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BKASH__APPKEY": "sandbox-key",
        "BKASH__APPSECRET": "sandbox-secret"
      }
    }
  }
}
```

**Advantages:**
- ? Perfect for Visual Studio development
- ? Easy to switch profiles
- ? No runtime dependencies

**Disadvantages:**
- ?? Development only
- ?? VS/Rider specific

---

### Method 4: System Environment Variables

**Windows (PowerShell):**
```powershell
# Set for current session
$env:BKASH__APPKEY = "your-key"
$env:BKASH__APPSECRET = "your-secret"

# Set permanently (requires admin)
[System.Environment]::SetEnvironmentVariable('BKASH__APPKEY', 'your-key', 'Machine')
```

**Windows (CMD):**
```cmd
set BKASH__APPKEY=your-key
set BKASH__APPSECRET=your-secret
```

**Linux/macOS:**
```bash
# Set for current session
export BKASH__APPKEY="your-key"
export BKASH__APPSECRET="your-secret"

# Add to ~/.bashrc or ~/.zshrc for persistence
echo 'export BKASH__APPKEY="your-key"' >> ~/.bashrc
```

**Advantages:**
- ? No files to manage
- ? Secure (not in source control)
- ? Works everywhere

**Disadvantages:**
- ?? Harder to manage multiple environments
- ?? Can be forgotten/lost

---

## Platform-Specific Setup

### Azure App Service

**Option 1: Azure Portal**
1. Go to your App Service
2. Navigate to Configuration > Application settings
3. Add new settings:
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

**Option 3: Azure Key Vault (Recommended)**

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

---

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

**Option 3: AWS Secrets Manager (Recommended)**

```csharp
// Install: Amazon.Extensions.Configuration.SystemsManager
var builder = WebApplication.CreateBuilder(args);

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

---

### Docker & Docker Compose

**Dockerfile:**
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

# Environment variables can be passed at runtime
ENV BKASH__ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "YourApp.dll"]
```

**docker-compose.yml:**
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

**Run with environment variables:**
```bash
# Using .env file
docker-compose up

# Or pass directly
docker run -e BKASH__APPKEY=your-key \
           -e BKASH__APPSECRET=your-secret \
           your-image
```

---

### Kubernetes

**Create Secret:**
```bash
kubectl create secret generic bkash-credentials \
  --from-literal=appkey='your-app-key' \
  --from-literal=appsecret='your-app-secret' \
  --from-literal=username='your-username' \
  --from-literal=password='your-password' \
  --namespace=your-namespace
```

**Or from .env file:**
```bash
kubectl create secret generic bkash-credentials \
  --from-env-file=.env.production \
  --namespace=your-namespace
```

**Deployment YAML:**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: bkash-api
  namespace: your-namespace
spec:
  replicas: 3
  selector:
    matchLabels:
      app: bkash-api
  template:
    metadata:
      labels:
        app: bkash-api
    spec:
      containers:
      - name: api
        image: your-registry/bkash-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
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

---

### Heroku

```bash
# Set environment variables
heroku config:set BKASH__APPKEY=your-key
heroku config:set BKASH__APPSECRET=your-secret
heroku config:set BKASH__USERNAME=your-username
heroku config:set BKASH__PASSWORD=your-password
heroku config:set BKASH__ENVIRONMENT=Production

# View current config
heroku config
```

---

### GitHub Actions CI/CD

**.github/workflows/deploy.yml:**
```yaml
name: Deploy

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 9.0.x
      
      - name: Build
        run: dotnet build
        env:
          BKASH__APPKEY: ${{ secrets.BKASH_APPKEY }}
          BKASH__APPSECRET: ${{ secrets.BKASH_APPSECRET }}
          BKASH__USERNAME: ${{ secrets.BKASH_USERNAME }}
          BKASH__PASSWORD: ${{ secrets.BKASH_PASSWORD }}
      
      - name: Deploy
        run: |
          # Your deployment commands
```

Store secrets in GitHub repository settings under Settings > Secrets and variables > Actions.

---

## Security Best Practices

### ? DO

1. **Use Secret Management Services in Production**
   - Azure Key Vault
   - AWS Secrets Manager
   - HashiCorp Vault
   - Google Cloud Secret Manager

2. **Rotate Credentials Regularly**
   ```bash
   # Document rotation schedule
   # Update in all environments simultaneously
   ```

3. **Use Different Credentials Per Environment**
   - Sandbox credentials for development/staging
   - Production credentials only in production
   - Different credentials for each team/service

4. **Restrict Access**
   - Limit who can view production credentials
   - Use role-based access control
   - Audit access logs

5. **Use Environment-Specific Files**
   ```
   .env.development
   .env.staging
   .env.production
   ```

### ? DON'T

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
   - Use temporary access links

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

---

## Troubleshooting

### Issue: Environment Variables Not Loading

**Check Configuration:**
```csharp
// Add this to see what's loaded
var config = builder.Configuration
    .GetSection("Bkash")
    .Get<BkashOptions>();

Console.WriteLine($"AppKey: {config?.AppKey?.Substring(0, 5)}...");
Console.WriteLine($"Environment: {config?.Environment}");
```

**Verify Environment Variable Naming:**
- ? Correct: `BKASH__APPKEY` (double underscore)
- ? Wrong: `BKASH_APPKEY` (single underscore)

### Issue: Configuration Override Not Working

**Configuration Priority (highest to lowest):**
1. Command line arguments
2. Environment variables
3. User secrets (development only)
4. appsettings.{Environment}.json
5. appsettings.json

### Issue: Credentials Working Locally But Not in Production

**Check:**
1. Environment variables are set in production environment
2. Application has permission to read secrets
3. Correct environment name (case-sensitive)
4. No typos in variable names

### Testing Configuration

```csharp
// Startup validation
var builder = WebApplication.CreateBuilder(args);

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
        Console.WriteLine("  ? Configuration valid");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ? Configuration invalid: {ex.Message}");
        throw;
    }
});
```

---

## Complete Example

**Directory Structure:**
```
YourProject/
??? .env.example          # Template (committed)
??? .env                  # Local dev (not committed)
??? .env.sandbox          # Sandbox template
??? .env.production       # Production template
??? .gitignore
??? Program.cs
??? appsettings.json
```

**.gitignore:**
```gitignore
.env
.env.local
.env.production
.env.*.local
!.env.example
```

**Program.cs:**
```csharp
using DotNetEnv;
using Bikiran.Payment.Bkash;

// Load .env in development
if (args.Contains("--load-env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBkashPayment(builder.Configuration);
builder.Services.AddHealthChecks().AddBkashHealthCheck("bkash");

var app = builder.Build();

app.MapHealthChecks("/health");
app.Run();
```

**Run locally:**
```bash
dotnet run --load-env
```

---

## Quick Reference

| Platform | Method | Security Level |
|----------|--------|----------------|
| Local Dev | .env file | ?? Low |
| Visual Studio | launchSettings.json | ?? Low |
| Azure | Key Vault | ? High |
| AWS | Secrets Manager | ? High |
| Docker | Environment variables | ?? Medium |
| Kubernetes | Secrets | ? High |
| CI/CD | Repository secrets | ? High |

---

For more information, see:
- [.NET Configuration Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [ASP.NET Core Environment Variables](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/)
- [AWS Secrets Manager](https://aws.amazon.com/secrets-manager/)
