# BCC4002: Magic value

## What it reports

The rule reports literal values other than `0`, `1`, `-1`, and `null`. String literals inside a
binary expression, such as a comparison or concatenation, are ignored.

## Why it matters

An unexplained literal hides its meaning and makes policy changes error-prone. A named constant
or enum member documents why the value is used.

## Example

```csharp
if (user.Age >= 18) // BCC4002 on the literal 18
{
    AllowAccess();
}
```

Replace meaningful values with a named constant, for example `MinimumAdultAge`.
