# BCC3008: Use only ASCII characters for names

## What it reports

The rule reports an identifier containing a non-ASCII character.

## Why it matters

Visually similar characters from different alphabets can be difficult to distinguish and can
cause confusion in code review, search, and tooling.

## Example

```csharp
class Cоnfiguration // BCC3008: the "о" is Cyrillic, not ASCII
{
}
```

Use ASCII characters for identifiers. Non-ASCII text in user-facing strings is not the concern
of this naming rule.
