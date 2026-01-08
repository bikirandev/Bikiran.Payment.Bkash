using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Responses;

/// <summary>
/// Response model from bKash execute payment API
/// </summary>
public class BkashExecutePaymentResponse
{
    /// <summary>
    /// Payment ID
    /// </summary>
    [JsonProperty("paymentID")]
    public string PaymentID { get; set; } = string.Empty;

    /// <summary>
    /// Agreement ID (for subscription payments)
    /// </summary>
    [JsonProperty("agreementID")]
    public string AgreementID { get; set; } = string.Empty;

    /// <summary>
    /// Customer mobile number
    /// </summary>
    [JsonProperty("customerMsisdn")]
    public string CustomerMsisdn { get; set; } = string.Empty;

    /// <summary>
    /// Payer reference
    /// </summary>
    [JsonProperty("payerReference")]
    public string PayerReference { get; set; } = string.Empty;

    /// <summary>
    /// Agreement execution time
    /// </summary>
    [JsonProperty("agreementExecuteTime")]
    public string AgreementExecuteTime { get; set; } = string.Empty;

    /// <summary>
    /// Agreement status
    /// </summary>
    [JsonProperty("agreementStatus")]
    public string AgreementStatus { get; set; } = string.Empty;

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
    /// Transaction status (e.g., "Completed")
    /// </summary>
    [JsonProperty("transactionStatus")]
    public string TransactionStatus { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount
    /// </summary>
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

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
    /// Checks if payment was completed successfully
    /// </summary>
    public bool IsCompleted => TransactionStatus == "Completed" && !string.IsNullOrEmpty(TrxID);
}
