using Bikiran.Payment.Bkash.Models.Requests;
using Bikiran.Payment.Bkash.Models.Responses;

namespace Bikiran.Payment.Bkash.Services;

/// <summary>
/// Interface for bKash payment operations
/// </summary>
public interface IBkashPaymentService
{
    /// <summary>
    /// Creates a new payment in bKash
    /// </summary>
    /// <param name="request">Payment creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment creation response with payment URL</returns>
    Task<BkashCreatePaymentResponse> CreatePaymentAsync(
        BkashCreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a payment after customer authorization
    /// </summary>
    /// <param name="paymentId">Payment ID to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment execution response</returns>
    Task<BkashExecutePaymentResponse> ExecutePaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the status of a payment
    /// </summary>
    /// <param name="paymentId">Payment ID to query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment status response</returns>
    Task<BkashQueryPaymentResponse> QueryPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refunds a completed payment
    /// </summary>
    /// <param name="request">Refund request details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refund response</returns>
    Task<BkashRefundPaymentResponse> RefundPaymentAsync(
        BkashRefundPaymentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the status of a refund
    /// </summary>
    /// <param name="paymentId">Original payment ID</param>
    /// <param name="trxId">Original transaction ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refund status response</returns>
    Task<BkashRefundStatusResponse> QueryRefundStatusAsync(
        string paymentId,
        string trxId,
        CancellationToken cancellationToken = default);
}
