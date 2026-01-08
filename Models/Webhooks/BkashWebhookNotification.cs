using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Webhooks;

/// <summary>
/// Webhook notification model from bKash
/// </summary>
public class BkashWebhookNotification
{
    /// <summary>
    /// Payment ID
    /// </summary>
    [JsonProperty("paymentID")]
    public string PaymentID { get; set; } = string.Empty;

    /// <summary>
    /// Transaction ID
    /// </summary>
    [JsonProperty("trxID")]
    public string TrxID { get; set; } = string.Empty;

    /// <summary>
    /// Transaction status (e.g., Completed, Cancelled)
    /// </summary>
    [JsonProperty("transactionStatus")]
    public string TransactionStatus { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount
    /// </summary>
    [JsonProperty("amount")]
    public string Amount { get; set; } = string.Empty;

    /// <summary>
    /// Currency (e.g., BDT)
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Merchant invoice number
    /// </summary>
    [JsonProperty("merchantInvoiceNumber")]
    public string MerchantInvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Payment execution time
    /// </summary>
    [JsonProperty("paymentExecuteTime")]
    public string PaymentExecuteTime { get; set; } = string.Empty;

    /// <summary>
    /// Customer MSISDN
    /// </summary>
    [JsonProperty("customerMsisdn")]
    public string CustomerMsisdn { get; set; } = string.Empty;

    /// <summary>
    /// Webhook event type
    /// </summary>
    [JsonProperty("eventType")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the webhook event
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }
}
