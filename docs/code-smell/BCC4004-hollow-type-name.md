# BCC4004: Hollow type name

## What it reports

The rule reports a type name ending in `Helper`, `Util`, `Utils`, `Utility`, `Utilities`, `Info`,
or `Data`, or containing `Manager`. These word lists can be configured with `.editorconfig`.

## Why it matters

These broad names often say little about a type's actual responsibility and can become a home
for unrelated operations.

## Example

```csharp
class OrderHelper // BCC4004
{
    public void Validate(Order order) { }
    public void Export(Order order) { }
}
```

Name the type after the specific behavior it owns, such as `OrderValidator` or `OrderExporter`.
