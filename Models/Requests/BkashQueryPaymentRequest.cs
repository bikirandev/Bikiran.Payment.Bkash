using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Requests;

/// <summary>
/// Request model for querying bKash payment status
/// </summary>
public class BkashQueryPaymentRequest
{
    /// <summary>
    /// Payment ID to query
    /// </summary>
    [JsonProperty("paymentID")]
    public string PaymentID { get; set; } = string.Empty;

    /// <summary>
    /// Validates the request
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PaymentID))
            throw new ArgumentException("PaymentID is required", nameof(PaymentID));
    }
}
