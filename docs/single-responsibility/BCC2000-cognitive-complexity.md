# BCC2000: Cognitive complexity of a method

## What it reports

The rule reports methods whose cognitive complexity reaches the configured low, middle, or high
threshold. The defaults are 8, 10, and 15, respectively. The diagnostic identifies the method
and indicates the complexity band.

## Why it matters

Nested control flow and branching force readers to keep more conditions in mind. A method with
high cognitive complexity is harder to understand, review, and safely change.

## Example

```csharp
int Calculate(Order order)
{
    if (order != null)                 // branching
    {
        foreach (var item in order.Items) // loop and nesting
        {
            if (item.IsDiscounted)
            {
                if (item.IsAvailable)
                {
                    if (order.IsApproved)
                        return item.Price;
                }
            }
        }
    }

    return 0;
}
```

Consider extracting decisions into small, named methods. Thresholds can be configured through
`.editorconfig`; see the README's threshold options. For background, see the
[Cognitive Complexity white paper](https://www.sonarsource.com/docs/CognitiveComplexity.pdf).
