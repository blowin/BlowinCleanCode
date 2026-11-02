# BCC2008: Lambda has too many lines

## What it reports

The rule reports a lambda body longer than ten lines by default. The threshold can be configured
with `bcc.BCC2008.max_lambda_count_of_lines`.

## Why it matters

Large inline functions obscure the surrounding operation and are harder to name, test, and reuse.

## Example

```csharp
orders.Select(order =>
{
    Validate(order);
    var subtotal = CalculateSubtotal(order);
    var discount = CalculateDiscount(order);
    var taxableAmount = subtotal - discount;
    var tax = CalculateTax(order, taxableAmount);
    var shipping = CalculateShipping(order);
    var total = taxableAmount + tax + shipping;
    var currency = GetCurrency(order);
    var customerName = GetCustomerName(order);
    var summary = new Summary(order.Id, customerName, currency, total);
    return summary;
});
```

This lambda has more than ten body lines, so it exceeds the default threshold.

Move the transformation into a named method or a dedicated type when it has meaningful
responsibility.
