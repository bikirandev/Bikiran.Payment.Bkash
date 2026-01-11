using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Requests;

/// <summary>
/// Request model for refunding a bKash payment
/// </summary>
public class BkashRefundPaymentRequest
{
    /// <summary>
    /// Payment ID to refund
    /// </summary>
    [JsonProperty("paymentId")]
    public string PaymentId { get; set; } = string.Empty;

    /// <summary>
    /// Transaction ID to refund
    /// </summary>
    [JsonProperty("trxId")]
    public string TrxId { get; set; } = string.Empty;

    /// <summary>
    /// Refund amount
    /// </summary>
    [JsonProperty("refundAmount")]
    public double RefundAmount { get; set; }

    /// <summary>
    /// SKU (max 255 characters) - Product/service related information
    /// </summary>
    [JsonProperty("sku")]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Refund reason (max 255 characters)
    /// </summary>
    [JsonProperty("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Validates the request
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(PaymentId))
            throw new ArgumentException("PaymentId is required", nameof(PaymentId));

        if (string.IsNullOrWhiteSpace(TrxId))
            throw new ArgumentException("TrxId is required", nameof(TrxId));

        if (RefundAmount <= 0)
            throw new ArgumentException("valid RefundAmount is required", nameof(RefundAmount));

        if (SKU?.Length > 255)
            throw new ArgumentException("SKU cannot exceed 255 characters", nameof(SKU));

        if (Reason?.Length > 255)
            throw new ArgumentException("Reason cannot exceed 255 characters", nameof(Reason));
    }
}
