# BCC4001: Complex condition

## What it reports

The rule reports a condition that combines more than four boolean terms by default. It analyzes
conditions in constructs such as `if`, `while`, variable declarations, `return` statements, and
arguments. Configure the limit with `bcc.BCC4001.max_count_of_condition`.

## Why it matters

Many combined terms make it harder to determine which cases satisfy a condition and to identify
missing cases. Naming meaningful predicates can make the intent clearer.

## Example

```csharp
if (user.IsActive && user.Age >= 18 && user.HasEmail &&
    !user.IsBanned && user.Country == "DE") // BCC4001
{
    AllowAccess();
}
```

Extract cohesive checks into methods or local variables with descriptive names.
