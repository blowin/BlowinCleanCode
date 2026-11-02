# BCC4003: Preserve whole object

## What it reports

The rule reports a call that passes more than two members of the same object as separate
arguments.

## Why it matters

Passing several pieces of one object couples the caller to its internal shape. Passing the
object preserves that context and can make a method signature more stable.

## Example

```csharp
CreateUser(customer.FirstName, customer.LastName, customer.Email); // BCC4003
```

Consider accepting the `customer` object or a purpose-built value object instead. The maximum
member count can be configured with `bcc.BCC4003.max_preserve_whole_object_count`.
