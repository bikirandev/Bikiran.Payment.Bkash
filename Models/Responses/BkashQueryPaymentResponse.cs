using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Responses;

/// <summary>
/// Response model from bKash query payment status API
/// </summary>
public class BkashQueryPaymentResponse
{
    /// <summary>
    /// Payment ID
    /// </summary>
    [JsonProperty("paymentID")]
    public string PaymentID { get; set; } = string.Empty;

    /// <summary>
    /// Transaction mode
    /// </summary>
    [JsonProperty("mode")]
    public string Mode { get; set; } = string.Empty;

    /// <summary>
    /// Payer reference
    /// </summary>
    [JsonProperty("payerReference")]
    public string PayerReference { get; set; } = string.Empty;

    /// <summary>
    /// Payment creation timestamp
    /// </summary>
    [JsonProperty("paymentCreateTime")]
    public string PaymentCreateTime { get; set; } = string.Empty;

    /// <summary>
    /// Payment execution timestamp
    /// </summary>
    [JsonProperty("paymentExecuteTime")]
    public string PaymentExecuteTime { get; set; } = string.Empty;

    /// <summary>
    /// bKash transaction ID
    /// </summary>
    [JsonProperty("trxID")]
    public string TrxID { get; set; } = string.Empty;

    /// <summary>
    /// Transaction status
    /// </summary>
    [JsonProperty("transactionStatus")]
    public string TransactionStatus { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount
    /// </summary>
    [JsonProperty("amount")]
    public double Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Payment intent
    /// </summary>
    [JsonProperty("intent")]
    public string Intent { get; set; } = string.Empty;

    /// <summary>
    /// Merchant invoice number
    /// </summary>
    [JsonProperty("merchantInvoiceNumber")]
    public string MerchantInvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// User verification status
    /// </summary>
    [JsonProperty("userVerificationStatus")]
    public string UserVerificationStatus { get; set; } = string.Empty;

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
    /// Error code (if any)
    /// </summary>
    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Error message (if any)
    /// </summary>
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Checks if payment was completed successfully
    /// </summary>
    public bool IsPaymentCompletedSuccessfully => TransactionStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) && StatusCode == "0000";
}
