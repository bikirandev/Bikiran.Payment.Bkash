# ⚠️ Important API Updates - bKash Tokenized Checkout v1.2.0-beta

This document highlights critical changes made to align with the official bKash Tokenized Checkout API v1.2.0-beta specification.

## Critical Changes

### 1. 🔴 Mandatory Field: `payerReference`

**Issue:** The `payerReference` field was missing from all request examples.

**Impact:** Without this field, API calls will fail with "Invalid Data" or "Missing Mandatory Parameters" errors.

**Fix Applied:**

```csharp
public class BkashCreatePaymentRequest
{
    public double Amount { get; set; }
    public string Intent { get; set; }
    public string MerchantInvoiceNumber { get; set; }
    public string PayerReference { get; set; }           // ✅ ADDED - Required field
    public string Currency { get; set; } = "BDT";
    public string Mode { get; set; } = "0011";           // ✅ CORRECTED - Was "0001"
    public string CallbackURL { get; set; }
}
```

**What is `payerReference`?**

- Typically the customer's phone number (e.g., "01712345678")
- Can also be a unique customer ID from your system
- Must be provided in every Create Payment request

**Example Usage:**

```csharp
var request = new BkashCreatePaymentRequest
{
    Amount = 1000.00m,
    MerchantInvoiceNumber = "INV-123456",
    PayerReference = "01712345678",  // Customer phone number
    Intent = "sale"
};
```

### 2. 🟠 Corrected Default Mode

**Issue:** Default mode was set to "0001" (non-tokenized)

**Fix:** Changed default to "0011" (Tokenized Checkout)

**Why this matters:**

- `0011` is the standard mode for URL-based Tokenized Checkout
- `0001` refers to older, non-tokenized flows
- Using the wrong mode may cause integration issues

**Payment Modes:**
| Mode | Description | Usage |
|------|-------------|-------|
| `0011` | Tokenized Checkout (URL-based) | ✅ **Default - Use this** |
| `0001` | Non-tokenized Checkout | Legacy systems only |
| `0002` | Pre-authorization | Special use cases |

### 3. ✅ Added `payerReference` to Response Models

The `payerReference` field is also returned in response models for verification:

```csharp
public class BkashExecutePaymentResponse : BkashBaseResponse
{
    public string PaymentID { get; set; }
    public string TrxID { get; set; }
    public string PayerReference { get; set; }  // ✅ ADDED
    // ... other fields
}

public class BkashQueryPaymentResponse : BkashBaseResponse
{
    public string PaymentID { get; set; }
    public string TrxID { get; set; }
    public string PayerReference { get; set; }  // ✅ ADDED
    // ... other fields
}
```

### 4. 🟡 Query Payment Implementation Note

**Important:** The Query Payment endpoint uses **POST**, not GET.

**Endpoint:** `POST /tokenized/checkout/payment/status`

**Request Body:**

```json
{
  "paymentID": "TR001ABC123"
}
```

This is different from older API versions that used GET with URL parameters.

## Migration Checklist

If you have existing code using this library, update your code:

- [ ] Add `PayerReference` to all `BkashCreatePaymentRequest` instances
- [ ] Verify Mode is set to "0011" (or let it default)
- [ ] Update request validation to require `PayerReference`
- [ ] Test with bKash sandbox to verify changes work
- [ ] Update database schema if storing `PayerReference` for reconciliation

## Updated Documentation Files

All documentation has been updated with these corrections:

### API Reference

- ✅ [payment-operations.md](api-reference/payment-operations.md) - Updated request/response models

### Getting Started

- ✅ [quick-start.md](getting-started/quick-start.md) - Updated quick start example
- ✅ [examples.md](getting-started/examples.md) - All code examples updated

### Guides

- ✅ [payment-flow.md](guides/payment-flow.md) - Updated payment flow examples
- ✅ [error-handling.md](guides/error-handling.md) - Updated validation examples

### Development

- ✅ [testing.md](development/testing.md) - Updated test examples

### Root Files

- ✅ [README.md](../README.md) - Updated main examples and payment modes

## Before & After Comparison

### ❌ Before (Incorrect)

```csharp
var request = new BkashCreatePaymentRequest
{
    Amount = 1000.00m,
    MerchantInvoiceNumber = "INV-123456",
    Intent = "sale"
    // Missing: PayerReference
    // Wrong: Mode defaults to "0001"
};
```

### ✅ After (Correct)

```csharp
var request = new BkashCreatePaymentRequest
{
    Amount = 1000.00m,
    MerchantInvoiceNumber = "INV-123456",
    PayerReference = "01712345678",  // ✅ Required field
    Intent = "sale"
    // Mode defaults to "0011" ✅
};
```

## Testing Your Changes

Use these sandbox credentials to test:

```json
{
  "Bkash": {
    "AppKey": "4f6o0cjiki2rfm34kfdadl1eqq",
    "AppSecret": "2is7hdktrekvrbljjh44ll3d9l1dtjo4pasmjvs5vl5qr3fug4b",
    "Username": "sandboxTokenizedUser02",
    "Password": "sandboxTokenizedUser02@12345",
    "Environment": "Sandbox"
  }
}
```

## Questions?

If you encounter issues after these updates:

1. Verify you're using the correct sandbox/production credentials
2. Check that `PayerReference` is provided in all payment creation requests
3. Ensure Mode is set to "0011" (or let it default)
4. Review the [Error Handling Guide](guides/error-handling.md) for troubleshooting

## References

- Official bKash Tokenized Checkout API Documentation v1.2.0-beta
- [bKash Developer Portal](https://developer.bka.sh/)

---

**Last Updated:** January 10, 2026  
**Status:** ✅ All documentation updated and verified
