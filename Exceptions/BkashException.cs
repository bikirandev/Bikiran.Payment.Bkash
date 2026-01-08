namespace Bikiran.Payment.Bkash.Exceptions;

/// <summary>
/// Base exception for bKash payment gateway errors
/// </summary>
public class BkashException : Exception
{
    /// <summary>
    /// bKash error code
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// HTTP status code (if applicable)
    /// </summary>
    public int? HttpStatusCode { get; set; }

    public BkashException(string message) : base(message)
    {
        ErrorCode = "UNKNOWN";
    }

    public BkashException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public BkashException(string message, string errorCode, int httpStatusCode) : base(message)
    {
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
    }

    public BkashException(string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = "UNKNOWN";
    }
}

/// <summary>
/// Exception for bKash authentication failures
/// </summary>
public class BkashAuthenticationException : BkashException
{
    public BkashAuthenticationException(string message) : base(message, "AUTH_FAILED") { }
    public BkashAuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    public BkashAuthenticationException(string message, int httpStatusCode) : base(message, "AUTH_FAILED", httpStatusCode) { }
}

/// <summary>
/// Exception for bKash payment operation failures
/// </summary>
public class BkashPaymentException : BkashException
{
    public BkashPaymentException(string message, string errorCode) : base(message, errorCode) { }
    public BkashPaymentException(string message, string errorCode, int httpStatusCode) : base(message, errorCode, httpStatusCode) { }
}

/// <summary>
/// Exception for bKash configuration errors
/// </summary>
public class BkashConfigurationException : BkashException
{
    public BkashConfigurationException(string message) : base(message, "CONFIG_ERROR") { }
}
