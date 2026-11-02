# BCC2004: Method contains a lot of declarations

## What it reports

The rule reports a method with more than ten local variable declarations by default. The
threshold can be configured with `bcc.BCC2004.max_method_declaration`.

## Why it matters

Many locals can indicate that a method is coordinating too many steps or carrying too much
intermediate state. This makes the flow harder to follow and maintain.

## Example

```csharp
void BuildReport()
{
    var title = GetTitle();
    var owner = GetOwner();
    var created = GetCreatedDate();
    var status = GetStatus();
    var summary = GetSummary();
    var totals = GetTotals();
    var rows = GetRows();
    var footer = GetFooter();
    var layout = GetLayout();
    var theme = GetTheme();
    var output = GetOutput(); // the method has exceeded the default threshold
}
```

Consider extracting cohesive steps into small methods or introducing a type that owns related
report-building state.
