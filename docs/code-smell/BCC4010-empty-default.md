# BCC4010: Empty default clauses should be removed

## What it reports

The rule reports a switch statement with an empty `default` clause.

## Why it matters

An empty default branch can imply that a case was intentionally handled when it was not. It
also adds a branch without behavior.

## Example

```csharp
switch (status)
{
    case Status.Ready:
        Start();
        break;
    default: // BCC4010
        break;
}
```

Remove the empty clause, or add explicit behavior if the default case is meaningful.
