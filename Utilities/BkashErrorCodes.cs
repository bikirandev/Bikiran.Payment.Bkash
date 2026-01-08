namespace Bikiran.Payment.Bkash.Utilities;

/// <summary>
/// Error code mappings and descriptions for bKash API
/// </summary>
public static class BkashErrorCodes
{
    /// <summary>
    /// Dictionary of bKash error codes and their descriptions
    /// </summary>
    public static readonly Dictionary<string, string> ErrorMessages = new()
    {
        { "0000", "Success" },
        { "2023", "Insufficient Balance" },
        { "2029", "Duplicate Transaction" },
        { "2071", "Refund after specified days not allowed" },
        { "2072", "Refund amount not valid" },
        { "2073", "Invalid SKU" },
        { "2074", "The transaction cannot be reversed" },
        { "2075", "SKU Character Limit Exceeded" },
        { "2076", "Reason Character Limit Exceeded" },
        { "2077", "Invalid TrxID" },
        { "2078", "Invalid Reason" },
        { "2079", "Invalid app Token" },
        { "2080", "The identity is not permitted to initiate this transaction" },
        { "2081", "The identity of the debit or credit party is in a state which prohibits the execution of this transaction" },
        { "2082", "The merchant is not permitted to initiate this transaction" },
        { "2127", "Transaction not yet completed" }
    };

    /// <summary>
    /// Gets the error message for a given error code
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <returns>Error message or "Unknown Error" if code not found</returns>
    public static string GetErrorMessage(string errorCode)
    {
        return ErrorMessages.TryGetValue(errorCode, out var message)
            ? message
            : "Unknown Error";
    }

    /// <summary>
    /// Checks if an error code indicates success
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <returns>True if error code is 0000</returns>
    public static bool IsSuccess(string errorCode)
    {
        return errorCode == "0000";
    }

    /// <summary>
    /// Checks if an error code exists in the known error codes
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <returns>True if error code is known</returns>
    public static bool IsKnownErrorCode(string errorCode)
    {
        return ErrorMessages.ContainsKey(errorCode);
    }
}
