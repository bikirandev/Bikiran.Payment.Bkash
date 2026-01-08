using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Requests;

/// <summary>
/// Request model for creating a bKash payment
/// </summary>
public class BkashCreatePaymentRequest
{
    /// <summary>
    /// Transaction mode for bKash payment. Use constants from <see cref="BkashTransactionMode"/>
    /// Default: 0001 (Checkout)
    /// </summary>
    [JsonProperty("mode")]
    public string Mode { get; set; } = BkashTransactionMode.Checkout;

    /// <summary>
    /// Reference for the payer (optional)
    /// </summary>
    [JsonProperty("payerReference")]
    public string PayerReference { get; set; } = string.Empty;

    /// <summary>
    /// Callback URL for payment status
    /// </summary>
    [JsonProperty("callbackURL")]
    public string CallbackURL { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount (must be greater than 0)
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (default: BDT)
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; } = "BDT";

    /// <summary>
    /// Payment intent (default: sale)
    /// </summary>
    [JsonProperty("intent")]
    public string Intent { get; set; } = "sale";

    /// <summary>
    /// Unique merchant invoice number
    /// </summary>
    [JsonProperty("merchantInvoiceNumber")]
    public string MerchantInvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// Validates the request
    /// </summary>
    public void Validate()
    {
        if (Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0", nameof(Amount));

        if (string.IsNullOrWhiteSpace(MerchantInvoiceNumber))
            throw new ArgumentException("MerchantInvoiceNumber is required", nameof(MerchantInvoiceNumber));

        if (string.IsNullOrWhiteSpace(CallbackURL))
            throw new ArgumentException("CallbackURL is required", nameof(CallbackURL));
    }
}
