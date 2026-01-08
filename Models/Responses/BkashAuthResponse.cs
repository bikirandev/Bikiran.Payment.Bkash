using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Responses;

/// <summary>
/// Response model from bKash token grant/refresh
/// </summary>
public class BkashAuthResponse
{
    /// <summary>
    /// Status code (0000 = success)
    /// </summary>
    [JsonProperty("statusCode")]
    public string StatusCode { get; set; } = string.Empty;

    /// <summary>
    /// Status message
    /// </summary>
    [JsonProperty("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// ID Token for API authentication
    /// </summary>
    [JsonProperty("id_token")]
    public string IdToken { get; set; } = string.Empty;

    /// <summary>
    /// Token type (typically "Bearer")
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; } = string.Empty;

    /// <summary>
    /// Token expiry in seconds (typically 3600 = 1 hour)
    /// </summary>
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Refresh token for obtaining new ID tokens
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Checks if the response indicates success
    /// </summary>
    public bool IsSuccess => StatusCode == "0000";
}
