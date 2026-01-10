# Installation Guide

Complete guide for installing Bikiran.Payment.Bkash in your .NET application.

## Requirements

- **.NET 9.0 SDK** or later
- **NuGet Package Manager** or .NET CLI
- **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider (optional but recommended)

## Installation from NuGet

### Using .NET CLI (Recommended)

```bash
dotnet add package Bikiran.Payment.Bkash
```

### Using Package Manager Console (Visual Studio)

```powershell
Install-Package Bikiran.Payment.Bkash
```

### Using Visual Studio NuGet Package Manager

1. Right-click on your project in Solution Explorer
2. Select **Manage NuGet Packages**
3. Search for **Bikiran.Payment.Bkash**
4. Click **Install**

### Manual Package Reference

Add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Bikiran.Payment.Bkash" Version="1.0.0" />
</ItemGroup>
```

Then restore packages:

```bash
dotnet restore
```

## Verifying Installation

### Check Package Version

```bash
dotnet list package
```

You should see:

```
Bikiran.Payment.Bkash    1.0.0
```

### Test Basic Import

Create a test file to verify the package is installed correctly:

```csharp
using Bikiran.Payment.Bkash;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.Configuration;

// If no compilation errors, the package is installed correctly
```

## Dependencies

The package automatically installs these dependencies:

- **Microsoft.Extensions.DependencyInjection.Abstractions** (9.0.0)
- **Microsoft.Extensions.Diagnostics.HealthChecks** (9.0.0)
- **Microsoft.Extensions.Http** (9.0.0)
- **Microsoft.Extensions.Logging.Abstractions** (9.0.0)
- **Microsoft.Extensions.Options** (9.0.0)
- **Newtonsoft.Json** (13.0.3)

These are standard Microsoft packages and should not cause conflicts in most projects.

## Project Types

### ASP.NET Core Web API

```bash
# Create new project
dotnet new webapi -n MyBkashApi
cd MyBkashApi

# Add package
dotnet add package Bikiran.Payment.Bkash

# Run
dotnet run
```

### ASP.NET Core MVC

```bash
# Create new project
dotnet new mvc -n MyBkashWeb
cd MyBkashWeb

# Add package
dotnet add package Bikiran.Payment.Bkash

# Run
dotnet run
```

### Console Application

```bash
# Create new project
dotnet new console -n MyBkashConsole
cd MyBkashConsole

# Add package
dotnet add package Bikiran.Payment.Bkash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Logging.Console

# Run
dotnet run
```

### Minimal API

```bash
# Create new project
dotnet new web -n MyBkashMinimal
cd MyBkashMinimal

# Add package
dotnet add package Bikiran.Payment.Bkash

# Run
dotnet run
```

## Installation from Local Package (Development)

If you're testing a local build of the package:

### Step 1: Add Local Package Source

```bash
# Add local source
dotnet nuget add source "D:\Path\To\Package\bin\Release" -n LocalBkash

# Verify source was added
dotnet nuget list source
```

### Step 2: Install Package

```bash
dotnet add package Bikiran.Payment.Bkash --version 1.0.0 --source LocalBkash
```

### Alternative: Using nuget.config

Create `nuget.config` in your project root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="LocalBkash" value="D:\Path\To\Package\bin\Release" />
  </packageSources>
</configuration>
```

Then install normally:

```bash
dotnet add package Bikiran.Payment.Bkash --version 1.0.0
```

## Troubleshooting

### Issue: Package Not Found

**Error**: `NU1101: Unable to find package Bikiran.Payment.Bkash`

**Solutions**:
1. Check your internet connection
2. Clear NuGet cache: `dotnet nuget locals all --clear`
3. Restore packages: `dotnet restore`
4. Verify package source: `dotnet nuget list source`

### Issue: Version Conflict

**Error**: `NU1605: Detected package downgrade`

**Solutions**:
1. Update all packages to latest compatible versions
2. Check for conflicting package versions in your project
3. Use specific version constraint: `dotnet add package Bikiran.Payment.Bkash --version 1.0.0`

### Issue: .NET Version Mismatch

**Error**: `Package Bikiran.Payment.Bkash 1.0.0 is not compatible with net8.0`

**Solutions**:
1. Update your project to .NET 9.0
2. Or wait for a version targeting your .NET version

### Issue: Missing Dependencies

**Error**: Missing required assemblies

**Solutions**:
1. Run `dotnet restore`
2. Clear NuGet cache: `dotnet nuget locals all --clear`
3. Delete `bin` and `obj` folders and rebuild

### Issue: Local Package Installation Fails

**Error**: `NU1301: The local source doesn't exist`

**Solutions**:
1. Use absolute path for local source
2. Verify the .nupkg file exists at the specified location
3. See [Testing Guide](../development/testing.md) for detailed local testing instructions

## Upgrading

To upgrade to the latest version:

```bash
# Update to latest version
dotnet add package Bikiran.Payment.Bkash

# Or specify version
dotnet add package Bikiran.Payment.Bkash --version 1.1.0
```

Check the [Changelog](../development/changelog.md) for breaking changes between versions.

## Uninstallation

To remove the package:

```bash
dotnet remove package Bikiran.Payment.Bkash
```

## Next Steps

After installation:

1. 🚀 [Quick Start Guide](quick-start.md) - Get started quickly
2. ⚙️ [Configuration](../configuration/overview.md) - Configure the library
3. 📖 [Basic Examples](basic-examples.md) - See code examples
4. 🔧 [API Reference](../api-reference/) - Detailed API documentation

---

Need help? Check the [Documentation Index](../README.md) or [open an issue](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues).
