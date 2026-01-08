using System.Security.Cryptography;
using System.Text;

namespace Bikiran.Payment.Bkash.Utilities;

/// <summary>
/// Helper class for verifying bKash webhook signatures
/// </summary>
public static class BkashWebhookHelper
{
    /// <summary>
    /// Verifies the signature of a bKash webhook request
    /// </summary>
    /// <param name="payload">Raw webhook payload (JSON string)</param>
    /// <param name="signature">Signature from webhook header</param>
    /// <param name="appSecret">Your bKash app secret</param>
    /// <returns>True if signature is valid</returns>
    public static bool VerifyWebhookSignature(string payload, string signature, string appSecret)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload cannot be null or empty", nameof(payload));

        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException("Signature cannot be null or empty", nameof(signature));

        if (string.IsNullOrWhiteSpace(appSecret))
            throw new ArgumentException("App secret cannot be null or empty", nameof(appSecret));

        var computedSignature = ComputeSignature(payload, appSecret);
        return string.Equals(computedSignature, signature, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Computes HMAC-SHA256 signature for webhook verification
    /// </summary>
    /// <param name="payload">Webhook payload</param>
    /// <param name="secret">Secret key</param>
    /// <returns>Computed signature</returns>
    public static string ComputeSignature(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Validates webhook timestamp to prevent replay attacks
    /// </summary>
    /// <param name="timestamp">Webhook timestamp</param>
    /// <param name="toleranceInSeconds">Acceptable time difference in seconds (default: 300 = 5 minutes)</param>
    /// <returns>True if timestamp is within tolerance</returns>
    public static bool IsTimestampValid(long timestamp, int toleranceInSeconds = 300)
    {
        var webhookTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        var currentTime = DateTimeOffset.UtcNow;
        var difference = Math.Abs((currentTime - webhookTime).TotalSeconds);

        return difference <= toleranceInSeconds;
    }
}
