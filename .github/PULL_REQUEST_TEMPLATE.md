## What

<!-- One or two sentences. What does this change do? -->

## Why

Closes #<issue>

<!-- Explain the user-facing problem. For analyzer changes, include a minimal example that
     demonstrates the behavior or the issue being fixed. -->

```csharp
// Minimal example, when applicable
```

## How

<!-- Summarize the approach and any trade-offs. -->

## Verification

- [ ] `dotnet test BlowinCleanCode/BlowinCleanCode.Test/BlowinCleanCode.Test.csproj` passes
- [ ] New or updated tests: `<test file names>`
- [ ] Full solution build verified in a Visual Studio 2022 Developer Command Prompt (only when
      VSIX or packaging projects changed)

## Checklist

- [ ] Diagnostic ID is the next free number in its category and is declared in `Constant.Id`
- [ ] Analyzer is in the folder matching `Constant.Category`, and its file name matches its class
- [ ] Feature is registered in `BlowinCleanCodeAnalyzer.Features` under the correct category
- [ ] Thresholds are read through `Settings.Resolve(context)`; no magic values are inlined
- [ ] Any new option is declared in `Constant.Option`, has a default and reader property, and is
      listed in the README option table
- [ ] Diagnostic honors the `// Disable BCCxxxx` comment
- [ ] Positive, negative, and boundary cases are covered by tests
- [ ] Option changes are tested with `.editorconfig`
- [ ] English `docs/<category>/<diagnostic-id>-<rule-name>.md` page is added or updated with
      behavior, purpose, applicable thresholds or exclusions, and an example
- [ ] README category table links to the rule page
- [ ] `changelog.md` is updated for user-visible changes
- [ ] No version bump, generated files, or unrelated changes

## Breaking change

<!-- State "No" or describe the impact on users and any required migration. -->
