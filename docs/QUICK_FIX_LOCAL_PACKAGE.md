# Quick Fix for Local Package Installation Error

## Problem
```
error: NU1301: The local source 'D:\P_Bikiran_SSO_Repos\7201DOT-Bikiran-API\LocalBkash' doesn't exist.
```

## Root Cause
The package source path is being resolved relative to your project directory instead of using the absolute path you specified.

## Quick Solutions

### Solution 1: Remove and Re-add Source (Fastest)

```powershell
# Navigate to your project
cd D:\P_Bikiran_SSO_Repos\7201DOT-Bikiran-API

# Remove the problematic source
dotnet nuget remove source LocalBkash

# Add it back with the correct path
dotnet nuget add source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" -n LocalBkash

# Verify it's added correctly
dotnet nuget list source

# Try installing the package again
dotnet add package Bikiran.Payment.Bkash --version 1.0.0

# If still fails, try with explicit restore
dotnet restore --source LocalBkash --source https://api.nuget.org/v3/index.json
```

### Solution 2: Create Local nuget.config (Most Reliable)

```powershell
# Navigate to your project
cd D:\P_Bikiran_SSO_Repos\7201DOT-Bikiran-API

# Create nuget.config file
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="LocalBkash" value="D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath nuget.config -Encoding utf8

# Now add the package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0
```

### Solution 3: Manual Package Reference

```powershell
# Edit BikiranWebAPI.csproj and add this ItemGroup:
```

```xml
<ItemGroup>
  <PackageReference Include="Bikiran.Payment.Bkash" Version="1.0.0" />
</ItemGroup>
```

```powershell
# Then restore with explicit sources
dotnet restore --source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" --source https://api.nuget.org/v3/index.json
```

### Solution 4: Direct .nupkg Installation

```powershell
# Install directly from the .nupkg file
dotnet add package Bikiran.Payment.Bkash --version 1.0.0 --source "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release"
```

## Verification Steps

After applying any solution, verify:

```powershell
# 1. Check if package is added to .csproj
cat BikiranWebAPI.csproj | Select-String "Bikiran.Payment.Bkash"

# 2. Check restore output
dotnet restore -v detailed

# 3. Build the project
dotnet build
```

## If All Solutions Fail

### Clean Everything and Start Fresh

```powershell
# 1. Remove all NuGet sources related to LocalBkash
dotnet nuget list source
dotnet nuget remove source LocalBkash

# 2. Clear all NuGet caches
dotnet nuget locals all --clear

# 3. Remove any package references from .csproj
# Edit BikiranWebAPI.csproj and remove Bikiran.Payment.Bkash lines

# 4. Create nuget.config in project directory
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="LocalBkash" value="D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath nuget.config -Encoding utf8

# 5. Verify the .nupkg file exists
Test-Path "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release\Bikiran.Payment.Bkash.1.0.0.nupkg"

# 6. Add package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0

# 7. Restore
dotnet restore

# 8. Build
dotnet build
```

## Alternative: Test in New Project First

If you want to verify the package works before adding to your main project:

```powershell
# Create test project
cd D:\
mkdir BkashTest
cd BkashTest
dotnet new console -n TestApp
cd TestApp

# Create nuget.config
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="LocalBkash" value="D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath nuget.config -Encoding utf8

# Add package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0

# If this works, copy the nuget.config to your main project
```

## Important Notes

1. **Absolute Paths**: Always use absolute paths for local sources
2. **Source Priority**: Local nuget.config takes precedence over global sources
3. **Cache Issues**: Clear caches if you update the .nupkg file
4. **Multiple Sources**: Ensure both local source and nuget.org are available for dependencies

## Still Having Issues?

Check these:

1. Verify package file exists:
   ```powershell
   Get-ChildItem "D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" -Filter "*.nupkg"
   ```

2. Check global nuget.config:
   ```powershell
   cat "$env:APPDATA\NuGet\NuGet.Config"
   ```

3. Check project-level nuget.config:
   ```powershell
   cat nuget.config
   ```

4. List all sources:
   ```powershell
   dotnet nuget list source
   ```

## Recommended Approach for Your Situation

Based on your error, I recommend **Solution 2** (Create Local nuget.config) as it's the most reliable and project-specific:

```powershell
cd D:\P_Bikiran_SSO_Repos\7201DOT-Bikiran-API

# Create nuget.config
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="LocalBkash" value="D:\P_Bikiran_Packages\Bikiran.Payment.Bkash\bin\Release" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath nuget.config -Encoding utf8

# Remove global LocalBkash source
dotnet nuget remove source LocalBkash

# Add package
dotnet add package Bikiran.Payment.Bkash --version 1.0.0

# Build
dotnet build
```

This ensures the source path is always correct relative to your project and won't have path resolution issues.
