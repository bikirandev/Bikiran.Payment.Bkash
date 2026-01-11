# Token Management API Reference

Automatic token management with intelligent refresh capabilities.

## Overview

Bikiran.Payment.Bkash automatically handles bKash API token management including:

- Token generation (grant)
- Token refresh
- Automatic expiry detection
- Thread-safe caching

## IBkashTokenService Interface

```csharp
public interface IBkashTokenService
{
    Task<string> GetValidTokenAsync(CancellationToken cancellationToken = default);
    Task<BkashGrantTokenResponse> GrantTokenAsync(CancellationToken cancellationToken = default);
    Task<BkashRefreshTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
```

## Automatic Token Management

**You don't need to call token methods directly!** The library automatically:

1. Requests a token on first API call
2. Caches the token in memory
3. Refreshes before expiry (configurable buffer)
4. Handles token refresh failures gracefully

```csharp
// Just call payment methods - tokens are handled automatically
var response = await _bkashService.CreatePaymentAsync(request);
```

## Token Lifecycle

```
┌─────────────────────────────────────────────────┐
│ First API Call                                  │
│  ↓                                             │
│ Grant Token (credentials) → ID Token (55 min)  │
│                            → Refresh Token (28d)│
│  ↓                                             │
│ Cache Token (in-memory, thread-safe)           │
│  ↓                                             │
│ Use Token for API Calls                        │
│  ↓                                             │
│ Check Expiry Before Each Call                  │
│  ↓                                             │
│ Refresh if Near Expiry (5 min buffer)          │
│  ↓                                             │
│ Use Refreshed Token                            │
└─────────────────────────────────────────────────┘
```

## Token Types

### ID Token

- **Validity**: 55 minutes
- **Usage**: API authentication
- **Auto-refresh**: 5 minutes before expiry (configurable)

### Refresh Token

- **Validity**: 28 days
- **Usage**: Obtaining new ID token
- **Behavior**: Automatically used when ID token expires

## Configuration

Control token refresh behavior:

```csharp
builder.Services.AddBkashPayment(options =>
{
    options.AppKey = "your-key";
    options.AppSecret = "your-secret";
    options.Username = "your-username";
    options.Password = "your-password";
    options.Environment = BkashEnvironment.Sandbox;

    // Refresh token 10 minutes before expiry (default: 300 seconds = 5 minutes)
    options.TokenRefreshBufferSeconds = 600;
});
```

## Manual Token Operations

### GetValidTokenAsync

Gets a valid token, automatically refreshing if needed.

```csharp
var token = await _tokenService.GetValidTokenAsync();
// Returns: "eyJhbGciOiJIUzI1NiIsInR5..."
```

### GrantTokenAsync

Manually request a new token (rarely needed).

```csharp
var response = await _tokenService.GrantTokenAsync();
Console.WriteLine($"ID Token: {response.IdToken}");
Console.WriteLine($"Refresh Token: {response.RefreshToken}");
Console.WriteLine($"Expires In: {response.ExpiresIn} seconds");
Console.WriteLine($"Token Type: {response.TokenType}");
```

### RefreshTokenAsync

Manually refresh a token (handled automatically).

```csharp
var response = await _tokenService.RefreshTokenAsync(refreshToken);
Console.WriteLine($"New ID Token: {response.IdToken}");
Console.WriteLine($"New Refresh Token: {response.RefreshToken}");
Console.WriteLine($"Expires In: {response.ExpiresIn} seconds");
```

## Thread Safety

The token service is implemented as a **Singleton** with thread-safe operations:

```csharp
// Multiple concurrent requests safely share the same token
await Task.WhenAll(
    _bkashService.CreatePaymentAsync(request1),
    _bkashService.CreatePaymentAsync(request2),
    _bkashService.CreatePaymentAsync(request3)
);
// All use the same cached token - no duplicate token requests
```

## Token Storage

- **Location**: In-memory only (not persisted)
- **Scope**: Application lifetime
- **Security**: Never logged or exposed

## Error Handling

```csharp
try
{
    var token = await _tokenService.GetValidTokenAsync();
}
catch (BkashAuthenticationException ex)
{
    // Invalid credentials or authentication failure
    _logger.LogError(ex, "Token authentication failed");
}
```

## Best Practices

1. **Don't call token methods directly** - Let the library handle it
2. **Use Singleton registration** (default) for proper caching
3. **Configure appropriate refresh buffer** for your needs
4. **Monitor authentication errors** in logs
5. **Rotate credentials regularly** in production

## Next Steps

- 📖 [Payment Operations](payment-operations.md)
- 🔐 [Webhook Handling](webhook-handling.md)
- 🔍 [Health Checks](health-checks.md)
