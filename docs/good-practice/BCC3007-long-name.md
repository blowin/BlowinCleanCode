# BCC3007: Name is too long

## What it reports

The rule reports an identifier longer than 26 characters by default. Configure the maximum with
`bcc.BCC3007.max_name_length`.

## Why it matters

Very long identifiers can make declarations and expressions harder to scan. A shorter,
domain-specific name is often easier to use consistently.

## Example

```csharp
class CustomerNotificationPreferencesRepository // BCC3007
{
}
```

Prefer a concise name that preserves the concept without redundant words. Increase the threshold
in `.editorconfig` if a longer identifier is clearer in a specific codebase.
