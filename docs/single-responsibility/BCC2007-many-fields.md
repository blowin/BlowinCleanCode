# BCC2007: Large number of fields in a type

## What it reports

A type that declares methods is reported when it has more than five non-`const` fields by
default. The threshold can be configured with `bcc.BCC2007.max_number_of_field`.

## Why it matters

Many independent fields increase the amount of state a reader must track and can indicate that
the type owns several unrelated concerns.

## Example

```csharp
class OrderProcessor
{
    private Order _order;
    private Customer _customer;
    private ILogger _logger;
    private IEmailSender _emailSender;
    private IClock _clock;
    private IReportWriter _reportWriter;

    public void Process() { }
}
```

Consider separating responsibilities or grouping state that has a clear independent lifecycle.
