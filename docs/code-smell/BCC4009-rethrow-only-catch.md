# BCC4009: Catch should do more than rethrow

## What it reports

The rule reports a `catch` clause whose only action is to rethrow the caught exception.

## Why it matters

Catching and immediately rethrowing without adding context or recovery does not handle the
exception. It adds noise and can accidentally change stack information if the exception is
thrown as a new value.

## Example

```csharp
try
{
    Save();
}
catch (Exception exception) // BCC4009
{
    throw exception;
}
```

Remove the redundant `try`/`catch`, or add meaningful handling such as recovery or contextual
logging before rethrowing.
