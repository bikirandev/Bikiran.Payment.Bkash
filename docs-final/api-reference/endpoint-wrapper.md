# BkashEndpoint Wrapper API Reference

A generic wrapper class for standardizing API responses in your application.

## Overview

`BkashEndpoint<T>` is a generic wrapper class that provides a consistent response structure for all bKash operations. It helps maintain a uniform API contract across your endpoints.

## Class Definition

```csharp
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
```

## Properties

| Property  | Type     | Description                                                     |
| --------- | -------- | --------------------------------------------------------------- |
| `Status`  | `string` | Indicates operation result: "success", "error", "pending", etc. |
| `Data`    | `T?`     | The actual response data (nullable)                             |
| `Message` | `string` | Human-readable message describing the result                    |

## Usage Examples

### Payment Creation

```csharp
using Bikiran.Payment.Bkash.Models.Endpoints;
using Bikiran.Payment.Bkash.Models.Requests;
using Bikiran.Payment.Bkash.Models.Responses;

[HttpPost("payment/create")]
public async Task<ActionResult<BkashEndpoint<BkashCreatePaymentResponse>>> CreatePayment(
    [FromBody] CreatePaymentDto dto)
{
    try
    {
        var request = new BkashCreatePaymentRequest
        {
            Amount = dto.Amount,
            MerchantInvoiceNumber = dto.InvoiceNumber,
            PayerReference = dto.CustomerPhone,
            Intent = "sale"
        };

        var response = await _bkashService.CreatePaymentAsync(request);

        return Ok(new BkashEndpoint<BkashCreatePaymentResponse>
        {
            Status = "success",
            Data = response,
            Message = "Payment created successfully. Redirect customer to Data.BkashURL"
        });
    }
    catch (BkashException ex)
    {
        return BadRequest(new BkashEndpoint<BkashCreatePaymentResponse>
        {
            Status = "error",
            Data = null,
            Message = $"Payment creation failed: {ex.Message}"
        });
    }
}
```

### Payment Execution

```csharp
[HttpPost("payment/execute/{paymentId}")]
public async Task<ActionResult<BkashEndpoint<BkashExecutePaymentResponse>>> ExecutePayment(
    string paymentId)
{
    try
    {
        var response = await _bkashService.ExecutePaymentAsync(paymentId);

        if (response.IsCompleted)
        {
            return Ok(new BkashEndpoint<BkashExecutePaymentResponse>
            {
                Status = "success",
                Data = response,
                Message = $"Payment completed. Transaction ID: {response.TrxID}"
            });
        }

        return BadRequest(new BkashEndpoint<BkashExecutePaymentResponse>
        {
            Status = "error",
            Data = response,
            Message = "Payment execution failed"
        });
    }
    catch (BkashException ex)
    {
        return BadRequest(new BkashEndpoint<BkashExecutePaymentResponse>
        {
            Status = "error",
            Data = null,
            Message = ex.Message
        });
    }
}
```

### Query Payment Status

```csharp
[HttpGet("payment/status/{paymentId}")]
public async Task<ActionResult<BkashEndpoint<BkashQueryPaymentResponse>>> GetPaymentStatus(
    string paymentId)
{
    try
    {
        var response = await _bkashService.QueryPaymentAsync(paymentId);

        return Ok(new BkashEndpoint<BkashQueryPaymentResponse>
        {
            Status = "success",
            Data = response,
            Message = $"Payment status: {response.TransactionStatus}"
        });
    }
    catch (BkashException ex)
    {
        return NotFound(new BkashEndpoint<BkashQueryPaymentResponse>
        {
            Status = "error",
            Data = null,
            Message = $"Payment not found: {ex.Message}"
        });
    }
}
```

### Refund Processing

```csharp
[HttpPost("refund")]
public async Task<ActionResult<BkashEndpoint<BkashRefundPaymentResponse>>> ProcessRefund(
    [FromBody] RefundDto dto)
{
    try
    {
        var request = new BkashRefundPaymentRequest
        {
            PaymentId = dto.PaymentId,
            TrxId = dto.TransactionId,
            RefundAmount = dto.Amount,
            Sku = dto.OrderId,
            Reason = dto.Reason
        };

        var response = await _bkashService.RefundPaymentAsync(request);

        return Ok(new BkashEndpoint<BkashRefundPaymentResponse>
        {
            Status = response.IsCompleted ? "success" : "error",
            Data = response,
            Message = response.IsCompleted
                ? $"Refund completed. Refund ID: {response.RefundTrxID}"
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
```

## Status Values

While you can use any status values that fit your application, here are common conventions:

