# Error Handling Guide

Comprehensive guide to handling errors in bKash payment integration.

## Exception Types

### BkashAuthenticationException
Thrown when authentication fails.

```csharp
catch (BkashAuthenticationException ex)
{
    // Invalid credentials, expired token, etc.
    _logger.LogError(ex, "Authentication failed");
    // Check credentials in configuration
}
```

### BkashPaymentException
Thrown when payment operation fails.

```csharp
catch (BkashPaymentException ex)
{
    _logger.LogError(ex, "Payment failed: {ErrorCode}", ex.ErrorCode);
    // ex.ErrorCode contains bKash error code
    // ex.HttpStatusCode contains HTTP status
}
```

### BkashException
Base exception for all bKash-related errors.

```csharp
catch (BkashException ex)
{
    _logger.LogError(ex, "bKash error");
}
```

## Common Error Codes

| Code | Description | Solution |
|------|-------------|----------|
| 2001 | Invalid amount | Check amount is positive |
| 2014 | Payment already executed | Implement idempotency |
| 2029 | Payment expired | Timeout after 15 minutes |
| 2062 | Payment not found | Verify payment ID |

## Best Practices

1. **Log all errors** with context
2. **Show user-friendly messages** to customers
3. **Implement retry logic** for transient failures
4. **Monitor error rates** in production
5. **Alert on critical errors**

## Next Steps

- 📖 [Payment Operations API](../api-reference/payment-operations.md)
- 📖 [Payment Flow Guide](payment-flow.md)
