# BCC4012: Negative boolean name

## What it reports

The rule reports a `bool` field, local variable, property, or parameter whose name contains a
configured negative marker. The default list includes `Not`, `No`, `Dont`, `Disable`, and
`Invalid`.

## Why it matters

Negative booleans make conditions easy to invert mentally and can produce confusing double
negatives such as `if (!isNotReady)`.

## Example

```csharp
bool isNotAvailable; // BCC4012
```

Prefer an affirmative name such as `isAvailable` and invert the condition at the use site when
needed. The marker list can be configured in `.editorconfig`.
