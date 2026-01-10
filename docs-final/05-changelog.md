# Changelog

All notable changes to the Bikiran.Payment.Bkash project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-08

### 🎉 Initial Release

This is the first public release of Bikiran.Payment.Bkash - a comprehensive .NET library for integrating bKash payment gateway.

### Added

#### Core Payment Features
- Payment creation with bKash Tokenized Checkout API
- Payment execution after customer authorization
- Payment status query functionality
- Refund processing capabilities
- Refund status tracking
- Automatic token management (grant and refresh)
- Support for bKash API v1.2.0-beta

#### Production-Ready Features
- Health check integration for monitoring bKash service connectivity
- Webhook signature verification using HMAC-SHA256
- Replay attack prevention with timestamp validation
- Comprehensive request validation for all models
- Multi-environment support (Sandbox and Production)
- Configurable HTTP client timeouts
- Configurable token refresh buffer

#### Developer Experience
- Dependency injection support via `AddBkashPayment()` extension
- Strongly-typed request/response models
- Comprehensive XML documentation for IntelliSense
- Detailed error handling with specific exception types:
  - `BkashAuthenticationException`
  - `BkashPaymentException`
  - `BkashConfigurationException`
- Built-in validation for all request models
- Extensive usage examples and documentation

#### Security Features
- `BkashWebhookHelper` utility for webhook verification
- HMAC-SHA256 signature computation
- Timestamp validation to prevent replay attacks
- Secure in-memory token storage
- Thread-safe token management with `SemaphoreSlim`

#### Configuration Options
- Support for appsettings.json configuration
- Support for environment variables
- Configurable base URLs for custom endpoints
- Configurable timeout and token refresh settings

### Changed
- Token service registered as Singleton (instead of Scoped) for better caching
- Updated to use .NET 9 modern null-checking patterns:
  - `ArgumentNullException.ThrowIfNull()`
  - `ArgumentException.ThrowIfNullOrWhiteSpace()`

### Fixed
- Corrected duplicate service registration issue
- Fixed copyright year from 2026 to 2025

### Dependencies
- Microsoft.Extensions.DependencyInjection.Abstractions (9.0.0)
- Microsoft.Extensions.Diagnostics.HealthChecks (9.0.0)
- Microsoft.Extensions.Http (9.0.0)
- Microsoft.Extensions.Logging.Abstractions (9.0.0)
- Microsoft.Extensions.Options (9.0.0)
- Newtonsoft.Json (13.0.3)

### Known Limitations
- No built-in retry policies (consider using Polly)
- No rate limiting implementation
- No Agreement API support (recurring payments)
- No Search Transaction API support
- No B2B (Business-to-Business) payment support
- Unit test coverage not yet implemented

---

## [Unreleased]

### Planned for v1.1.0
- Comprehensive unit test suite
- Integration test suite
- HTTP retry policies with Polly
- Rate limiting support
- Request/response logging middleware
- Agreement API support for recurring payments
- Search Transaction API implementation
- OpenTelemetry integration for observability

---

## Version History

| Version | Release Date | Status | Notes |
|---------|--------------|--------|-------|
| 1.0.0 | 2025-01-08 | ✅ Released | Initial public release |

---

## Migration Guide

### From No Library to v1.0.0

If you're implementing bKash integration for the first time:

1. **Install the package**:
   ```bash
   dotnet add package Bikiran.Payment.Bkash
   ```

2. **Configure services**:
   ```csharp
   services.AddBkashPayment(options =>
   {
       options.AppKey = Configuration["Bkash:AppKey"];
       options.AppSecret = Configuration["Bkash:AppSecret"];
       options.Username = Configuration["Bkash:Username"];
       options.Password = Configuration["Bkash:Password"];
       options.Environment = BkashEnvironment.Sandbox;
   });
   ```

3. **Use the services**:
   ```csharp
   public class PaymentController : ControllerBase
   {
       private readonly IBkashPaymentService _bkashService;
       
       public PaymentController(IBkashPaymentService bkashService)
       {
           _bkashService = bkashService;
       }
   }
   ```

See the documentation for complete implementation examples.

---

## Breaking Changes

### v1.0.0
- None (initial release)

---

## Support

For issues, questions, or feature requests:
- **GitHub Issues**: https://github.com/bikirandev/Bikiran.Payment.Bkash/issues
- **Documentation**: See docs-final folder in the repository

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Acknowledgments

- bKash for providing the payment gateway API
- The .NET community for excellent tooling and libraries
- All contributors and users of this library

---

**Note**: This changelog follows the [Keep a Changelog](https://keepachangelog.com/) format and uses [Semantic Versioning](https://semver.org/).

- **MAJOR** version for incompatible API changes
- **MINOR** version for backwards-compatible functionality additions
- **PATCH** version for backwards-compatible bug fixes
