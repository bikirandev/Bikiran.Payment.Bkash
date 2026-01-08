using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Responses;

/// <summary>
/// Response model from bKash refund payment API
/// </summary>
public class BkashRefundPaymentResponse
{
    /// <summary>
    /// Original transaction ID
    /// </summary>
    [JsonProperty("originalTrxId")]
    public string OriginalTrxId { get; set; } = string.Empty;

    /// <summary>
    /// Refund transaction ID
    /// </summary>
    [JsonProperty("refundTrxId")]
    public string RefundTrxID { get; set; } = string.Empty;

    /// <summary>
    /// Refund transaction status
    /// </summary>
    [JsonProperty("refundTransactionStatus")]
    public string RefundTransactionStatus { get; set; } = string.Empty;

    /// <summary>
    /// Original transaction amount
    /// </summary>
    [JsonProperty("originalTrxAmount")]
    public string OriginalTrxAmount { get; set; } = string.Empty;

    /// <summary>
    /// Refund amount
    /// </summary>
    [JsonProperty("refundAmount")]
    public string RefundAmount { get; set; } = string.Empty;

    /// <summary>
    /// Currency code
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Completion timestamp
    /// </summary>
    [JsonProperty("completedTime")]
    public string CompletedTime { get; set; } = string.Empty;

    /// <summary>
    /// SKU
    /// </summary>
    [JsonProperty("sku")]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Refund reason
    /// </summary>
    [JsonProperty("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Internal error code
    /// </summary>
    [JsonProperty("internalCode")]
    public string InternalCode { get; set; } = string.Empty;

    /// <summary>
    /// External error code from bKash
    /// </summary>
    [JsonProperty("externalCode")]
    public string ExternalCode { get; set; } = string.Empty;

    /// <summary>
    /// Error message in English
    /// </summary>
    [JsonProperty("errorMessageEn")]
    public string ErrorMessageEn { get; set; } = string.Empty;

    /// <summary>
    /// Error message in Bengali
    /// </summary>
    [JsonProperty("errorMessageBn")]
    public string ErrorMessageBn { get; set; } = string.Empty;

    /// <summary>
    /// Checks if refund was completed successfully
    /// </summary>
    public bool IsCompleted => RefundTransactionStatus == "Completed";
}
