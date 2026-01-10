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
public class BkashRefundPaymentResponse : BkashBaseResponse
{
    public string RefundTrxID { get; set; }      // Refund transaction ID
    public string OriginalTrxID { get; set; }    // Original transaction ID
    public string TransactionStatus { get; set; } // Refund status
    public double RefundAmount { get; set; }    // Refunded amount
    public DateTime CompletionTime { get; set; } // Refund completion time
    
    public bool IsCompleted => TransactionStatus == "Completed";
}
```

### Example Usage

```csharp
var request = new BkashRefundPaymentRequest
{
    PaymentId = "TR00111A2B3C4",
    TrxId = "8AS7D6F89G",
    RefundAmount = 500.00m,
    Sku = "PRODUCT-123",
    Reason = "Customer requested refund"
};

var response = await _bkashService.RefundPaymentAsync(request);

if (response.IsCompleted)
{
    Console.WriteLine($"Refund successful: {response.RefundTrxID}");
}
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
public class BkashRefundStatusResponse : BkashBaseResponse
{
    public string OriginalTrxId { get; set; }
    public double OriginalTrxAmount { get; set; }
    public List<RefundTransaction> RefundTransactions { get; set; }
}

public class RefundTransaction
{
    public string RefundTrxId { get; set; }
    public double Amount { get; set; }
    public string TransactionStatus { get; set; }
    public DateTime CompletionTime { get; set; }
}
```

### Example Usage

```csharp
var response = await _bkashService.QueryRefundStatusAsync(
    paymentId: "TR00111A2B3C4",
    trxId: "8AS7D6F89G"
);

Console.WriteLine($"Original Amount: {response.OriginalTrxAmount}");
Console.WriteLine($"Refunds: {response.RefundTransactions.Count}");

foreach (var refund in response.RefundTransactions)
{
    Console.WriteLine($"  {refund.RefundTrxId}: {refund.Amount} - {refund.TransactionStatus}");
}
```

## Best Practices

1. **Verify payment is completed** before attempting refund
2. **Store refund transaction IDs** for reconciliation
3. **Handle partial refunds** correctly (track total refunded)
4. **Implement retry logic** for failed refunds
5. **Query refund status** to confirm completion

## Common Errors

| Status Code | Description |
|-------------|-------------|
| `2023` | Insufficient balance for refund |
| `2053` | Invalid payment for refund |
| `2054` | Refund amount exceeds payment |
| `2055` | Payment already fully refunded |

## Next Steps

- 📖 [Payment Operations](payment-operations.md)
- 🔐 [Webhook Handling](webhook-handling.md)
- 📘 [Payment Flow Guide](../guides/payment-flow.md)
