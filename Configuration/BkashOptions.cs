namespace Bikiran.Payment.Bkash.Configuration;

/// <summary>
/// Configuration options for bKash payment gateway integration
/// </summary>
public class BkashOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json
    /// </summary>
    public const string SectionName = "Bkash";

    /// <summary>
    /// bKash Application Key
    /// </summary>
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// bKash Application Secret
    /// </summary>
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// bKash Merchant Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// bKash Merchant Password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// bKash API environment (Sandbox or Production)
    /// </summary>
    public BkashEnvironment Environment { get; set; } = BkashEnvironment.Sandbox;

    /// <summary>
    /// Custom base URL for bKash API (optional, overrides environment setting)
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// HTTP client timeout in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Token refresh buffer in seconds (default: 5 minutes before expiry)
    /// </summary>
    public int TokenRefreshBufferSeconds { get; set; } = 300;

    /// <summary>
    /// Gets the base API URL based on environment
    /// </summary>
    public string GetBaseUrl()
    {
        if (!string.IsNullOrEmpty(BaseUrl))
            return BaseUrl;

        return Environment switch
        {
            BkashEnvironment.Production => "https://tokenized.pay.bka.sh",
            BkashEnvironment.Sandbox => "https://tokenized.sandbox.bka.sh",
            _ => "https://tokenized.sandbox.bka.sh"
        };
    }

    /// <summary>
    /// Validates the configuration options
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AppKey))
            throw new ArgumentException("AppKey is required", nameof(AppKey));

        if (string.IsNullOrWhiteSpace(AppSecret))
            throw new ArgumentException("AppSecret is required", nameof(AppSecret));

        if (string.IsNullOrWhiteSpace(Username))
            throw new ArgumentException("Username is required", nameof(Username));

        if (string.IsNullOrWhiteSpace(Password))
            throw new ArgumentException("Password is required", nameof(Password));

        if (TimeoutSeconds <= 0)
            throw new ArgumentException("TimeoutSeconds must be greater than 0", nameof(TimeoutSeconds));
    }
}

/// <summary>
/// bKash API environment
/// </summary>
public enum BkashEnvironment
{
    /// <summary>
    /// Sandbox/Test environment
    /// </summary>
    Sandbox = 0,

    /// <summary>
    /// Production environment
    /// </summary>
    Production = 1
}
