# BCC4007: Switch should not have a lot of cases

## What it reports

The rule reports a switch with more than four `case` clauses by default. Configure the limit
with `bcc.BCC4007.max_switch_case_count`.

## Why it matters

A large switch often centralizes behavior that belongs with the values being switched on.
Polymorphism or a lookup can make extensions less likely to require edits to a long branch list.

## Example

```csharp
switch (shape)
{
    case Shape.Circle: DrawCircle(); break;
    case Shape.Square: DrawSquare(); break;
    case Shape.Triangle: DrawTriangle(); break;
    case Shape.Hexagon: DrawHexagon(); break;
    case Shape.Star: DrawStar(); break; // BCC4007
}
```

Consider polymorphic behavior or a data-driven mapping when appropriate.
