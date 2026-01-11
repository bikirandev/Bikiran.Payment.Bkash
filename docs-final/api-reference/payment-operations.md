# Payment Operations API Reference

Complete reference for payment creation, execution, and querying operations.

## IBkashPaymentService Interface

The main service interface for payment operations.

```csharp
public interface IBkashPaymentService
{
    Task<BkashCreatePaymentResponse> CreatePaymentAsync(
        BkashCreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<BkashExecutePaymentResponse> ExecutePaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default);

    Task<BkashQueryPaymentResponse> QueryPaymentAsync(
        string paymentId,
        CancellationToken cancellationToken = default);
}
```

## BkashEndpoint<T> Wrapper

For standardized API responses in your application, you can use the `BkashEndpoint<T>` generic wrapper class:

```csharp
public class BkashEndpoint<T>
{
    public string Status { get; set; }      // "success" or "error"
    public T? Data { get; set; }            // The actual response data
    public string Message { get; set; }     // Descriptive message
}
```

### Example Usage

```csharp
[HttpPost("create")]
public async Task<IActionResult> CreatePayment([FromBody] PaymentDto dto)
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
            Message = "Payment created successfully. Redirect to bKashURL."
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new BkashEndpoint<BkashCreatePaymentResponse>
        {
            Status = "error",
            Data = null,
            Message = ex.Message
        });
    }
}
```

## CreatePaymentAsync

Creates a new payment request and returns a bKash payment URL for the customer.

### Method Signature

```csharp
Task<BkashCreatePaymentResponse> CreatePaymentAsync(
    BkashCreatePaymentRequest request,
    CancellationToken cancellationToken = default)
```

### Request Model

```csharp
public class BkashCreatePaymentRequest
{
    public double Amount { get; set; }                  // Required
    public string Intent { get; set; }                   // Required: "sale" or "authorization"
    public string MerchantInvoiceNumber { get; set; }    // Required: Your invoice/order ID
    public string PayerReference { get; set; }           // Required: Customer phone number or unique ID
    public string Currency { get; set; } = "BDT";        // Optional: Default "BDT"
    public string Mode { get; set; } = "0011";           // Optional: Payment mode (0011 = Tokenized Checkout)
    public string CallbackURL { get; set; }              // Optional: Callback URL
}
```

### Response Model

```csharp
public class BkashCreatePaymentResponse : BkashBaseResponse
{
    public string PaymentID { get; set; }                // Payment identifier
    public string BkashURL { get; set; }                 // URL to redirect customer
    public string CallbackURL { get; set; }              // Your callback URL
    public string SuccessCallbackURL { get; set; }       // Success redirect URL
    public string FailureCallbackURL { get; set; }       // Failure redirect URL
    public string CancelledCallbackURL { get; set; }     // Cancel redirect URL
    public double Amount { get; set; }                  // Payment amount
    public string Intent { get; set; }                   // Payment intent
    public string Currency { get; set; }                 // Currency code
    public string MerchantInvoiceNumber { get; set; }    // Your invoice number

    // Inherited from BkashBaseResponse
    public string StatusCode { get; set; }
    public string StatusMessage { get; set; }
    public bool IsSuccess => StatusCode == "0000";
}
```

### Example Usage

```csharp
var request = new BkashCreatePaymentRequest
{
    Amount = 1000.50m,
    MerchantInvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
    PayerReference = "01712345678",  // Customer phone number
    Intent = "sale",
    Currency = "BDT",
    CallbackURL = "https://yoursite.com/payment/callback"
};

var response = await _bkashService.CreatePaymentAsync(request);

if (response.IsSuccess)
{
    // Redirect customer to response.BkashURL
    return Redirect(response.BkashURL);
}
```

### Payment Modes

| Mode   | Description                                                |
| ------ | ---------------------------------------------------------- |
| `0011` | Tokenized Checkout (URL-based, one-time payment) - Default |
| `0001` | Non-tokenized Checkout (legacy)                            |
| `0002` | Pre-authorization                                          |

### Validation Rules

- `Amount` must be greater than 0
- `Intent` must be "sale" or "authorization"
- `MerchantInvoiceNumber` is required and should be unique
- `PayerReference` is required (customer phone number or unique ID)
- `Currency` must be "BDT"
- `Mode` defaults to "0011" for Tokenized Checkout

### Common Errors

| Status Code | Description                     |
| ----------- | ------------------------------- |
| `2001`      | Invalid amount                  |
| `2002`      | Invalid merchant invoice number |
| `2027`      | Configuration error             |
| `9999`      | System error                    |

## ExecutePaymentAsync

Executes a payment after customer completes authorization on bKash.

### Method Signature

```csharp
Task<BkashExecutePaymentResponse> ExecutePaymentAsync(
    string paymentId,
    CancellationToken cancellationToken = default)
```

### Parameters

- `paymentId` (string, required): The payment ID returned from CreatePaymentAsync

### Response Model

