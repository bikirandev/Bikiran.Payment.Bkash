# Payment Flow Guide

Complete walkthrough of the bKash payment flow from creation to completion.

## Payment Flow Overview

```
Customer → Your Site → bKash → Customer → Your Site → Complete
   (1)        (2)       (3)       (4)       (5)        (6)
```

1. **Customer initiates payment** on your website
2. **Your backend creates payment** via bKash API
3. **Customer redirected to bKash** to authorize payment
4. **Customer completes payment** on bKash and returns
5. **Your backend executes payment** to finalize transaction
6. **Order confirmed** and customer sees success page

## Step-by-Step Implementation

### Step 1: Customer Initiates Payment

Customer clicks "Pay with bKash" on your website/app.

**Frontend (HTML/JavaScript):**

```html
<button id="pay-button">Pay with bKash</button>

<script>
  document.getElementById("pay-button").addEventListener("click", async () => {
    const amount = 1000; // Amount in BDT

    const response = await fetch("/api/payment/create", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ amount }),
    });

    const data = await response.json();

    if (data.success) {
      // Redirect to bKash
      window.location.href = data.bkashUrl;
    }
  });
</script>
```

### Step 2: Create Payment (Backend)

Your backend creates a payment request with bKash.

**Backend (C#):**

```csharp
[HttpPost("api/payment/create")]
public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
{
    // 1. Validate request
    if (dto.Amount <= 0)
    {
        return BadRequest("Invalid amount");
    }

    // 2. Create order in your database
    var order = await _orderService.CreateOrderAsync(new Order
    {
        Amount = dto.Amount,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    });

    // 3. Create bKash payment
    var request = new BkashCreatePaymentRequest
    {
        Amount = dto.Amount,
        MerchantInvoiceNumber = order.Id.ToString(),
        PayerReference = dto.CustomerPhone ?? "01712345678",  // Customer phone number
        Intent = "sale",
        CallbackURL = "https://yoursite.com/payment/callback"
    };

    try
    {
        var response = await _bkashService.CreatePaymentAsync(request);

        // 4. Store payment ID in database
        order.PaymentId = response.PaymentID;
        await _orderService.UpdateOrderAsync(order);

        // 5. Return bKash URL to frontend
        return Ok(new
        {
            success = true,
            paymentId = response.PaymentID,
            bkashUrl = response.BkashURL
        });
    }
    catch (BkashPaymentException ex)
    {
        _logger.LogError(ex, "Payment creation failed");
        return BadRequest(new { success = false, message = ex.Message });
    }
}
```

### Step 3: Customer Authorizes Payment

Customer is redirected to bKash payment page where they:

1. Enter their bKash account PIN
2. Review payment details
3. Confirm or cancel the payment

**bKash handles this step entirely** - no code needed.

### Step 4: Customer Returns to Your Site

After payment authorization, bKash redirects customer back to your callback URL with parameters:

```
https://yoursite.com/payment/callback?paymentID=TR001ABC123&status=success
```

Parameters:

- `paymentID`: Payment identifier
- `status`: "success", "failure", or "cancel"

### Step 5: Execute Payment (Backend)

Your backend receives the callback and executes the payment.

**Backend (C#):**

```csharp
[HttpGet("payment/callback")]
public async Task<IActionResult> PaymentCallback(
    [FromQuery] string paymentID,
    [FromQuery] string status)
{
    if (string.IsNullOrEmpty(paymentID))
    {
        return BadRequest("Missing paymentID");
    }

    // Find order by payment ID
    var order = await _orderService.GetOrderByPaymentIdAsync(paymentID);
    if (order == null)
    {
        return NotFound("Order not found");
    }

    if (status == "success")
    {
        try
        {
            // Execute the payment
            var response = await _bkashService.ExecutePaymentAsync(paymentID);

            if (response.IsCompleted)
            {
                // Update order status
                order.Status = "Paid";
                order.TransactionId = response.TrxID;
                order.CompletedAt = DateTime.UtcNow;
                await _orderService.UpdateOrderAsync(order);

                // Redirect to success page
                return Redirect($"/payment/success?orderId={order.Id}");
            }
            else
            {
                // Payment execution failed
                order.Status = "Failed";
                await _orderService.UpdateOrderAsync(order);

                return Redirect($"/payment/failure?orderId={order.Id}");
            }
        }
        catch (BkashPaymentException ex)
        {
            _logger.LogError(ex, "Payment execution failed for {PaymentId}", paymentID);

            order.Status = "Failed";
            await _orderService.UpdateOrderAsync(order);

            return Redirect($"/payment/failure?orderId={order.Id}");
        }
    }
    else if (status == "cancel")
    {
        // Customer cancelled payment
        order.Status = "Cancelled";
        await _orderService.UpdateOrderAsync(order);

        return Redirect($"/payment/cancelled?orderId={order.Id}");
    }
    else
    {
        // Payment failed
        order.Status = "Failed";
        await _orderService.UpdateOrderAsync(order);

        return Redirect($"/payment/failure?orderId={order.Id}");
    }
}
```

### Step 6: Show Confirmation Page

Display order confirmation to the customer.

**Success Page:**

```html
<h1>Payment Successful!</h1>
<p>Your payment has been processed successfully.</p>
<p>Transaction ID: {{transactionId}}</p>
<p>Order ID: {{orderId}}</p>
<p>Amount: {{amount}} BDT</p>
```

## Complete Flow Diagram

```
┌─────────────┐
│  Customer   │
└──────┬──────┘
       │ 1. Click "Pay with bKash"
       ↓
┌─────────────┐
│ Your Website│
└──────┬──────┘
       │ 2. POST /api/payment/create
       ↓
┌─────────────┐
│  Your API   │
│─────────────│
│ CreateOrder │
│CreatePayment│
│ SavePaymentID│
└──────┬──────┘
       │ 3. Return bkashURL
       ↓
┌─────────────┐
│ Your Website│
└──────┬──────┘
       │ 4. Redirect to bkashURL
       ↓
┌─────────────┐
│    bKash    │
│─────────────│
│Enter PIN    │
│Review Amount│
│Confirm      │
└──────┬──────┘
       │ 5. Redirect to callback
       ↓
┌─────────────┐
│  Your API   │
│─────────────│
│ExecutePayment│
│UpdateOrder  │
└──────┬──────┘
       │ 6. Redirect to success
       ↓
┌─────────────┐
│ Your Website│
│─────────────│
│Show Success │
└─────────────┘
```

## Important Timeouts

- **Payment Expiry**: 15 minutes from creation
- **Customer has 15 minutes** to complete payment on bKash
- **After 15 minutes**, payment expires and cannot be executed

## Error Handling

### Payment Creation Fails

```csharp
catch (BkashAuthenticationException ex)
{
    // Invalid credentials
    _logger.LogError(ex, "Authentication failed");
    return StatusCode(500, "Configuration error");
}
catch (BkashPaymentException ex)
{
    // Payment-specific error
    _logger.LogError(ex, "Payment creation failed");
    return BadRequest(new { error = ex.Message, code = ex.ErrorCode });
}
```

### Payment Execution Fails

```csharp
catch (BkashPaymentException ex)
{
    if (ex.ErrorCode == "2029")
    {
        // Payment expired
        return Redirect("/payment/expired");
    }
    else if (ex.ErrorCode == "2014")
    {
        // Already executed (idempotency)
        var existingOrder = await _orderService.GetOrderByPaymentIdAsync(paymentID);
        return Redirect($"/payment/success?orderId={existingOrder.Id}");
    }
    else
    {
        // Other error
        return Redirect("/payment/failure");
    }
}
```

## Best Practices

### 1. Store Payment ID Immediately

```csharp
// After creating payment, store immediately
order.PaymentId = response.PaymentID;
await _orderService.UpdateOrderAsync(order);
```

### 2. Implement Idempotency

```csharp
// Check if payment already processed
if (order.Status == "Paid")
{
    return Redirect($"/payment/success?orderId={order.Id}");
}
```

### 3. Handle Duplicate Callbacks

```csharp
// bKash may send callback multiple times
if (order.Status != "Pending")
{
    // Already processed
    return Redirect($"/payment/{order.Status.ToLower()}?orderId={order.Id}");
}
```

### 4. Log All Steps

```csharp
_logger.LogInformation("Payment created: {PaymentId} for Order: {OrderId}", paymentID, orderId);
_logger.LogInformation("Payment executed: {PaymentId}, TrxID: {TrxID}", paymentID, trxID);
```

### 5. Query Status if Callback Delayed

```csharp
// If callback doesn't arrive, query payment status
var response = await _bkashService.QueryPaymentAsync(paymentID);
if (response.TransactionStatus == "Completed")
{
    // Process payment even without callback
}
```

## Testing Flow

### Sandbox Test Cards

bKash sandbox provides test scenarios:

- **Success**: Use any valid bKash format number
- **Insufficient Balance**: Test specific scenarios
- **Timeout**: Wait 15+ minutes

### Test Checklist

- [ ] Create payment successfully
- [ ] Customer redirected to bKash
- [ ] Customer completes payment
- [ ] Callback received with correct parameters
- [ ] Payment executed successfully
- [ ] Order status updated correctly
- [ ] Success page displays correctly
- [ ] Handle cancelled payment
- [ ] Handle failed payment
- [ ] Handle timeout (15 minutes)
- [ ] Handle duplicate callbacks

## Next Steps

- 📖 [Webhook Integration](webhook-integration.md) - Add webhook notifications
- 📖 [Error Handling](error-handling.md) - Comprehensive error handling
- 🔒 [Security Best Practices](security-best-practices.md) - Secure your integration
- 🚀 [Production Deployment](production-deployment.md) - Deploy to production
