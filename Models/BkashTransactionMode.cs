namespace Bikiran.Payment.Bkash.Models;

/// <summary>
/// bKash transaction modes
/// </summary>
public static class BkashTransactionMode
{
    /// <summary>
    /// Checkout - One-time payment where customer pays merchant
    /// </summary>
    public const string Checkout = "0001";

    /// <summary>
    /// Pre-Authorization - Funds reserved, captured later
    /// </summary>
    public const string PreAuthorization = "0002";

    /// <summary>
    /// Agreement - Auto debit / subscription payments
    /// </summary>
    public const string Agreement = "0011";

    /// <summary>
    /// Disbursement - Merchant sends money to customer
    /// </summary>
    public const string Disbursement = "0021";

    /// <summary>
    /// Refund - Merchant refunds customer
    /// </summary>
    public const string Refund = "0031";
}
