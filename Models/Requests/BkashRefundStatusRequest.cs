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
}
