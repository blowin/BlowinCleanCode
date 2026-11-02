# BCC2009: Long method

## What it reports

The rule reports a method longer than 25 lines of code by default. Comments and other trivia do
not count. The threshold can be configured with `bcc.BCC2009.max_count_of_lines_in_method`.

## Why it matters

Long methods make it difficult to see the main path and to isolate the effect of a change.
Length is a useful signal to look for distinct steps or decisions that deserve their own names.

## Example

```csharp
public void ProcessOrder(Order order)
{
    Validate(order);
    ReserveStock(order);
    CalculateSubtotal(order);
    ApplyDiscount(order);
    CalculateTax(order);
    CalculateShipping(order);
    CalculateTotal(order);
    AuthorizePayment(order);
    CapturePayment(order);
    CreateInvoice(order);
    UpdateInventory(order);
    ScheduleDelivery(order);
    AssignWarehouse(order);
    CreateShipment(order);
    RegisterTracking(order);
    SendConfirmation(order);
    NotifyWarehouse(order);
    UpdateCustomerHistory(order);
    RecordAuditEntry(order);
    PublishOrderPlacedEvent(order);
    UpdateSearchIndex(order);
    RefreshOrderSummary(order);
    UpdateMetrics(order);
    ArchiveRequest(order);
    ReleaseTemporaryResources(order);
    LogCompletion(order);
    NotifySupportIfNeeded(order);
}
```

With more than 25 executable statement lines, this method exceeds the default threshold. Extract
cohesive operations and keep the top-level method focused on expressing their sequence.
