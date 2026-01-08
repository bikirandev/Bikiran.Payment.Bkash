using Bikiran.Payment.Bkash.Models.Responses;

namespace Bikiran.Payment.Bkash.Services;

/// <summary>
/// Interface for bKash token management
/// </summary>
public interface IBkashTokenService
{
    /// <summary>
    /// Gets a valid access token, automatically refreshing if needed
    /// </summary>
    /// <returns>Valid ID token for API calls</returns>
    Task<string> GetValidTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a new grant token from bKash
    /// </summary>
    /// <returns>Authentication response with new token</returns>
    Task<BkashAuthResponse> GrantTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing token using refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to use</param>
    /// <returns>Authentication response with refreshed token</returns>
    Task<BkashAuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the cached token (forces new grant on next request)
    /// </summary>
    void ClearCache();
}
