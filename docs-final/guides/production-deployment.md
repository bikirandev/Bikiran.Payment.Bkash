# Production Deployment Guide

Deploy your bKash integration to production environments.

## Pre-Deployment Checklist

- [ ] Production credentials obtained from bKash
- [ ] All tests passing
- [ ] Security audit completed
- [ ] Monitoring configured
- [ ] Health checks working
- [ ] Backup and recovery plan documented
- [ ] Rollback plan prepared

## Platform-Specific Guides

### Azure App Service

See [Environment Setup - Azure](../configuration/environment-setup.md#azure-app-service) for detailed steps.

**Quick steps:**
1. Store credentials in Azure Key Vault
2. Configure App Service with Key Vault reference
3. Enable Application Insights
4. Configure health check probes
5. Set up auto-scaling

### AWS Elastic Beanstalk

See [Environment Setup - AWS](../configuration/environment-setup.md#aws-elastic-beanstalk) for detailed steps.

**Quick steps:**
1. Store credentials in AWS Secrets Manager
2. Configure environment variables
3. Enable CloudWatch monitoring
4. Configure health check URL
5. Set up auto-scaling policies

### Docker/Kubernetes

See [Environment Setup - Docker/Kubernetes](../configuration/environment-setup.md#docker) for detailed steps.

**Quick steps:**
1. Create Kubernetes secrets for credentials
2. Configure deployment with secrets
3. Set up liveness and readiness probes
4. Configure horizontal pod autoscaling
5. Set up ingress with TLS

## Configuration

### Required Settings

```json
{
  "Bkash": {
    "Environment": "Production",
    "TimeoutSeconds": 30,
    "TokenRefreshBufferSeconds": 300
  }
}
```

### Credentials

**Never hardcode production credentials!**

Use secret managers:
- Azure Key Vault
- AWS Secrets Manager
- Kubernetes Secrets
- HashiCorp Vault

## Monitoring

### Application Insights (Azure)

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### CloudWatch (AWS)

```csharp
builder.Logging.AddAWSProvider();
```

### Custom Metrics

```csharp
// Track payment metrics
_metrics.IncrementCounter("payments.created");
_metrics.RecordValue("payment.amount", amount);
```

## Health Checks

```csharp
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
```

Configure monitoring to:
- Check health every 30 seconds
- Alert if unhealthy for 2 minutes
- Auto-restart if consistently unhealthy

## Scaling

### Horizontal Scaling

bKash integration supports horizontal scaling:
- Token service is singleton (shared in-memory)
- Stateless payment operations
- No sticky sessions required

### Vertical Scaling

Recommended minimum resources:
- **CPU**: 1 vCPU
- **Memory**: 512 MB
- **Network**: Low latency to bKash API

## Disaster Recovery

### Backup Strategy

1. **Database backups** - hourly
2. **Configuration backups** - on change
3. **Application logs** - retained 90 days

### Recovery Plan

1. Identify issue
2. Check health endpoints
3. Review logs and metrics
4. Rollback if necessary
5. Fix and redeploy

## Performance Optimization

1. **Enable response caching** where appropriate
2. **Use CDN** for static assets
3. **Optimize database queries**
4. **Monitor API response times**
5. **Set appropriate timeouts**

## Security in Production

1. **Use HTTPS** everywhere
2. **Enable WAF** (Web Application Firewall)
3. **Implement rate limiting**
4. **Enable DDoS protection**
5. **Regular security scans**

## Post-Deployment

### Verification

- [ ] Health check endpoints responding
- [ ] Can create test payment
- [ ] Webhooks receiving notifications
- [ ] Logs flowing correctly
- [ ] Metrics being recorded
- [ ] Alerts configured

### Monitoring Checklist

- [ ] Payment success rate
- [ ] Payment failure rate
- [ ] Average payment time
- [ ] API error rates
- [ ] Authentication failures
- [ ] Webhook delivery rate

## Troubleshooting

### Common Issues

1. **Authentication fails** - Check credentials
2. **Timeouts** - Increase timeout settings
3. **Webhooks not received** - Verify URL and firewall
4. **High error rate** - Check logs and metrics

## Support

- **bKash Support**: merchant.service@bka.sh
- **GitHub Issues**: [Repository Issues](https://github.com/bikirandev/Bikiran.Payment.Bkash/issues)

## Next Steps

- 📖 [Environment Setup](../configuration/environment-setup.md)
- 📖 [Security Best Practices](security-best-practices.md)
- 📖 [Health Checks](../api-reference/health-checks.md)
