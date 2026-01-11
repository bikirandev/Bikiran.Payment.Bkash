# Health Checks API Reference

Monitor bKash service connectivity and health.

## Overview

Bikiran.Payment.Bkash integrates with ASP.NET Core Health Checks to monitor bKash service availability.

## Adding Health Checks

### Basic Setup

```csharp
using Bikiran.Payment.Bkash;

var builder = WebApplication.CreateBuilder(args);

// Add bKash services
builder.Services.AddBkashPayment(builder.Configuration);

// Add health checks
builder.Services.AddHealthChecks()
    .AddBkashHealthCheck("bkash");

var app = builder.Build();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
```

### With Tags

```csharp
builder.Services.AddHealthChecks()
    .AddBkashHealthCheck(
        name: "bkash",
        tags: new[] { "payment", "external", "ready" }
    );
```

## Health Check Endpoint

### Basic Endpoint

```csharp
app.MapHealthChecks("/health");
```

Test:
```bash
curl http://localhost:5000/health
```

Response:
```
Healthy
```

### Detailed JSON Response

```csharp
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        
        await context.Response.WriteAsync(result);
    }
});
```

Response:
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "bkash",
      "status": "Healthy",
      "description": "bKash service is accessible",
      "duration": 523.45
    }
  ],
  "totalDuration": 523.45
}
```

## Health Status

| Status | Description |
|--------|-------------|
| `Healthy` | bKash service is accessible and responding |
| `Degraded` | Not used by bKash health check |
| `Unhealthy` | Cannot connect to bKash service |

## What It Checks

The bKash health check:
1. Attempts to obtain a valid token from bKash
2. Returns Healthy if successful
3. Returns Unhealthy if authentication fails

## Multiple Endpoints

Create separate endpoints for different purposes:

```csharp
// Liveness - Is the app running?
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false  // Don't run any checks
});

// Readiness - Is the app ready to serve requests?
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// All health checks
app.MapHealthChecks("/health");
```

## Kubernetes Integration

### Liveness Probe

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 30
```

### Readiness Probe

```yaml
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10
```

## Azure Application Insights

Enable health check telemetry:

```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;

builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(10);
    options.Period = TimeSpan.FromSeconds(30);
});

builder.Services.AddApplicationInsightsTelemetry();
```

## Docker Health Check

Add to Dockerfile:

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1
```

## Monitoring & Alerting

### Prometheus

```csharp
dotnet add package AspNetCore.HealthChecks.Prometheus
```

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = PrometheusResponseWriter.WritePrometheusResultText
});
```

### Custom Monitoring

```csharp
var healthCheckService = app.Services.GetRequiredService<HealthCheckService>();
var result = await healthCheckService.CheckHealthAsync();

if (result.Status == HealthStatus.Unhealthy)
{
    // Send alert
    await _alertService.SendAlertAsync("bKash service is down");
}
```

## Best Practices

1. **Separate endpoints** for liveness and readiness
2. **Set appropriate timeouts** for health checks
3. **Monitor health check failures** and alert
4. **Don't perform expensive operations** in health checks
5. **Use health checks** in all environments

## Next Steps

- 📖 [Payment Operations](payment-operations.md)
- 📖 [Token Management](token-management.md)
- 🌐 [Production Deployment](../guides/production-deployment.md)
