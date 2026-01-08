using Bikiran.Payment.Bkash.Configuration;
using Bikiran.Payment.Bkash.Exceptions;
using Bikiran.Payment.Bkash.Models.Requests;
using Bikiran.Payment.Bkash.Models.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Bikiran.Payment.Bkash.Services;

/// <summary>
/// Implementation of bKash payment service
/// </summary>
public class BkashPaymentService : IBkashPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly BkashOptions _options;
    private readonly IBkashTokenService _tokenService;
    private readonly ILogger<BkashPaymentService> _logger;

    public BkashPaymentService(
        HttpClient httpClient,
        IOptions<BkashOptions> options,
        IBkashTokenService tokenService,
        ILogger<BkashPaymentService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<BkashCreatePaymentResponse> CreatePaymentAsync(
        BkashCreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Validate();

        var token = await _tokenService.GetValidTokenAsync(cancellationToken);
        var url = $"{_options.GetBaseUrl()}/v1.2.0-beta/tokenized/checkout/create";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Authorization", token);
        httpRequest.Headers.Add("X-APP-Key", _options.AppKey);

        var json = JsonConvert.SerializeObject(request);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Creating bKash payment for invoice {InvoiceNumber}",
            request.MerchantInvoiceNumber);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("bKash create payment failed: {StatusCode} - {Response}",
                response.StatusCode, responseString);
            throw new BkashPaymentException(
                $"Create payment request failed: {response.StatusCode}",
                "CREATE_PAYMENT_FAILED",
                (int)response.StatusCode);
        }

        var paymentResponse = JsonConvert.DeserializeObject<BkashCreatePaymentResponse>(responseString);

        if (paymentResponse == null || !paymentResponse.IsSuccess)
        {
            throw new BkashPaymentException(
                $"Create payment failed: {paymentResponse?.StatusMessage ?? "Unknown error"}",
                paymentResponse?.ErrorCode ?? "UNKNOWN");
        }

        _logger.LogInformation("bKash payment created successfully: {PaymentId}",
            paymentResponse.PaymentID);

        return paymentResponse;
    }

    /// <inheritdoc/>
    public async Task<BkashExecutePaymentResponse> ExecutePaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);

        var token = await _tokenService.GetValidTokenAsync(cancellationToken);
        var url = $"{_options.GetBaseUrl()}/v1.2.0-beta/tokenized/checkout/execute";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Authorization", token);
        httpRequest.Headers.Add("X-APP-Key", _options.AppKey);

        var executeRequest = new BkashExecutePaymentRequest
        {
            PaymentID = paymentId
        };

        var json = JsonConvert.SerializeObject(executeRequest);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Executing bKash payment: {PaymentId}", paymentId);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("bKash execute payment failed: {StatusCode} - {Response}",
                response.StatusCode, responseString);
            throw new BkashPaymentException(
                $"Execute payment request failed: {response.StatusCode}",
                "EXECUTE_PAYMENT_FAILED",
                (int)response.StatusCode);
        }

        var executeResponse = JsonConvert.DeserializeObject<BkashExecutePaymentResponse>(responseString);

        if (executeResponse == null)
        {
            throw new BkashPaymentException(
                "Failed to deserialize execute payment response",
                "DESERIALIZATION_FAILED");
        }

        if (executeResponse.IsCompleted)
        {
            _logger.LogInformation("bKash payment executed successfully: {PaymentId}, TrxID: {TrxId}",
                paymentId, executeResponse.TrxID);
        }
        else
        {
            _logger.LogWarning("bKash payment execution incomplete: {PaymentId}, Status: {Status}",
                paymentId, executeResponse.TransactionStatus);
        }

        return executeResponse;
    }

    /// <inheritdoc/>
    public async Task<BkashQueryPaymentResponse> QueryPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);

        var token = await _tokenService.GetValidTokenAsync(cancellationToken);
        var url = $"{_options.GetBaseUrl()}/v1.2.0-beta/tokenized/checkout/payment/status";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Authorization", token);
        httpRequest.Headers.Add("X-APP-Key", _options.AppKey);

        var queryRequest = new BkashQueryPaymentRequest
        {
            PaymentID = paymentId
        };

        var json = JsonConvert.SerializeObject(queryRequest);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogDebug("Querying bKash payment status: {PaymentId}", paymentId);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var queryResponse = JsonConvert.DeserializeObject<BkashQueryPaymentResponse>(responseString)
            ?? new BkashQueryPaymentResponse();

        return queryResponse;
    }

    /// <inheritdoc/>
    public async Task<BkashRefundPaymentResponse> RefundPaymentAsync(
        BkashRefundPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Validate();

        var token = await _tokenService.GetValidTokenAsync(cancellationToken);
        var url = $"{_options.GetBaseUrl()}/v2/tokenized-checkout/refund/payment/transaction";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Authorization", token);
        httpRequest.Headers.Add("X-App-Key", _options.AppKey);

        var json = JsonConvert.SerializeObject(request);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Processing bKash refund: PaymentId={PaymentId}, TrxId={TrxId}, Amount={Amount}",
            request.PaymentId, request.TrxId, request.RefundAmount);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var refundResponse = JsonConvert.DeserializeObject<BkashRefundPaymentResponse>(responseString)
            ?? new BkashRefundPaymentResponse();

        if (refundResponse.IsCompleted)
        {
            _logger.LogInformation("bKash refund completed: RefundTrxId={RefundTrxId}",
                refundResponse.RefundTrxID);
        }
        else if (!string.IsNullOrEmpty(refundResponse.ExternalCode))
        {
            _logger.LogWarning("bKash refund failed: Code={Code}, Message={Message}",
                refundResponse.ExternalCode, refundResponse.ErrorMessageEn);
        }

        return refundResponse;
    }

    /// <inheritdoc/>
    public async Task<BkashRefundStatusResponse> QueryRefundStatusAsync(
        string paymentId,
        string trxId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(trxId);

        var token = await _tokenService.GetValidTokenAsync(cancellationToken);
        var url = $"{_options.GetBaseUrl()}/v2/tokenized-checkout/refund/payment/status";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Accept", "application/json");
        httpRequest.Headers.Add("Authorization", token);
        httpRequest.Headers.Add("X-APP-Key", _options.AppKey);

        var queryRequest = new BkashRefundStatusRequest
        {
            PaymentId = paymentId,
            TrxId = trxId
        };

        var json = JsonConvert.SerializeObject(queryRequest);
        httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogDebug("Querying bKash refund status: PaymentId={PaymentId}, TrxId={TrxId}",
            paymentId, trxId);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        var statusResponse = JsonConvert.DeserializeObject<BkashRefundStatusResponse>(responseString)
            ?? new BkashRefundStatusResponse();

        return statusResponse;
    }
}
