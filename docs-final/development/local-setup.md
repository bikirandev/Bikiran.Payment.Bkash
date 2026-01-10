# Local Development Setup

Guide for setting up a local development environment for contributing to Bikiran.Payment.Bkash.

## Prerequisites

- .NET 9.0 SDK
- Git
- IDE (Visual Studio 2022, VS Code, or Rider)
- bKash sandbox credentials

## Clone Repository

```bash
git clone https://github.com/bikirandev/Bikiran.Payment.Bkash.git
cd Bikiran.Payment.Bkash
```

## Project Structure

```
Bikiran.Payment.Bkash/
├── Configuration/          # Configuration models
├── Exceptions/             # Custom exceptions
├── HealthChecks/           # Health check implementations
├── Models/                 # Request/Response models
│   ├── Requests/
│   ├── Responses/
│   └── Webhooks/
├── Services/               # Core services
│   ├── BkashPaymentService.cs
│   └── BkashTokenService.cs
├── Utilities/              # Helper utilities
├── ServiceCollectionExtensions.cs
└── Bikiran.Payment.Bkash.csproj
```

## Build Project

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Build in Release mode
dotnet build --configuration Release
```

## Run Tests

```bash
# Run all tests (when available)
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Local Package Testing

See [Testing Guide](testing.md) for detailed local package testing instructions.

## Code Style

- Follow .NET 9 best practices
- Use modern C# patterns
- Add XML documentation for public APIs
- Use meaningful variable names
- Keep methods focused and small

## Next Steps

- 📖 [Testing Guide](testing.md)
- 📖 [Project Structure](project-structure.md)
- 📖 [Contributing Guidelines](contributing.md)
