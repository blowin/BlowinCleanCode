# BCC3000: Do not return null

## What it reports

The rule reports a method with a reference return type that returns `null`, including an
expression-bodied `=> null` return. A nullable-annotated return type is ignored.

## Why it matters

Callers must remember to check for null before using the result. A missing value represented by a
null object or an explicit result type can make the contract safer and clearer.

## Example

```csharp
User FindUser(int id)
{
    return null; // BCC3000
}
```

Consider returning a null object, a collection with no items, or an explicit `Option`/result type
appropriate to the API.
