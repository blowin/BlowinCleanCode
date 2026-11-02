# BCC4011: Middle man

## What it reports

The rule reports a type whose methods only delegate to a single field. Appending `Adapter` to
the type name opts out of the rule.

## Why it matters

A type that only forwards calls adds indirection without owning behavior. Callers must navigate
an extra object that does not provide a meaningful abstraction.

## Example

```csharp
class OrderService
{
    private readonly OrderRepository _repository;

    public Order Find(int id) => _repository.Find(id); // BCC4011 pattern
}
```

Remove unnecessary forwarding, move meaningful behavior into the owning type, or use an
`Adapter` suffix when the type is intentionally adapting an interface.
