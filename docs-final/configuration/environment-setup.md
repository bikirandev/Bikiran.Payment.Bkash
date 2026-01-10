# Environment Setup Guide

Platform-specific configuration instructions for Bikiran.Payment.Bkash across different deployment environments.

## Quick Links

- [Local Development](#local-development)
- [Azure App Service](#azure-app-service)
- [AWS Elastic Beanstalk](#aws-elastic-beanstalk)
- [Docker](#docker)
- [Kubernetes](#kubernetes)
- [Heroku](#heroku)
- [GitHub Actions CI/CD](#github-actions-cicd)

## Local Development

### Using .env Files (Recommended)

**Step 1: Install DotNetEnv**
```bash
dotnet add package DotNetEnv
```

**Step 2: Create `.env` file**
```env
BKASH__APPKEY=4f6o0cjiki2rfm34kfdadl1eqq
BKASH__APPSECRET=2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b
BKASH__USERNAME=sandboxTokenizedUser02
BKASH__PASSWORD=sandboxTokenizedUser02@12345
BKASH__ENVIRONMENT=Sandbox
```

**Step 3: Load in Program.cs**
```csharp
using DotNetEnv;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBkashPayment(builder.Configuration);
```

**Step 4: Add to .gitignore**
```gitignore
.env
.env.local
.env.*.local
```

### Using launchSettings.json (Visual Studio)

**Properties/launchSettings.json:**
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

## Azure App Service

### Option 1: Azure Portal

1. Navigate to your App Service in Azure Portal
2. Go to **Settings** → **Configuration** → **Application settings**
3. Click **New application setting** and add each:
   - Name: `BKASH__APPKEY`, Value: `your-key`
   - Name: `BKASH__APPSECRET`, Value: `your-secret`
   - Name: `BKASH__USERNAME`, Value: `your-username`
   - Name: `BKASH__PASSWORD`, Value: `your-password`
   - Name: `BKASH__ENVIRONMENT`, Value: `Production`
4. Click **Save**

### Option 2: Azure CLI

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

### Option 3: Azure Key Vault (Recommended for Production)

**Install packages:**
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

**Store in Key Vault as:**
- `Bkash--AppKey`
- `Bkash--AppSecret`
- `Bkash--Username`
- `Bkash--Password`

## AWS Elastic Beanstalk

### Option 1: EB CLI

```bash
eb setenv \
  BKASH__APPKEY=your-key \
  BKASH__APPSECRET=your-secret \
  BKASH__USERNAME=your-username \
  BKASH__PASSWORD=your-password \
  BKASH__ENVIRONMENT=Production
```

### Option 2: .ebextensions Configuration

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

### Option 3: AWS Secrets Manager (Recommended for Production)

**Install package:**
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

**Store secrets as:**
- `bkash/AppKey`
- `bkash/AppSecret`
- `bkash/Username`
- `bkash/Password`

## Docker

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

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
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

### docker-compose.yml

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

### Running with Docker

```bash
# Using environment variables
docker run \
  -e BKASH__APPKEY=your-key \
  -e BKASH__APPSECRET=your-secret \
  -e BKASH__USERNAME=your-username \
  -e BKASH__PASSWORD=your-password \
  -e BKASH__ENVIRONMENT=Production \
  -p 8080:8080 \
  your-image

# Using .env file
docker-compose up
```

## Kubernetes

### Create Secret

```bash
kubectl create secret generic bkash-credentials \
  --from-literal=appkey='your-app-key' \
  --from-literal=appsecret='your-app-secret' \
  --from-literal=username='your-username' \
  --from-literal=password='your-password' \
  --namespace=your-namespace
```

### Deployment YAML

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

## Heroku

```bash
# Set environment variables
heroku config:set BKASH__APPKEY=your-key
heroku config:set BKASH__APPSECRET=your-secret
heroku config:set BKASH__USERNAME=your-username
heroku config:set BKASH__PASSWORD=your-password
heroku config:set BKASH__ENVIRONMENT=Production

# View current config
heroku config

# Remove a config variable
heroku config:unset BKASH__APPKEY
```

## GitHub Actions CI/CD

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
          BKASH__ENVIRONMENT: Production
      
      - name: Test
        run: dotnet test
      
      - name: Deploy
        run: |
          # Your deployment commands here
```

**Configure secrets in GitHub:**
1. Go to repository **Settings** → **Secrets and variables** → **Actions**
2. Add repository secrets:
   - `BKASH_APPKEY`
   - `BKASH_APPSECRET`
   - `BKASH_USERNAME`
   - `BKASH_PASSWORD`

## Troubleshooting

### Environment Variables Not Loading

**Check the naming convention:**
- ✅ Correct: `BKASH__APPKEY` (double underscore)
- ❌ Wrong: `BKASH_APPKEY` (single underscore)

**Verify configuration:**
```csharp
var config = app.Services.GetRequiredService<IOptions<BkashOptions>>();
Console.WriteLine($"Environment: {config.Value.Environment}");
Console.WriteLine($"AppKey Length: {config.Value.AppKey?.Length}");
```

### Authentication Fails

1. Verify credentials match the environment (Sandbox vs Production)
2. Check that credentials haven't expired
3. Ensure no extra spaces in configuration values
4. Test with the default sandbox credentials first

### Configuration Override Not Working

Remember the configuration priority:
1. Command line arguments (highest)
2. Environment variables
3. User secrets (development only)
4. appsettings.{Environment}.json
5. appsettings.json (lowest)

## Security Best Practices

### ✅ DO

1. **Use secret managers in production** (Key Vault, Secrets Manager)
2. **Rotate credentials regularly** (every 90 days recommended)
3. **Use different credentials per environment**
4. **Restrict access to production credentials**
5. **Audit secret access logs**
6. **Use least-privilege access principles**

### ❌ DON'T

1. **Never commit credentials to source control**
2. **Don't use production credentials locally**
3. **Don't share credentials via email/chat**
4. **Don't log sensitive configuration**
5. **Don't hardcode credentials**

## Platform Comparison

| Platform | Security Level | Ease of Setup | Best For |
|----------|---------------|---------------|----------|
| Local .env | 🔒 Low | ⭐⭐⭐ Easy | Development |
| Azure Key Vault | 🔒🔒🔒 High | ⭐⭐ Medium | Production (Azure) |
| AWS Secrets Manager | 🔒🔒🔒 High | ⭐⭐ Medium | Production (AWS) |
| Kubernetes Secrets | 🔒🔒 Medium | ⭐⭐ Medium | Container orchestration |
| Docker env vars | 🔒🔒 Medium | ⭐⭐⭐ Easy | Containerized apps |
| GitHub Secrets | 🔒🔒🔒 High | ⭐⭐⭐ Easy | CI/CD pipelines |

## Next Steps

- 📖 [Configuration Overview](overview.md) - All configuration options
- 🎯 [Quick Reference](quick-reference.md) - Configuration cheat sheet
- 🔒 [Security Best Practices](../guides/security-best-practices.md) - Secure your integration
- 🚀 [Production Deployment](../guides/production-deployment.md) - Deploy your application

---

Need help? [Open an issue](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues) or check the [main documentation](../README.md).
