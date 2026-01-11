# Refund Operations API Reference

Complete reference for processing and tracking refunds.

## Overview

The Bikiran.Payment.Bkash library supports full and partial refunds for completed payments.

## RefundPaymentAsync

Process a refund for a completed payment.

### Method Signature

```csharp
Task<BkashRefundPaymentResponse> RefundPaymentAsync(
    BkashRefundPaymentRequest request,
    CancellationToken cancellationToken = default)
```

### Request Model

```csharp
public class BkashRefundPaymentRequest
{
    public string PaymentId { get; set; }        // Required: Original payment ID
    public string TrxId { get; set; }            // Required: Original transaction ID
    public double RefundAmount { get; set; }    // Required: Amount to refund
    public string Sku { get; set; }              // Optional: Product/Order SKU
    public string Reason { get; set; }           // Optional: Refund reason
}
```

### Response Model

```csharp
public class BkashRefundPaymentResponse
{
    public string OriginalTrxId { get; set; }            // Original transaction ID
    public string RefundTrxID { get; set; }              // Refund transaction ID
    public string RefundTransactionStatus { get; set; }  // Refund transaction status
    public double OriginalTrxAmount { get; set; }        // Original transaction amount
    public double RefundAmount { get; set; }             // Refund amount
    public string Currency { get; set; }                 // Currency code
    public string CompletedTime { get; set; }            // Completion timestamp
    public string SKU { get; set; }                      // SKU
    public string Reason { get; set; }                   // Refund reason
    public string InternalCode { get; set; }             // Internal error code
    public string ExternalCode { get; set; }             // External error code from bKash
    public string ErrorMessageEn { get; set; }           // Error message in English
    public string ErrorMessageBn { get; set; }           // Error message in Bengali

    public bool IsCompleted => RefundTransactionStatus == "Completed";
}
```

### Example Usage

#### Basic Example

```csharp
var request = new BkashRefundPaymentRequest
{
    PaymentId = "TR00111A2B3C4",
    TrxId = "8AS7D6F89G",
    RefundAmount = 500.00,
    Sku = "PRODUCT-123",
    Reason = "Customer requested refund"
};

var response = await _bkashService.RefundPaymentAsync(request);

if (response.IsCompleted)
{
    Console.WriteLine($"Refund successful: {response.RefundTrxID}");
}
```

#### Using BkashEndpoint Wrapper

```csharp
using Bikiran.Payment.Bkash.Models.Endpoints;

[HttpPost("refund")]
public async Task<ActionResult<BkashEndpoint<BkashRefundPaymentResponse>>> ProcessRefund(
    [FromBody] RefundRequest request)
{
    try
    {
        var bkashRequest = new BkashRefundPaymentRequest
        {
            PaymentId = request.PaymentId,
            TrxId = request.TransactionId,
            RefundAmount = request.Amount,
            Sku = request.OrderId,
            Reason = request.Reason
        };

        var response = await _bkashService.RefundPaymentAsync(bkashRequest);

        return Ok(new BkashEndpoint<BkashRefundPaymentResponse>
        {
            Status = response.IsCompleted ? "success" : "error",
            Data = response,
            Message = response.IsCompleted
                ? $"Refund processed successfully. Transaction ID: {response.RefundTrxID}"
                : "Refund processing failed"
        });
    }
    catch (BkashException ex)
    {
        return BadRequest(new BkashEndpoint<BkashRefundPaymentResponse>
        {
            Status = "error",
            Data = null,
            Message = $"Refund failed: {ex.Message}"
        });
    }
}

public record RefundRequest(
    string PaymentId,
    string TransactionId,
    double Amount,
    string OrderId,
    string Reason
);
```

### Refund Types

- **Full Refund**: RefundAmount equals original payment amount
- **Partial Refund**: RefundAmount is less than original amount

### Validation Rules

- Payment must be in "Completed" status
- RefundAmount must not exceed original payment amount
- Multiple partial refunds allowed (total cannot exceed original)

## QueryRefundStatusAsync

Query the status and details of a refund.

### Method Signature

```csharp
Task<BkashRefundStatusResponse> QueryRefundStatusAsync(
    string paymentId,
    string trxId,
    CancellationToken cancellationToken = default)
```

### Parameters

- `paymentId`: Original payment ID
- `trxId`: Original transaction ID

### Response Model

```csharp
public class BkashRefundStatusResponse
{
    public string OriginalTrxId { get; set; }                         // Original transaction ID
    public double OriginalTrxAmount { get; set; }                     // Original transaction amount
    public string OriginalTrxCompletedTime { get; set; }              // Original transaction completion time
    public List<BkashRefundTransaction> RefundTransactions { get; set; } // List of refund transactions
    public string InternalCode { get; set; }                          // Internal error code
    public string ExternalCode { get; set; }                          // External error code from bKash
    public string ErrorMessageEn { get; set; }                        // Error message in English
    public string ErrorMessageBn { get; set; }                        // Error message in Bengali

    // Computed Properties
    public bool IsFullRefunded { get; }                               // Whether transaction is fully refunded
    public bool HavingRefund { get; }                                 // Whether there are any refund transactions
    public double RemainRefundAmount { get; }                         // Remaining refundable amount
}

public class BkashRefundTransaction
{
    public string RefundTrxId { get; set; }              // Refund transaction ID
    public string RefundTransactionStatus { get; set; }  // Refund transaction status
    public double RefundAmount { get; set; }             // Refund amount
    public string CompletedTime { get; set; }            // Completion timestamp
}
```

### Example Usage

```csharp
var response = await _bkashService.QueryRefundStatusAsync(
    paymentId: "TR00111A2B3C4",
    trxId: "8AS7D6F89G"
);

Console.WriteLine($"Original Amount: {response.OriginalTrxAmount}");
Console.WriteLine($"Original Completed Time: {response.OriginalTrxCompletedTime}");
Console.WriteLine($"Refunds: {response.RefundTransactions.Count}");
Console.WriteLine($"Is Fully Refunded: {response.IsFullRefunded}");
Console.WriteLine($"Has Refunds: {response.HavingRefund}");
Console.WriteLine($"Remaining Refundable Amount: {response.RemainRefundAmount}");

foreach (var refund in response.RefundTransactions)
{
    Console.WriteLine($"  {refund.RefundTrxId}: {refund.RefundAmount} - {refund.RefundTransactionStatus}");
    Console.WriteLine($"    Completed: {refund.CompletedTime}");
}
```

## Best Practices

1. **Verify payment is completed** before attempting refund
2. **Store refund transaction IDs** for reconciliation
3. **Handle partial refunds** correctly (track total refunded)
4. **Implement retry logic** for failed refunds
5. **Query refund status** to confirm completion

## Common Errors

| Status Code | Description                     |
| ----------- | ------------------------------- |
| `2023`      | Insufficient balance for refund |
| `2053`      | Invalid payment for refund      |
| `2054`      | Refund amount exceeds payment   |
| `2055`      | Payment already fully refunded  |

## Next Steps

- 📖 [Payment Operations](payment-operations.md)
- 🔐 [Webhook Handling](webhook-handling.md)
- 📘 [Payment Flow Guide](../guides/payment-flow.md)
