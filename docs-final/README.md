# Bikiran.Payment.Bkash Documentation

Welcome to the comprehensive documentation for **Bikiran.Payment.Bkash** - a production-ready .NET library for integrating bKash payment gateway with automatic token management, payment processing, and refund capabilities.

## 📚 Documentation Structure

### 🚀 Getting Started
Start here if you're new to the library:
- [Quick Start Guide](getting-started/quick-start.md) - Get up and running in 5 minutes
- [Installation Guide](getting-started/installation.md) - Detailed installation instructions
- [Basic Examples](getting-started/basic-examples.md) - Simple code examples to get started

### ⚙️ Configuration
Learn how to configure the library for your environment:
- [Configuration Overview](configuration/overview.md) - All configuration options explained
- [Environment Setup](configuration/environment-setup.md) - Setup for development, staging, and production
- [Quick Reference](configuration/quick-reference.md) - Quick configuration cheat sheet

### 📖 API Reference
Detailed API documentation:
- [Payment Operations](api-reference/payment-operations.md) - Create, execute, and query payments
- [Refund Operations](api-reference/refund-operations.md) - Process and track refunds
- [Token Management](api-reference/token-management.md) - Automatic token handling
- [Webhook Handling](api-reference/webhook-handling.md) - Secure webhook verification
- [Health Checks](api-reference/health-checks.md) - Monitor service health

### 📘 Guides
In-depth guides for common scenarios:
- [Payment Flow Guide](guides/payment-flow.md) - Complete payment flow walkthrough
- [Webhook Integration](guides/webhook-integration.md) - Implementing webhooks securely
- [Error Handling](guides/error-handling.md) - Handling errors and exceptions
- [Production Deployment](guides/production-deployment.md) - Deploy to various platforms
- [Security Best Practices](guides/security-best-practices.md) - Keep your integration secure

### 🔧 Development
For contributors and developers:
- [Local Development Setup](development/local-setup.md) - Setup development environment
- [Testing Guide](development/testing.md) - Test the package locally
- [Project Structure](development/project-structure.md) - Understanding the codebase
- [Contributing Guidelines](development/contributing.md) - How to contribute
- [Changelog](development/changelog.md) - Version history and changes

## 🎯 Quick Links

### Essential Resources
- **Main README**: [Project Overview](../README.md)
- **NuGet Package**: [Bikiran.Payment.Bkash](https://www.nuget.org/packages/Bikiran.Payment.Bkash)
- **GitHub Repository**: [bikirandev/Bikiran.Payment.Bkash](https://github.com/bikirandev/Bikiran.Payment.Bkash)
- **bKash Developer Portal**: [developer.bka.sh](https://developer.bka.sh/)

### Common Tasks
- [Install the package](getting-started/installation.md#installation-from-nuget)
- [Configure for sandbox](configuration/overview.md#sandbox-configuration)
- [Create your first payment](getting-started/basic-examples.md#creating-a-payment)
- [Setup webhooks](guides/webhook-integration.md)
- [Deploy to Azure](guides/production-deployment.md#azure-app-service)
- [Deploy to AWS](guides/production-deployment.md#aws-elastic-beanstalk)

## 🌟 Features

- ✅ **Automatic token management** with intelligent refresh (55-minute ID tokens, 28-day refresh tokens)
- ✅ **Complete payment processing** - Create, execute, query, and cancel payments
- ✅ **Refund support** - Full and partial refunds with status tracking
- ✅ **Multi-environment support** - Seamless switching between sandbox and production
- ✅ **Built-in security** - Webhook signature verification and replay attack prevention
- ✅ **Health monitoring** - Integration with ASP.NET Core health checks
- ✅ **Strongly-typed models** - Full IntelliSense support
- ✅ **Dependency injection** - Native DI support for .NET applications
- ✅ **Comprehensive error handling** - Specific exceptions for different error types

## 🆘 Getting Help

### Documentation
If you can't find what you're looking for:
1. Check the [API Reference](api-reference/) for specific methods
2. Browse the [Guides](guides/) for detailed walkthroughs
3. Review the [Configuration](configuration/) section for setup issues

### Support
- **Issues**: [GitHub Issues](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues)
- **Discussions**: [GitHub Discussions](https://github.com/bikirandev/Bikiran.Payment.Bkash/discussions)
- **Email**: merchant.service@bka.sh (for bKash-specific questions)

## 📝 Version Information

- **Current Version**: 1.0.0
- **Target Framework**: .NET 9.0
- **License**: MIT
- **Last Updated**: January 2025

## 🔄 Migration Guides

- [Migrating from v0.x to v1.0](development/changelog.md#migration-guide) (if applicable)

---

**Ready to get started?** Head over to the [Quick Start Guide](getting-started/quick-start.md)!
