# BCC2002: Method contains 'And'

## What it reports

The rule reports a method whose name contains `And`, such as `SaveAndNotify`.

## Why it matters

The name often signals that the method performs multiple actions. Separating those actions can
make each operation easier to reuse, test, and understand.

## Example

```csharp
void SaveAndNotify(Order order) // BCC2002
{
    Save(order);
    NotifyCustomer(order);
}
```

Consider exposing `Save` and `NotifyCustomer` as separate operations, or delegate orchestration
to a caller with a name that describes the larger workflow.
