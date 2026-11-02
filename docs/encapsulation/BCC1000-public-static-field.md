# BCC1000: Do not use a public mutable static field

## What it reports

The rule reports a field that is `public` and `static` and is neither `readonly` nor `const`.
Public constants and readonly static fields are not reported.

## Why it matters

Any caller can change a public static field at any time. That creates shared mutable state,
makes behavior depend on call order, and exposes the implementation as a writable global API.
Prefer a property with controlled access or an immutable value.

## Example

```csharp
public static class Settings
{
    public static int RetryCount = 3; // BCC1000
}
```

```csharp
public static class Settings
{
    public static int RetryCount { get; private set; } = 3;
}
```
