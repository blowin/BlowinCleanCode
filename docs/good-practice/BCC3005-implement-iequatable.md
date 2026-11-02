# BCC3005: Implement IEquatable when providing Equals

## What it reports

The rule reports a type that declares a single-parameter `Equals(T)` method but does not
implement `IEquatable<T>`. An `Equals(object)` override alone does not trigger this rule.

## Why it matters

`IEquatable<T>` makes the strongly typed equality contract explicit and lets generic collections
and algorithms use it without falling back to object-based equality.

## Example

```csharp
class Customer
{
    public bool Equals(Customer other) => other != null && Id == other.Id; // BCC3005
    public int Id { get; }
}
```

Implement `IEquatable<Customer>` and ensure its equality and hash-code behavior agree.
