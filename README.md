# BlowinCleanCode

<img src="https://github.com/blowin/BlowinCleanCode/blob/master/icon.png" width="80" height="80">

| Source     | Link                                                                                                                                                                     |
| ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| VSIX       | [![VSIX](https://img.shields.io/visual-studio-marketplace/i/Blowin.1)](https://marketplace.visualstudio.com/items?itemName=Blowin.1)                                     |
| VSIX(VS22) | [![VSIX](https://img.shields.io/visual-studio-marketplace/i/Blowin.BlowinCleanCodeVS22)](https://marketplace.visualstudio.com/items?itemName=Blowin.BlowinCleanCodeVS22) |
| Nuget      | [![NUGET package](https://img.shields.io/nuget/v/Blowin.CleanCode.svg)](https://www.nuget.org/packages/Blowin.CleanCode/)                                                |

## Contents

- [Introduction](#introduction)
- [Requirements](#requirements)
- [Installation](#installation)
- [What a diagnostic looks like](#what-a-diagnostic-looks-like)
- [Rules](#rules)
  - [Single responsibility (BCC2xxx)](#single-responsibility-bcc2xxx)
  - [Encapsulation (BCC1xxx)](#encapsulation-bcc1xxx)
  - [Good practice (BCC3xxx)](#good-practice-bcc3xxx)
  - [Code smell (BCC4xxx)](#code-smell-bcc4xxx)
- [Default thresholds](#default-thresholds)
- [Suppressing a rule](#suppressing-a-rule)
- [Changelog](#changelog)
- [Contributing](#contributing)
- [Repository layout](#repository-layout)
- [Feedback](#feedback)

## Introduction

BlowinCleanCode is a Roslyn-based C# code analyzer that aims to provide a set of rules that helps
to simplify code and make it cleaner.

It is built on top of the Roslyn compiler platform, so it analyses your code while you type and
while you build, and it works anywhere Roslyn analyzers work: Visual Studio, JetBrains Rider,
VS Code (with the C# extension) and `dotnet build`. Every rule is a heuristic for one of four
categories:

| Category                  | Prefix    | Idea                                                                    |
| ------------------------- | --------- | ----------------------------------------------------------------------- |
| **Single responsibility** | `BCC2xxx` | A method or a type is doing more than one thing, or is simply too big.   |
| **Encapsulation**         | `BCC1xxx` | State is exposed in a way that lets any code mutate it.                  |
| **Good practice**         | `BCC3xxx` | Patterns that are known to cause bugs or cost performance later.         |
| **Code smell**            | `BCC4xxx` | Code that is correct but hard to read, and therefore hard to maintain.   |

The analyzer ships **32 rules**, all enabled by default with `Warning` severity, and it never
rewrites your code on its own — it only reports, and offers a code fix that suppresses the
diagnostic with a comment when you decide the rule does not apply.

This is a *style and structure* analyzer. It deliberately does not duplicate compiler warnings,
security analyzers, or framework-specific rules.

## Requirements

| Distribution                | Requirement                                                          |
| --------------------------- | -------------------------------------------------------------------- |
| NuGet package               | Any toolchain that supports Roslyn analyzers (Roslyn 3.3 and newer)   |
| VSIX for Visual Studio      | Visual Studio 2017 or 2019 (17.0 excluded)                           |
| VSIX for Visual Studio 2022 | Visual Studio 2022 (17.0 and newer, up to but excluding 18.0)         |

The analyzer itself targets `netstandard2.0` and is compiled against Roslyn 3.3, so it loads in
both older and current tooling.

## Installation

### NuGet (recommended — works in any IDE and in CI)

```bash
dotnet add package Blowin.CleanCode
```

or add the reference manually:

```xml
<ItemGroup>
  <PackageReference Include="Blowin.CleanCode" Version="2.6.0">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

The package is a **development dependency**: it is marked `PrivateAssets="all"` by default, so it
never flows to consumers of your library.

### Visual Studio extension

Install from the Visual Studio Marketplace and restart Visual Studio:

* **Visual Studio 2017 / 2019** — [BlowinCleanCode `Blowin.1`](https://marketplace.visualstudio.com/items?itemName=Blowin.1)
* **Visual Studio 2022** — [BlowinCleanCode `Blowin.BlowinCleanCodeVS22`](https://marketplace.visualstudio.com/items?itemName=Blowin.BlowinCleanCodeVS22)

The extension contains the same analyzers and the same code fix as the NuGet package.

## What a diagnostic looks like

Given this method:

```csharp
public bool Validate(User user)
{
    if (user.Age > 18 && user.Country == "DE" && user.IsActive && !user.IsBanned && user.Email != null)
        return true;

    return false;
}
```

BlowinCleanCode reports:

```text
warning BCC4001: The expression in the condition is too complex
warning BCC4002: Magic value '18'
```

Each diagnostic points at the exact expression that triggered it, and the diagnostic ID is a
stable, permanent identifier you can use in a suppression comment or in `.editorconfig`.

## Rules

All rules are enabled by default with `Warning` severity. The tables below list every rule with
its diagnostic ID and the condition that triggers it. The ID block encodes the category: `BCC1xxx`
encapsulation, `BCC2xxx` single responsibility, `BCC3xxx` good practice, `BCC4xxx` code smell.

### Single responsibility (BCC2xxx)

| ID        | Rule                                                                                       | Reported when                                                                                              |
| --------- | ------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------------------------------- |
| `BCC2000` | [Cognitive complexity of the method](https://www.sonarsource.com/docs/CognitiveComplexity.pdf) | The cognitive complexity of a method reaches 8. The message says whether it is low, middle or high.     |
| `BCC2001` | Many parameters in method                                                                   | A method has more than 4 parameters.                                                                        |
| `BCC2002` | Method contains 'And'                                                                       | The name of a method contains `And`, as in `RunAndClose` — a sign that it does two things.                   |
| `BCC2003` | Control flag                                                                                | A `bool` parameter is used as a control flag in an `if` or ternary condition inside the method.              |
| `BCC2004` | Method contains a lot of declarations                                                       | A method has more than 10 local variable declarations.                                                      |
| `BCC2005` | Too many chained references                                                                 | An invocation chain is longer than 5 calls (fluent chains are counted separately).                          |
| `BCC2006` | Large class                                                                                 | The weighted method count of a type exceeds 10 (private methods count 0.55, others count 1).                |
| `BCC2007` | Large number of fields in types                                                             | A type with methods has more than 5 non-`const` fields.                                                     |
| `BCC2008` | Lambda have too many lines                                                                  | A lambda body is longer than 10 lines.                                                                      |
| `BCC2009` | Long method                                                                                 | A method is longer than 25 lines of code (trivia and comments are not counted).                             |

### Encapsulation (BCC1xxx)

| ID        | Rule                         | Reported when                                                                     |
| --------- | ---------------------------- | --------------------------------------------------------------------------------- |
| `BCC1000` | Don't use public static field | A field is `public` **and** `static` **and** neither `readonly` nor `const`.        |

### Good practice (BCC3xxx)

| ID        | Rule                                                    | Reported when                                                                                   |
| --------- | ------------------------------------------------------- | ----------------------------------------------------------------------------------------------- |
| `BCC3000` | Don't return null                                       | A method with a reference return type returns `null` (including `=> null`); nullable-annotated returns are ignored. Use the null object pattern instead. |
| `BCC3001` | Don't use static class                                  | A class is declared `static`. Consider the singleton pattern instead.                            |
| `BCC3002` | Disposable member in non disposable class               | A type holds an `IDisposable`/`IAsyncDisposable` member that it creates, but does not implement `IDisposable` itself. |
| `BCC3003` | Switch statements should have at least 2 case clauses    | A `switch` has fewer than 2 `case` clauses; an `if`/ternary would be more readable.              |
| `BCC3004` | Finalizers should not be empty                          | A finalizer has an empty body — it costs performance with no benefit.                            |
| `BCC3005` | Type that provide Equals should implement IEquatable     | A type declares a single-parameter `Equals(T)` but does not implement `IEquatable<T>`; `Equals(object)` is ignored. |
| `BCC3006` | 'ThreadStatic' fields should not be initialized          | A field marked `[ThreadStatic]` has an initializer, which only initializes the first thread.     |
| `BCC3007` | Name is too long                                        | An identifier is longer than 26 characters.                                                      |
| `BCC3008` | Use only ASCII characters for names                      | An identifier contains non-ASCII characters.                                                     |

### Code smell (BCC4xxx)

| ID        | Rule                                                | Reported when                                                                                                    |
| --------- | --------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------- |
| `BCC4000` | Nested ternary operator                             | A ternary operator is nested inside another one.                                                                  |
| `BCC4001` | Complex condition                                   | A condition (in `if`, `while`, variable declaration, `return` or argument) combines more than 4 boolean terms.     |
| `BCC4002` | Magic value                                         | An expression contains a literal other than `0`, `1`, `-1` or `null`. String literals inside a binary expression (a comparison or a concatenation) are ignored. |
| `BCC4003` | Preserve whole object                               | A call passes more than 2 members of the same object as separate arguments instead of the object itself.          |
| `BCC4004` | Hollow type name                                    | A type name ends with `Helper`, `Util`, `Utils`, `Utility`, `Utilities`, `Info` or `Data`, or contains `Manager`.  |
| `BCC4005` | Deeply nested                                       | Statements are nested more than 3 levels deep.                                                                    |
| `BCC4006` | Method should not have many return statements       | A method has more than 4 `return` statements (8 when it returns `bool`); all returns of one `switch` count as 1. |
| `BCC4007` | Switch should not have a lot of cases               | A `switch` has more than 4 `case` clauses — polymorphism may be a better fit.                                     |
| `BCC4008` | Switch statements should not be nested              | A `switch` statement contains another `switch` statement.                                                          |
| `BCC4009` | Catch should do more than rethrow                   | A `catch` clause only rethrows the caught exception.                                                              |
| `BCC4010` | Empty 'default' clauses should be removed           | A `switch` has an empty `default` clause.                                                                         |
| `BCC4011` | [Middle man](https://refactoring.guru/smells/middle-man) | A type whose methods only delegate to a single field. Append `Adapter` to the type name to opt out.          |

> The rule IDs are permanent. They never change meaning, and a removed rule keeps its number
> reserved. New rules always take the next free number in their block.

## Default thresholds

Every threshold below is a default compiled into the analyzer. They are **not configurable from
`.editorconfig` at the moment**, so if a rule fires too often for your code base, either suppress
the individual diagnostic (see [Suppressing a rule](#suppressing-a-rule)) or open an issue with
the code that triggers it.

The values live in
[`AnalyzerSettings`](BlowinCleanCode/BlowinCleanCode/Model/Settings/AnalyzerSettings.cs); changing
a default is a user-visible change and follows the usual documentation and test rules described
in [CONTRIBUTING.md](CONTRIBUTING.md).

| Setting                                     | Default | Used by                                          |
| ------------------------------------------- | ------- | ------------------------------------------------ |
| `MaxNameLength`                             | `26`    | `BCC3007` Name is too long                       |
| `MaxNumberOfField`                          | `5`     | `BCC2007` Large number of fields                 |
| `MaxDeeplyNested`                           | `3`     | `BCC4005` Deeply nested                          |
| `MaxMethodDeclaration`                      | `10`    | `BCC2004` Method contains a lot of declarations  |
| `MaxCountOfLinesInMethod`                   | `25`    | `BCC2009` Long method                            |
| `MaxLambdaCountOfLines`                     | `10`    | `BCC2008` Lambda have too many lines             |
| `MaxMethodParameter`                        | `4`     | `BCC2001` Many parameters in method              |
| `MaxCountOfCondition`                       | `4`     | `BCC4001` Complex condition                      |
| `MaxPreserveWholeObjectCount`               | `2`     | `BCC4003` Preserve whole object                  |
| `MaxReturnStatement`                        | `4`     | `BCC4006` Method return statements               |
| `MaxReturnStatementForReturnBool`           | `8`     | `BCC4006` for methods returning `bool`           |
| `MaxSwitchCaseCount`                        | `4`     | `BCC4007` Switch should not have a lot of cases  |
| `CognitiveComplexity.MinLowComplexity`      | `8`     | `BCC2000` Cognitive complexity                   |
| `CognitiveComplexity.MinMiddleComplexity`   | `10`    | `BCC2000` Cognitive complexity                   |
| `CognitiveComplexity.MinHighComplexity`     | `15`    | `BCC2000` Cognitive complexity                   |
| `ChainCallSettings.MaxCall`                 | `5`     | `BCC2005` Too many chained references            |
| `ChainCallSettings.MaxFluentInterfaceCall`  | —       | `BCC2005`, disabled unless set                   |
| `LargeClass.MaxMethodThreshold`             | `10`    | `BCC2006` Large class                            |
| `LargeClass.PrivateMethodThreshold`         | `0.55`  | `BCC2006` weight of a private method             |
| `LargeClass.NonPrivateMethodThreshold`      | `1`     | `BCC2006` weight of a non-private method         |

## Suppressing a rule

### With a comment

Place a single-line comment of the form `// Disable BCCxxxx` on the line directly above the
statement, method or type you want to exclude:

```csharp
public class OrderService
{
    // Disable BCC2003
    public void Process(Order order, bool validate)
    {
        if (validate)
            order.Validate();
    }
}
```

* Above a **statement** — the rule stops reporting on that statement.
* Above a **method** — the rule stops reporting anywhere inside that method.
* Above a **type** — the rule stops reporting anywhere inside that type.

The comment must contain exactly `// Disable ` followed by the diagnostic ID; anything else,
including a different comment style, is ignored. In Visual Studio the lightbulb menu offers
*Disable '…' with comment for line / for method / for class*, which inserts the comment for you.

### With `.editorconfig`

Because the diagnostics are ordinary Roslyn diagnostics, the standard mechanism works as well:

```ini
[*.cs]
dotnet_diagnostic.BCC4002.severity = none       # turn a rule off completely
dotnet_diagnostic.BCC2009.severity = suggestion # or lower its severity
```

## Changelog

Release notes for every version live in [CHANGELOG.md](CHANGELOG.md).

## Contributing

Contributions are welcome — bug reports, false-positive reports, new rules and documentation
improvements alike.

**Read [CONTRIBUTING.md](CONTRIBUTING.md) before you start.** It documents the repository
structure, how diagnostic IDs and constants are allocated, how an analyzer is implemented, the
mandatory test coverage, the README/changelog requirements, and the exact format of commits and
pull requests.

In short:

```bash
# build and run the tests (the VSIX projects need the Visual Studio SDK and are not required)
dotnet test BlowinCleanCode/BlowinCleanCode.Test/BlowinCleanCode.Test.csproj
```

A change is only complete when it includes tests, a `README.md` update (for new or changed
rules) and a `changelog.md` entry (for user-visible changes).

## Repository layout

| Path                                                                  | What it contains                                             |
| --------------------------------------------------------------------- | ------------------------------------------------------------ |
| [`BlowinCleanCode/BlowinCleanCode/`](BlowinCleanCode/BlowinCleanCode/) | The analyzer: features, base classes, constants, settings.    |
| [`BlowinCleanCode/BlowinCleanCode.CodeFix/`](BlowinCleanCode/BlowinCleanCode.CodeFix/) | The *Disable with comment* code fix.        |
| [`BlowinCleanCode/BlowinCleanCode.Test/`](BlowinCleanCode/BlowinCleanCode.Test/) | The xUnit test suite, one file per rule.     |
| [`BlowinCleanCode/BlowinCleanCode.Package/`](BlowinCleanCode/BlowinCleanCode.Package/) | The `Blowin.CleanCode` NuGet package.    |
| [`BlowinCleanCode/BlowinCleanCode.Vsix/`](BlowinCleanCode/BlowinCleanCode.Vsix/) | The VSIX for Visual Studio 2017/2019.        |
| [`BlowinCleanCode/BlowinCleanCode.Analyzer.Vsix.VS22/`](BlowinCleanCode/BlowinCleanCode.Analyzer.Vsix.VS22/) | The VSIX for Visual Studio 2022. |
| [`changelog.md`](changelog.md)                                        | Release notes.                                                |
| [`CONTRIBUTING.md`](CONTRIBUTING.md)                                  | Development rules and pull request process.                    |

Every rule is registered in a single place —
[`BlowinCleanCodeAnalyzer.cs`](BlowinCleanCode/BlowinCleanCode/BlowinCleanCodeAnalyzer.cs) — and
every diagnostic ID is declared in
[`Constant.cs`](BlowinCleanCode/BlowinCleanCode/Constant.cs).

## Feedback

* Found a false positive or a rule that is missing? Open an
  [issue](https://github.com/blowin/BlowinCleanCode/issues) with a minimal code sample that
  reproduces it.
* Want to submit a fix? See [CONTRIBUTING.md](CONTRIBUTING.md).
