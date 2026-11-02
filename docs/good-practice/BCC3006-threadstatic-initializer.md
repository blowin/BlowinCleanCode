# BCC3006: ThreadStatic fields should not be initialized

## What it reports

The rule reports a field marked with `[ThreadStatic]` that also has an initializer.

## Why it matters

The initializer runs only when the static field is first initialized, not once for every thread.
Other threads can therefore observe the default value rather than the intended initial value.

## Example

```csharp
class Context
{
    [ThreadStatic]
    private static int _depth = 1; // BCC3006
}
```

Initialize the value separately for each thread, or use a thread-local abstraction that supports
per-thread initialization.
