# BCC3003: Switch statements should have at least two case clauses

## What it reports

The rule reports a `switch` statement with fewer than two `case` clauses.

## Why it matters

A switch with only one handled case adds structure without much benefit and can hide a simple
conditional. A short `if` statement often communicates the intent more directly.

## Example

```csharp
switch (status)
{
    case Status.Ready: Start(); break; // BCC3003 when this is the only case
}
```

Consider replacing a single-case switch with an `if` statement.
