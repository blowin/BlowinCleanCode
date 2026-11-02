# BCC2005: Too many chained references

## What it reports

The rule reports an invocation chain that exceeds the configured call limit. The default limit
is five calls. Calls returning the same type are treated as fluent calls and can have a separate
limit; both behaviors are configurable through `.editorconfig`.

## Why it matters

Long chains couple a caller to a sequence of intermediate details. A change to that sequence can
break distant callers, and deeply chained expressions are harder to debug.

## Example

```csharp
var name = order.GetCustomer().GetAddress().GetCountry().GetName().Trim().ToUpperInvariant(); // BCC2005
```

Consider introducing a named operation or moving the responsibility to the object that owns
those details. Use configuration for intentionally fluent APIs.