| Status               | Usage                            |
| -------------------- | -------------------------------- |
| `"success"`          | Operation completed successfully |
| `"error"`            | Operation failed                 |
| `"pending"`          | Operation is in progress         |
| `"partial_success"`  | Operation partially completed    |
| `"validation_error"` | Request validation failed        |

## Response Examples

### Success Response

```json
{
  "status": "success",
  "data": {
    "paymentID": "TR00111A2B3C4D5E6",
    "bkashURL": "https://checkout.sandbox.bka.sh/...",
    "amount": 1000.0,
    "intent": "sale",
    "merchantInvoiceNumber": "INV-2024-001"
  },
  "message": "Payment created successfully. Redirect customer to Data.BkashURL"
}
```

### Error Response

```json
{
  "status": "error",
  "data": null,
  "message": "Payment creation failed: Invalid credentials"
}
```

### Partial Success Response

```json
{
  "status": "partial_success",
  "data": {
    "transactionStatus": "Initiated",
    "paymentID": "TR00111A2B3C4D5E6"
  },
  "message": "Payment initiated but not yet completed"
}
```

## Best Practices

### 1. Consistent Status Values

Define status constants for consistency:

```csharp
public static class ResponseStatus
{
    public const string Success = "success";
    public const string Error = "error";
    public const string Pending = "pending";
    public const string ValidationError = "validation_error";
}

// Usage
return Ok(new BkashEndpoint<BkashCreatePaymentResponse>
{
    Status = ResponseStatus.Success,
    Data = response,
    Message = "Payment created successfully"
});
```

### 2. Descriptive Messages

Provide clear, actionable messages:

```csharp
// Good
Message = "Payment created. Redirect customer to Data.BkashURL within 5 minutes."

// Bad
Message = "Success"
```

### 3. Include Data Even on Errors

When possible, include partial data in error responses:

```csharp
catch (BkashPaymentException ex)
{
    return BadRequest(new BkashEndpoint<BkashCreatePaymentResponse>
    {
        Status = "error",
        Data = new BkashCreatePaymentResponse
        {
            StatusCode = ex.ErrorCode,
            StatusMessage = ex.Message
        },
        Message = $"Payment failed: {ex.Message}"
    });
}
```

### 4. Use Appropriate HTTP Status Codes

Match HTTP status codes with wrapper status:

```csharp
// 200 OK for success
return Ok(new BkashEndpoint<T> { Status = "success", ... });

// 400 Bad Request for validation/business errors
return BadRequest(new BkashEndpoint<T> { Status = "error", ... });

// 404 Not Found when resource doesn't exist
return NotFound(new BkashEndpoint<T> { Status = "error", ... });

// 500 Internal Server Error for unexpected errors
return StatusCode(500, new BkashEndpoint<T> { Status = "error", ... });
```

### 5. Type Safety

Leverage generic type parameter for IntelliSense support:

```csharp
// Client code gets full IntelliSense for response.Data
var result = await httpClient.GetFromJsonAsync<BkashEndpoint<BkashQueryPaymentResponse>>(url);
if (result.Status == "success" && result.Data != null)
{
    var transactionId = result.Data.TrxID;  // IntelliSense works!
}
```

## Benefits

✅ **Consistency** - Uniform response structure across all endpoints  
✅ **Type Safety** - Generic type provides compile-time checking  
✅ **Client-Friendly** - Easy to parse and handle in client applications  
✅ **Flexibility** - Works with any response type  
✅ **Error Handling** - Clear distinction between success and error states

## Integration with Frontend

### JavaScript/TypeScript Example

```typescript
interface BkashEndpoint<T> {
  status: string;
  data: T | null;
  message: string;
}

interface PaymentResponse {
  paymentID: string;
  bkashURL: string;
  amount: number;
}

async function createPayment(amount: number): Promise<void> {
  const response = await fetch("/api/payment/create", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ amount }),
  });

  const result: BkashEndpoint<PaymentResponse> = await response.json();

  if (result.status === "success" && result.data) {
    // Redirect to bKash payment page
    window.location.href = result.data.bkashURL;
  } else {
    alert(`Payment failed: ${result.message}`);
  }
}
```

## Related Documentation

- 📖 [Payment Operations](payment-operations.md)
- 📖 [Refund Operations](refund-operations.md)
- 📖 [Error Handling Guide](../guides/error-handling.md)
- 📖 [Code Examples](../getting-started/examples.md)

## See Also

- `BkashCreatePaymentResponse` - [Payment Operations](payment-operations.md#response-model)
- `BkashExecutePaymentResponse` - [Payment Operations](payment-operations.md#executepaymentasync)
- `BkashRefundPaymentResponse` - [Refund Operations](refund-operations.md#response-model)
