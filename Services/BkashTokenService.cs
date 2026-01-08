using Bikiran.Payment.Bkash.Configuration;
using Bikiran.Payment.Bkash.Exceptions;
using Bikiran.Payment.Bkash.Models.Requests;
using Bikiran.Payment.Bkash.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Bikiran.Payment.Bkash.Services;

/// <summary>
/// Implementation of bKash token service with automatic token management
/// </summary>
public class BkashTokenService : IBkashTokenService
{
    private readonly HttpClient _httpClient;
    private readonly BkashOptions _options;
    private readonly ILogger<BkashTokenService> _logger;

    private string? _cachedIdToken;
    private string? _cachedRefreshToken;
    private DateTime _tokenExpiryTime;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    public BkashTokenService(
        HttpClient httpClient,
        IOptions<BkashOptions> options,
        ILogger<BkashTokenService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options.Validate();
    }

    /// <inheritdoc/>
    public async Task<string> GetValidTokenAsync(CancellationToken cancellationToken = default)
    {
        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            // Check if we have a valid cached token
            if (!string.IsNullOrEmpty(_cachedIdToken) &&
                DateTime.UtcNow.AddSeconds(_options.TokenRefreshBufferSeconds) < _tokenExpiryTime)
            {
                _logger.LogDebug("Using cached bKash token");
                return _cachedIdToken;
            }

            // Try to refresh if we have a refresh token and it's not too old (within 28 days)
            if (!string.IsNullOrEmpty(_cachedRefreshToken))
            {
                try
                {
                    _logger.LogInformation("Refreshing bKash token");
                    var refreshResponse = await RefreshTokenAsync(_cachedRefreshToken, cancellationToken);

                    if (refreshResponse.IsSuccess)
                    {
                        CacheToken(refreshResponse);
                        return _cachedIdToken!;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Token refresh failed, will request new grant token");
                }
            }

            // Grant new token
            _logger.LogInformation("Granting new bKash token");
            var grantResponse = await GrantTokenAsync(cancellationToken);

            if (!grantResponse.IsSuccess)
            {
                throw new BkashAuthenticationException($"Failed to grant token: {grantResponse.StatusMessage}");
            }

            CacheToken(grantResponse);
            return _cachedIdToken!;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<BkashAuthResponse> GrantTokenAsync(CancellationToken cancellationToken = default)
    {
        var url = $"{_options.GetBaseUrl()}/v1.2.0-beta/tokenized/checkout/token/grant";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("username", _options.Username);
        request.Headers.Add("password", _options.Password);

        var authRequest = new BkashAuthRequest
        {
            AppKey = _options.AppKey,
            AppSecret = _options.AppSecret
        };

        var json = JsonConvert.SerializeObject(authRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("bKash grant token failed: {StatusCode} - {Response}",
                response.StatusCode, responseString);
            throw new BkashAuthenticationException(
                $"Grant token request failed: {response.StatusCode}",
                (int)response.StatusCode);
        }

        var authResponse = JsonConvert.DeserializeObject<BkashAuthResponse>(responseString);

        if (authResponse == null || !authResponse.IsSuccess)
        {
            throw new BkashAuthenticationException(
                $"Grant token failed: {authResponse?.StatusMessage ?? "Unknown error"}");
        }

        return authResponse;
    }

    /// <inheritdoc/>
    public async Task<BkashAuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

        var url = $"{_options.GetBaseUrl()}/v1.2.0-beta/tokenized/checkout/token/refresh";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("username", _options.Username);
        request.Headers.Add("password", _options.Password);

        var refreshRequest = new BkashRefreshTokenRequest
        {
            AppKey = _options.AppKey,
            AppSecret = _options.AppSecret,
            RefreshToken = refreshToken
        };

        var json = JsonConvert.SerializeObject(refreshRequest);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("bKash token refresh failed: {StatusCode} - {Response}",
                response.StatusCode, responseString);
            throw new BkashAuthenticationException(
                $"Refresh token request failed: {response.StatusCode}",
                (int)response.StatusCode);
        }

        var authResponse = JsonConvert.DeserializeObject<BkashAuthResponse>(responseString);

        if (authResponse == null || !authResponse.IsSuccess)
        {
            throw new BkashAuthenticationException(
                $"Token refresh failed: {authResponse?.StatusMessage ?? "Unknown error"}");
        }

        return authResponse;
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        _cachedIdToken = null;
        _cachedRefreshToken = null;
        _tokenExpiryTime = DateTime.MinValue;
        _logger.LogInformation("bKash token cache cleared");
    }

    private void CacheToken(BkashAuthResponse response)
    {
        _cachedIdToken = response.IdToken;
        _cachedRefreshToken = response.RefreshToken;

        // Set expiry time (typically 3600 seconds = 1 hour, but we use a buffer)
        var expirySeconds = response.ExpiresIn > 0 ? response.ExpiresIn : 3600;
        _tokenExpiryTime = DateTime.UtcNow.AddSeconds(expirySeconds);

        _logger.LogDebug("Cached bKash token, expires at {ExpiryTime}", _tokenExpiryTime);
    }
}
