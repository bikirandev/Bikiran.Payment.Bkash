using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Requests;

/// <summary>
/// Request model for querying bKash refund status
/// </summary>
public class BkashRefundStatusRequest
{
    /// <summary>
    /// Payment ID
    /// </summary>
    [JsonProperty("paymentId")]
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// Transaction ID
    /// </summary>
    [JsonProperty("trxId")]
    public string TrxId { get; set; } = string.Empty;

    /// <summary>
    /// Validates the request
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PaymentId))
            throw new ArgumentException("PaymentId is required", nameof(PaymentId));

        if (string.IsNullOrWhiteSpace(TrxId))
            throw new ArgumentException("TrxId is required", nameof(TrxId));
    }
}
