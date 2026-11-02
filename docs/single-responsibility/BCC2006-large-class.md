# BCC2006: Large class

## What it reports

The rule reports a type when its weighted method count exceeds 10 by default. A private method
counts as `0.55`; every other method counts as `1`. The limit and weights are configurable.

## Why it matters

A type with many methods often combines several responsibilities. Such types are harder to
understand, test, and change without affecting unrelated behavior.

## Example

```csharp
public class OrderService
{
    public void PlaceOrder() { }
    public void CancelOrder() { }
    public void RefundOrder() { }
    public void SendReceipt() { }
    public void ExportOrders() { }
    public void ImportOrders() { }
    public void RebuildIndex() { }
    public void GenerateReport() { }
    public void ArchiveOrders() { }
    public void NotifyCustomers() { }
    public void ScheduleDelivery() { } // weighted method count exceeds the default
}
```

Look for cohesive groups of methods that can move into focused types.
