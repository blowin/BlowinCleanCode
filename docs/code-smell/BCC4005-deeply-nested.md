# BCC4005: Deeply nested

## What it reports

The rule reports statements nested more than three levels deep by default. Configure the
maximum with `bcc.BCC4005.max_deeply_nested`.

## Why it matters

Deep nesting makes the main path harder to follow and increases the visual complexity of every
branch inside it.

## Example

```csharp
if (order != null)
{
    if (order.Customer != null)
    {
        if (order.Customer.IsActive)
        {
            if (order.IsPaid) // BCC4005: nesting exceeds the default limit
                Ship(order);
        }
    }
}
```

Use guard clauses, extract a method, or otherwise reduce the nesting depth.
