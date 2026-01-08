using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Responses;

/// <summary>
/// Response model from bKash create payment API
/// </summary>
public class BkashCreatePaymentResponse
{
    /// <summary>
    /// Payment ID for this transaction
    /// </summary>
    [JsonProperty("paymentID")]
    public string PaymentID { get; set; } = string.Empty;

    /// <summary>
    /// Agreement ID (for subscription payments)
    /// </summary>
    [JsonProperty("agreementID")]
    public string AgreementID { get; set; } = string.Empty;

    /// <summary>
    /// Payment creation timestamp
    /// </summary>
    [JsonProperty("paymentCreateTime")]
    public string PaymentCreateTime { get; set; } = string.Empty;

    /// <summary>
    /// Current transaction status
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
    /// bKash payment URL for customer to complete payment
    /// </summary>
    [JsonProperty("bkashURL")]
    public string BkashURL { get; set; } = string.Empty;

    /// <summary>
    /// Callback URL
    /// </summary>
    [JsonProperty("callbackURL")]
    public string CallbackURL { get; set; } = string.Empty;

    /// <summary>
    /// Success callback URL
    /// </summary>
    [JsonProperty("successCallbackURL")]
    public string SuccessCallbackURL { get; set; } = string.Empty;

    /// <summary>
    /// Failure callback URL
    /// </summary>
    [JsonProperty("failureCallbackURL")]
    public string FailureCallbackURL { get; set; } = string.Empty;

    /// <summary>
    /// Cancelled callback URL
    /// </summary>
    [JsonProperty("cancelledCallbackURL")]
    public string CancelledCallbackURL { get; set; } = string.Empty;

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
    /// Checks if the response indicates success
    /// </summary>
    public bool IsSuccess => StatusCode == "0000" && !string.IsNullOrEmpty(PaymentID);
}
