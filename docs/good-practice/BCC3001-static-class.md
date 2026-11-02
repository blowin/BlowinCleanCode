# BCC3001: Do not use a static class

## What it reports

The rule reports a class declared with the `static` modifier.

## Why it matters

Static classes cannot be instantiated or substituted, which makes their behavior harder to
configure and test. When the type represents a service or shared behavior, an instance-based
design can support dependency injection and controlled lifetime.

## Example

```csharp
public static class Clock // BCC3001
{
    public static DateTime Now() => DateTime.Now;
}
```

Consider whether an injectable service or a singleton instance better expresses the type's role.
Pure utility functions may have valid reasons to remain static.
