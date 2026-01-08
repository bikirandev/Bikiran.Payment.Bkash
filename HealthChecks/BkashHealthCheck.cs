using Microsoft.Extensions.Diagnostics.HealthChecks;
using Bikiran.Payment.Bkash.Services;

namespace Bikiran.Payment.Bkash.HealthChecks;

/// <summary>
/// Health check for bKash payment service connectivity
/// </summary>
public class BkashHealthCheck : IHealthCheck
{
    private readonly IBkashTokenService _tokenService;

    public BkashHealthCheck(IBkashTokenService tokenService)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    /// <summary>
    /// Checks if bKash service is accessible by attempting to get a token
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _tokenService.GetValidTokenAsync(cancellationToken);
            
            if (!string.IsNullOrEmpty(token))
            {
                return HealthCheckResult.Healthy("bKash service is accessible and token is valid");
            }
            
            return HealthCheckResult.Unhealthy("bKash service returned empty token");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "bKash service is not accessible",
                ex);
        }
    }
}
