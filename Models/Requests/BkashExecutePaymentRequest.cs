using Newtonsoft.Json;

namespace Bikiran.Payment.Bkash.Models.Requests;

/// <summary>
/// Request model for executing a bKash payment
/// </summary>
public class BkashExecutePaymentRequest
{
    /// <summary>
    /// Payment ID returned from create payment API
    /// </summary>
    [JsonProperty("paymentID")]
    public string PaymentID { get; set; } = string.Empty;
}
