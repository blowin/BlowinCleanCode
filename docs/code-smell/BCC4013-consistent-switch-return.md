# BCC4013: Consistent return in switch sections

## What it reports

When any section of a switch statement returns, the rule requires every section to terminate
with `return` or `throw`. Switch expressions are not reported because each arm already produces
a value or throws.

## Why it matters

Mixed terminating and fall-through sections make control flow less predictable and can cause
readers to miss that execution continues into later sections.

## Example

```csharp
switch (status)
{
    case Status.Ready:
        return "ready";
    case Status.Pending:
        LogPending(); // BCC4013: this section does not return or throw
        break;
    default:
        throw new InvalidOperationException();
}
```

Make each section's exit explicit and consistent, or use a switch expression where suitable.
