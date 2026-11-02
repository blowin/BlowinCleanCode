# BCC2001: Many parameters in a method

## What it reports

A method is reported when it has more than four parameters by default. The maximum can be
configured with `bcc.BCC2001.max_method_parameter`.

## Why it matters

Long parameter lists are difficult to call correctly and often indicate that a group of values
belongs together or that a method has too many responsibilities.

## Example

```csharp
void CreateUser(string firstName, string lastName, string email, string phone, string country)
{
    // ...
}
```

Consider grouping related values into a parameter object or splitting the operation into smaller
methods.
