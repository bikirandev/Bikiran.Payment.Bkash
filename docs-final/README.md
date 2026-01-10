# Bikiran.Payment.Bkash Documentation

Complete documentation for the Bikiran.Payment.Bkash library - a comprehensive .NET library for integrating bKash payment gateway.

## 📚 Documentation Structure

This documentation is organized into focused, non-overlapping sections:

### 1. [Getting Started](01-getting-started.md)
**Start here!** Quick installation and basic setup guide.
- Installation instructions
- Quick start with code examples
- Sandbox credentials for testing
- First payment flow

### 2. [Configuration Guide](02-configuration.md)
**Complete configuration reference** for all platforms and environments.
- Local development setup (appsettings.json, .env files, environment variables)
- Platform-specific setup (Azure, AWS, Docker, Kubernetes, Heroku)
- Security best practices
- Troubleshooting configuration issues

### 3. [API Usage Guide](03-api-usage.md)
**Comprehensive API usage examples** and integration patterns.
- Payment flow (create, execute, query)
- Refund operations
- Webhook handling with signature verification
- Health monitoring
- Error handling patterns
- Complete controller examples

### 4. [Quick Reference](04-quick-reference.md)
**Fast lookup** for common configurations and code snippets.
- Configuration formats
- Common commands
- Code snippets
- Exception types
- Security checklist

### 5. [Changelog](05-changelog.md)
**Version history** and release notes.
- Release notes for each version
- Breaking changes
- Migration guides
- Known limitations

## 🚀 Quick Navigation

### I'm New to This Library
→ Start with [Getting Started](01-getting-started.md)

### I Need to Configure for Production
→ See [Configuration Guide](02-configuration.md)

### I Want to Integrate Payment APIs
→ Check [API Usage Guide](03-api-usage.md)

### I Need a Quick Lookup
→ Use [Quick Reference](04-quick-reference.md)

### I Want to See What's New
→ Read [Changelog](05-changelog.md)

## ✨ Key Features

- ✅ Automatic token management with refresh
- ✅ Payment creation, execution, and status queries
- ✅ Refund processing with status tracking
- ✅ Multi-environment support (Sandbox/Production)
- ✅ Health check integration
- ✅ Webhook signature verification
- ✅ Strongly-typed models
- ✅ Comprehensive error handling
- ✅ Dependency injection support

## 📦 Installation

```bash
dotnet add package Bikiran.Payment.Bkash
```

## 🎯 Quick Example

```csharp
// Configure in Program.cs
builder.Services.AddBkashPayment(builder.Configuration);

// Use in your code
public class PaymentController : ControllerBase
{
    private readonly IBkashPaymentService _bkashService;
    
    public PaymentController(IBkashPaymentService bkashService)
    {
        _bkashService = bkashService;
    }
    
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment(decimal amount)
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = amount,
            MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
            Intent = "sale"
        };
        
        var response = await _bkashService.CreatePaymentAsync(request);
        return Ok(new { bkashUrl = response.BkashURL });
    }
}
```

## 🔐 Security Note

⚠️ **Never commit credentials to source control!**

Always use:
- Environment variables for local development
- Secret managers (Azure Key Vault, AWS Secrets Manager) for production
- `.gitignore` to exclude `.env` and credential files

See [Configuration Guide](02-configuration.md) for secure configuration methods.

## 🧪 Testing with Sandbox

Use these public sandbox credentials for testing:

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

## 📖 Complete Documentation Index

| Document | Description | Best For |
|----------|-------------|----------|
| [01-getting-started.md](01-getting-started.md) | Installation and quick start | New users |
| [02-configuration.md](02-configuration.md) | Complete configuration guide | DevOps, deployment |
| [03-api-usage.md](03-api-usage.md) | API integration examples | Developers |
| [04-quick-reference.md](04-quick-reference.md) | Quick lookup reference | Everyone |
| [05-changelog.md](05-changelog.md) | Version history | Existing users |

## 🛠️ Support

- **GitHub Issues**: https://github.com/bikirandev/Bikiran.Payment.Bkash/issues
- **bKash Support**: merchant.service@bka.sh
- **Website**: https://www.bka.sh/

## 📝 License

MIT License - see LICENSE file for details.

## 🙏 Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

---

**Package Version**: 1.0.0  
**Target Framework**: .NET 9.0  
**Last Updated**: January 2025
