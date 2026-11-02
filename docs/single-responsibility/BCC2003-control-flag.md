# BCC2003: Control flag

## What it reports

The rule reports a `bool` parameter used in an `if` or conditional expression inside the method
to select behavior.

## Why it matters

A control flag makes one method represent multiple behaviors. Call sites can be unclear
(`Process(order, true)`), and the method's responsibilities become harder to test independently.

## Example

```csharp
void Process(Order order, bool validate) // BCC2003
{
    if (validate)
        order.Validate();

    order.Save();
}
```

Consider separate methods such as `ProcessValidated` and `ProcessWithoutValidation`, or move the
choice to the caller.
