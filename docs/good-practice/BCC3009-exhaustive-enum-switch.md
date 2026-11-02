# BCC3009: Enum switches should be exhaustive

## What it reports

The rule reports a switch statement or switch expression over a non-flags enum when one or more
distinct declared enum values are not handled. A `default` switch label does not count as
coverage; explicit enum cases or a discard pattern in a pattern switch can cover values.
`[Flags]` enums are excluded.

## Why it matters

When an enum grows, an incomplete switch can silently leave a new state unhandled. Explicit
coverage makes the code's behavior easier to review and helps reveal missing cases.

## Example

```csharp
enum Status { New, Active, Closed }

string Describe(Status status) => status switch
{
    Status.New => "New",
    Status.Active => "Active"
    // BCC3009: Status.Closed is not handled
};
```

Add a case for each declared value and keep the handling intentional when the enum changes.
