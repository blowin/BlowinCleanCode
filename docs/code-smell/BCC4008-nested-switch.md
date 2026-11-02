# BCC4008: Switch statements should not be nested

## What it reports

The rule reports a `switch` statement that contains another `switch` statement.

## Why it matters

Nested switches combine two sets of decisions and make their branches harder to trace. The
result can be difficult to extend without adding another layer of cases.

## Example

```csharp
switch (payment.Method)
{
    case Method.Card:
        switch (payment.Status) // BCC4008
        {
            case Status.Pending: Retry(); break;
        }
        break;
}
```

Extract one decision into a method or model the combined behavior explicitly.
