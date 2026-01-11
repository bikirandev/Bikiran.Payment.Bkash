# Project Structure

Overview of the Bikiran.Payment.Bkash codebase structure.

## Directory Structure

```
Bikiran.Payment.Bkash/
├── Configuration/
│   └── BkashOptions.cs              # Configuration model
├── Exceptions/
│   ├── BkashException.cs            # Base exception
│   ├── BkashAuthenticationException.cs
│   ├── BkashPaymentException.cs
│   └── BkashConfigurationException.cs
├── HealthChecks/
│   └── BkashHealthCheck.cs          # ASP.NET Core health check
├── Models/
│   ├── Endpoints/                   # Endpoint wrapper models
│   │   └── BkashEndpoint.cs         # Generic response wrapper
│   ├── Requests/                    # Request models
│   │   ├── BkashCreatePaymentRequest.cs
│   │   ├── BkashExecutePaymentRequest.cs
│   │   ├── BkashQueryPaymentRequest.cs
│   │   ├── BkashRefundPaymentRequest.cs
│   │   └── BkashRefundStatusRequest.cs
│   ├── Responses/                   # Response models
│   │   ├── BkashBaseResponse.cs
│   │   ├── BkashCreatePaymentResponse.cs
│   │   ├── BkashExecutePaymentResponse.cs
│   │   ├── BkashQueryPaymentResponse.cs
│   │   ├── BkashRefundPaymentResponse.cs
│   │   ├── BkashRefundStatusResponse.cs
│   │   ├── BkashGrantTokenResponse.cs
│   │   └── BkashRefreshTokenResponse.cs
│   ├── Webhooks/
│   │   └── BkashWebhookNotification.cs
│   └── BkashTransactionMode.cs      # Transaction mode enum
├── Services/
│   ├── IBkashPaymentService.cs      # Payment service interface
│   ├── BkashPaymentService.cs       # Payment service implementation
│   ├── IBkashTokenService.cs        # Token service interface
│   └── BkashTokenService.cs         # Token service implementation
├── Utilities/
│   └── BkashWebhookHelper.cs        # Webhook signature verification
├── ServiceCollectionExtensions.cs   # DI extensions
└── Bikiran.Payment.Bkash.csproj    # Project file
```

## Key Components

### Configuration

- **BkashOptions**: Configuration model with validation

### Services

- **BkashPaymentService**: Handles payment operations
- **BkashTokenService**: Manages authentication tokens (Singleton)

### Models

- **Endpoint Models**: Generic response wrapper for standardized API responses
- **Request Models**: Strongly-typed API request models
- **Response Models**: Strongly-typed API response models
- **Webhook Models**: Webhook notification models

### Exceptions

- Custom exception hierarchy for error handling

### Health Checks

- Integration with ASP.NET Core health checks

### Utilities

- Helper classes for common operations

## Dependencies

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0" />
  <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="9.0.0" />
  <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
  <PackageReference Include="Microsoft.Extensions.Options" Version="9.0.0" />
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

## Design Patterns

1. **Dependency Injection**: Services registered via extension methods
2. **Options Pattern**: Configuration via BkashOptions
3. **Singleton Pattern**: Token service for caching
4. **Factory Pattern**: HttpClient factory for HTTP requests

## Code Conventions

- XML documentation for all public APIs
- Async/await for all I/O operations
- Modern C# patterns (.NET 9)
- Proper exception handling
- Validation in request models

## Next Steps

- 📖 [Local Development Setup](local-setup.md)
- 📖 [Contributing Guidelines](contributing.md)
- 📖 [Testing Guide](testing.md)
