namespace Bikiran.Payment.Bkash.Models.Endpoints;

/// <summary>
/// Standard endpoint response wrapper for bKash operations
/// </summary>
/// <typeparam name="T">Type of the data being returned</typeparam>
public class BkashEndpoint<T>
{
    /// <summary>
    /// Status of the operation (e.g., "success", "error")
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The actual data being returned
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Message describing the result of the operation
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
