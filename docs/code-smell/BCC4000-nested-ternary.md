# BCC4000: Nested ternary operator

## What it reports

The rule reports a conditional (`?:`) expression nested inside another conditional expression.

## Why it matters

Nested ternaries compress multiple branches into a single expression. Their precedence and
reading order can be easy to misinterpret, especially when conditions are complex.

## Example

```csharp
var label = isReady ? (hasItems ? "Ready" : "Empty") : "Waiting"; // BCC4000
```

Use an `if`/`else` statement or extract the decision into a named method.
