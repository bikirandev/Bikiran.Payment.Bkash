using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Responses;

/// <summary>
/// Response model from bKash refund status query API
/// </summary>
public class BkashRefundStatusResponse
{
    /// <summary>
    /// Original transaction ID
    /// </summary>
    [JsonProperty("originalTrxId")]
    public string OriginalTrxId { get; set; } = string.Empty;

    /// <summary>
    /// Original transaction amount
    /// </summary>
    [JsonProperty("originalTrxAmount")]
    public double OriginalTrxAmount { get; set; }

    /// <summary>
    /// Original transaction completion time
    /// </summary>
    [JsonProperty("originalTrxCompletedTime")]
    public string OriginalTrxCompletedTime { get; set; } = string.Empty;

    /// <summary>
    /// List of refund transactions
    /// </summary>
    [JsonProperty("refundTransactions")]
    public List<BkashRefundTransaction> RefundTransactions { get; set; } = new();

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
    /// Indicates whether the transaction has been fully refunded
    /// </summary>
    public bool IsFullRefunded => RefundTransactions.Sum(r => r.RefundAmount) >= OriginalTrxAmount;

    /// <summary>
    /// Indicates whether there are any refund transactions
    /// </summary>
    public bool HavingRefund => RefundTransactions.Any();

    /// <summary>
    /// Remaining refundable amount from the original transaction
    /// </summary>
    public double RemainRefundAmount => OriginalTrxAmount - RefundTransactions.Sum(r => r.RefundAmount);
}

/// <summary>
/// Individual refund transaction details
/// </summary>
public class BkashRefundTransaction
{
    /// <summary>
    /// Refund transaction ID
    /// </summary>
    [JsonProperty("refundTrxId")]
    public string RefundTrxId { get; set; } = string.Empty;

    /// <summary>
    /// Refund transaction status
    /// </summary>
    [JsonProperty("refundTransactionStatus")]
    public string RefundTransactionStatus { get; set; } = string.Empty;

    /// <summary>
    /// Refund amount
    /// </summary>
    [JsonProperty("refundAmount")]
    public double RefundAmount { get; set; }

    /// <summary>
    /// Completion timestamp
    /// </summary>
    [JsonProperty("completedTime")]
    public string CompletedTime { get; set; } = string.Empty;
}
