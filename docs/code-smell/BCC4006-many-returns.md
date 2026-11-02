# BCC4006: Method should not have many return statements

## What it reports

The rule reports a method with more than four `return` statements by default, or more than eight
when the method returns `bool`. Return statements belonging to one switch are counted as one.
Both limits are configurable in `.editorconfig`.

## Why it matters

Many exit points can make it difficult to reason about how a method reaches its result. A high
count may indicate that decision logic should be extracted or expressed more directly.

## Example

```csharp
string GetLabel(int code)
{
    if (code == 1) return "One";
    if (code == 2) return "Two";
    if (code == 3) return "Three";
    if (code == 4) return "Four";
    if (code == 5) return "Five"; // BCC4006: exceeds the default
    return "Unknown";
}
```

Consider a switch expression or a lookup table when it makes the mapping clearer.