```csharp
public class BkashExecutePaymentResponse : BkashBaseResponse
{
    public string PaymentID { get; set; }                // Payment identifier
    public string TrxID { get; set; }                    // bKash transaction ID
    public string TransactionStatus { get; set; }        // "Completed", "Cancelled", etc.
    public double Amount { get; set; }                  // Transaction amount
    public string Currency { get; set; }                 // Currency code
    public string Intent { get; set; }                   // Payment intent
    public string MerchantInvoiceNumber { get; set; }    // Your invoice number
    public string PayerReference { get; set; }           // Customer reference from request
    public DateTime PaymentExecuteTime { get; set; }     // Execution timestamp
    public string CustomerMsisdn { get; set; }           // Customer phone number

    public bool IsCompleted => TransactionStatus == "Completed";
    public bool IsCancelled => TransactionStatus == "Cancelled";
}
```

### Example Usage

```csharp
// After customer returns from bKash
var paymentId = Request.Query["paymentID"];
var status = Request.Query["status"];

if (status == "success")
{
    var response = await _bkashService.ExecutePaymentAsync(paymentId);

    if (response.IsCompleted)
    {
        // Payment successful - update order status
        await UpdateOrderStatus(response.MerchantInvoiceNumber, "Paid");

        return Ok(new
        {
            success = true,
            transactionId = response.TrxID,
            amount = response.Amount
        });
    }
}
```

### Transaction Status Values

| Status      | Description                    |
| ----------- | ------------------------------ |
| `Completed` | Payment successfully completed |
| `Cancelled` | Payment cancelled by customer  |
| `Failed`    | Payment failed                 |

### Timeout

- Payment must be executed within **15 minutes** of creation
- After 15 minutes, the payment expires

### Common Errors

| Status Code | Description              |
| ----------- | ------------------------ |
| `2014`      | Payment already executed |
| `2029`      | Payment expired          |
| `2062`      | Payment not found        |

## QueryPaymentAsync

Queries the current status and details of a payment.

### Method Signature

```csharp
Task<BkashQueryPaymentResponse> QueryPaymentAsync(
    string paymentId,
    CancellationToken cancellationToken = default)
```

### Parameters

- `paymentId` (string, required): The payment ID to query

### Response Model

```csharp
public class BkashQueryPaymentResponse : BkashBaseResponse
{
    public string PaymentID { get; set; }                // Payment identifier
    public string TrxID { get; set; }                    // Transaction ID (if executed)
    public string TransactionStatus { get; set; }        // Current status
    public double Amount { get; set; }                  // Payment amount
    public string Currency { get; set; }                 // Currency code
    public string Intent { get; set; }                   // Payment intent
    public string MerchantInvoiceNumber { get; set; }    // Your invoice number
    public string PayerReference { get; set; }           // Customer reference from request
    public DateTime CreateTime { get; set; }             // Creation timestamp
    public DateTime UpdateTime { get; set; }             // Last update timestamp
    public DateTime? PaymentExecuteTime { get; set; }    // Execution time (if completed)
    public string CustomerMsisdn { get; set; }           // Customer phone (if available)
}
```

### Implementation Note

**Important:** The Query Payment endpoint uses a **POST** request with a JSON body, not a GET request.

**Endpoint:** `POST /tokenized/checkout/payment/status`

**Request Body:**

```json
{
  "paymentID": "TR001ABC123"
}
```

This is different from older API versions that used GET with URL parameters.

### Example Usage

```csharp
// Query payment status
var response = await _bkashService.QueryPaymentAsync(paymentId);

Console.WriteLine($"Status: {response.TransactionStatus}");
Console.WriteLine($"Amount: {response.Amount}");
Console.WriteLine($"Created: {response.CreateTime}");

if (response.TransactionStatus == "Completed")
{
    Console.WriteLine($"Transaction ID: {response.TrxID}");
    Console.WriteLine($"Customer: {response.CustomerMsisdn}");
}
```

### Use Cases

- Check payment status after callback
- Reconciliation and reporting
- Customer support queries
- Webhook verification

### Query Limits

- Can query completed payments for up to **30 days**
- No query limit for recent payments

## Error Handling

### Exception Types

```csharp
try
{
    var response = await _bkashService.CreatePaymentAsync(request);
}
catch (BkashAuthenticationException ex)
{
    // Invalid credentials or token expired
    Console.WriteLine($"Auth error: {ex.Message}");
}
catch (BkashPaymentException ex)
{
    // Payment operation failed
    Console.WriteLine($"Payment error: {ex.ErrorCode} - {ex.Message}");
}
catch (BkashException ex)
{
    // General bKash error
    Console.WriteLine($"bKash error: {ex.Message}");
}
```

### Best Practices

1. **Always validate request data** before calling API
2. **Store PaymentID** in your database immediately after creation
3. **Implement idempotency** for execute operations
4. **Use try-catch** blocks for error handling
5. **Log transaction details** for auditing
6. **Query payment status** if callback is delayed

## Next Steps

- 📖 [Refund Operations](refund-operations.md) - Processing refunds
- 🔐 [Webhook Handling](webhook-handling.md) - Secure webhook integration
- 💉 [Token Management](token-management.md) - Automatic token handling
- 🔍 [Health Checks](health-checks.md) - Monitor service health

---

For more examples, see [Payment Flow Guide](../guides/payment-flow.md).
