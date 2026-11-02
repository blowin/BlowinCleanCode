# BCC3002: Disposable member in a non-disposable class

## What it reports

The rule reports a type that holds an `IDisposable` or `IAsyncDisposable` member it creates but
does not itself implement `IDisposable`.

## Why it matters

The owning type is responsible for releasing resources it creates. If it does not expose a
disposal path, handles, streams, or other resources can remain open longer than intended.

## Example

```csharp
class ReportWriter
{
    private readonly StreamWriter _writer = new StreamWriter("report.txt"); // BCC3002
}
```

Implement `IDisposable` and dispose the owned member, or make ownership explicit by receiving
the resource from a caller that remains responsible for disposing it.
