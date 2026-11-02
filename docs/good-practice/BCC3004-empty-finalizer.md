# BCC3004: Finalizers should not be empty

## What it reports

The rule reports a finalizer whose body is empty.

## Why it matters

Finalizers add runtime and garbage-collection overhead. An empty finalizer performs no cleanup,
so it adds cost without providing a benefit.

## Example

```csharp
class ResourceHolder
{
    ~ResourceHolder() { } // BCC3004
}
```

Remove the finalizer if there is no unmanaged resource to release. Use `IDisposable` for
deterministic cleanup of managed resources.
