using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Requests;

/// <summary>
/// Request model for bKash token grant
/// </summary>
public class BkashAuthRequest
{
    /// <summary>
    /// bKash Application Key
    /// </summary>
    [JsonProperty("app_key")]
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// bKash Application Secret
    /// </summary>
    [JsonProperty("app_secret")]
    public string AppSecret { get; set; } = string.Empty;
}

/// <summary>
/// Request model for bKash token refresh
/// </summary>
public class BkashRefreshTokenRequest
{
    /// <summary>
    /// bKash Application Key
    /// </summary>
    [JsonProperty("app_key")]
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// bKash Application Secret
    /// </summary>
    [JsonProperty("app_secret")]
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token to use for obtaining new ID token
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}
