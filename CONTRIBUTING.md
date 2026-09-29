# Contributing to BlowinCleanCode

This document is the single source of truth for how changes are made to this repository:
where code lives, how diagnostics and constants are declared, how tests and documentation are
written, and how a pull request must be prepared.

The keywords **MUST**, **MUST NOT**, **SHOULD**, **SHOULD NOT** and **MAY** are used in the
sense of RFC 2119.

---

## Table of contents

1. [What this project is](#1-what-this-project-is)
2. [Repository layout](#2-repository-layout)
3. [Local setup, build and test](#3-local-setup-build-and-test)
4. [Diagnostic IDs and constants](#4-diagnostic-ids-and-constants)
5. [Adding or changing an analyzer](#5-adding-or-changing-an-analyzer)
6. [Feature implementation rules](#6-feature-implementation-rules)
7. [Settings, thresholds and analyzer options](#7-settings-thresholds-and-analyzer-options)
8. [Tests](#8-tests)
9. [Documentation](#9-documentation)
10. [Packaging and versions](#10-packaging-and-versions)
11. [Branches and commits](#11-branches-and-commits)
12. [Pull requests](#12-pull-requests)
13. [Definition of Done](#13-definition-of-done)
14. [Appendix A — Diagnostic ID registry](#appendix-a--diagnostic-id-registry)
15. [Appendix B — PR description template](#appendix-b--pr-description-template)

---

## 1. What this project is

BlowinCleanCode is a Roslyn-based C# code analyzer distributed in two forms:

* as a **NuGet analyzer package** (`Blowin.CleanCode`), built from `BlowinCleanCode.Package`;
* as a **Visual Studio extension** (VSIX) for VS 2017/2019 and VS 2022.

Every rule is a self-contained *feature* that reports one diagnostic ID, and every diagnostic ID
is defined in exactly one place (`Constant.Id`). The public contract of the project is therefore
made of three things:

1. the **diagnostic IDs** — they are permanent, they appear in user code as
   `// Disable BCCxxxx` comments and in `.editorconfig` files;
2. the **message text** of each diagnostic;
3. the **thresholds** that decide when a diagnostic fires.

A change that alters any of these three is a breaking change for users and MUST be treated as
such (documented in `changelog.md`, and never done silently inside an unrelated PR).

---

## 2. Repository layout

```
BlowinCleanCode.sln                  solution (all projects)
icon.png                             package icon
NuGet.config                         single feed: nuget.org
README.md                            user-facing documentation
changelog.md                         release history
CONTRIBUTING.md                      this file
.github/workflows/dotnet.yml         build + test CI
.github/workflows/code-review.yml    automated review CI

BlowinCleanCode/
  BlowinCleanCode/                   analyzer assembly (netstandard2.0) — the core project
    BlowinCleanCodeAnalyzer.cs       the registry: the single list of all features
    Constant.cs                      diagnostic IDs, categories and option keys
    Feature/
      IFeature.cs                    the feature contract
      Base/                          base classes, one per analysis strategy
      CodeSmell/                     analyzers of category "Code smell"      (BCC4xxx)
      GoodPractice/                  analyzers of category "Good practice"    (BCC3xxx)
      SingleResponsibility/          analyzers of category "Single responsibility" (BCC2xxx)
      <FeatureName>FeatureAnalyze.cs analyzers of category "Encapsulation"   (BCC1xxx)
    Model/                           settings, options, comment/skip handling, walkers, value types
      Settings/                      AnalyzerSettings (defaults) and AnalyzerSettingsReader (effective values)
    Extension/                       extension methods, grouped by target type
      SymbolExtension/               ISymbol / INamedTypeSymbol extensions
      SyntaxExtension/               syntax node extensions
  BlowinCleanCode.CodeFix/           code fixes (netstandard2.0)
  BlowinCleanCode.Test/              xUnit test project (net6.0)
  BlowinCleanCode.Package/           NuGet packaging project
  BlowinCleanCode.Vsix/              VSIX for VS 2017/2019 (net472)
  BlowinCleanCode.Analyzer.Vsix.VS22/ VSIX for VS 2022 (net472)
```

Rules:

* **An analyzer MUST live in the folder that matches its `Constant.Category`**, and the folder
  MUST match the numeric block of its diagnostic ID (see §4).
* **One analyzer class per file.** The file name MUST equal the class name.
* **A test file MUST live in the test folder that matches the same category.**
* New top-level folders MUST NOT be added without a corresponding section in this document.
* Never move existing analyzers or tests between folders in a PR that also changes behaviour;
  do structural moves in a separate, behaviour-preserving PR.

---

## 3. Local setup, build and test

### Tooling

* .NET SDK 6.0 or newer (the test project targets `net6.0`).
* Visual Studio 2022 with the **Visual Studio extension development** workload — required only
  if you touch the VSIX projects.
* No other global tooling is required; there are no analyzers, formatters or code generators to
  install, because the repository has no `.editorconfig` and no `Directory.Build.props`.

### Build and test

The solution mixes `netstandard2.0` analyzer and code-fix projects, a `net6.0` test project, and
`net472` VSIX/packaging projects that require the Visual Studio SDK. A plain
`dotnet build BlowinCleanCode.sln` therefore does **not** work outside a Visual Studio developer
environment — restoring the two VSIX projects fails without the VS SDK. Unless you are changing
extension packaging, build and test through the test project:

```bash
dotnet test BlowinCleanCode/BlowinCleanCode.Test/BlowinCleanCode.Test.csproj
```

To build only the analyzer or the code fixes:

```bash
dotnet build BlowinCleanCode/BlowinCleanCode/BlowinCleanCode.csproj
dotnet build BlowinCleanCode/BlowinCleanCode.CodeFix/BlowinCleanCode.CodeFix.csproj
```

If you do change the VSIX or package projects, verify the full solution from a *Developer
Command Prompt for VS 2022* (`msbuild BlowinCleanCode.sln`) and state that you did so in the PR.

### CI

`.github/workflows/dotnet.yml` runs `dotnet restore`, `dotnet build` and `dotnet test` on
`ubuntu-latest`. It is triggered on pushes and pull requests to `master`, but **not** for
changes that touch only `README.md` or `changelog.md`. A PR that changes only documentation
will not receive any checks; that is intentional, but it also means documentation-only PRs must
be reviewed more carefully by a human.

Your PR MUST NOT be merged while the `Build` workflow is red.

---

## 4. Diagnostic IDs and constants

### 4.1 Where constants live

**Every constant of the analyzer belongs to `Constant.cs`.** No diagnostic ID, category name or
well-known string may be inlined anywhere else.

`Constant` has exactly four nested static classes and MUST NOT grow a fifth without a review:

```csharp
public static class Constant
{
    public static class ListOf { /* derived lookup sets */ }

    public static class Id
    {
        // Encapsulation
        public const string PublicStaticField = "BCC1000";
        // ...
    }

    public static class Category
    {
        public const string Encapsulation = "Encapsulation";
        // ...
    }

    public static class Option
    {
        public const string Prefix = "bcc";

        // the key is concatenated: the prefix and the diagnostic ID are never inlined
        public const string MaxReturnStatement =
            Prefix + "." + Id.MethodShouldNotHaveManyReturnStatements + ".max_return_statement";
        // ...
    }
}
```

### 4.2 ID format

`BCC` + four digits, where the first digit encodes the category:

| Prefix    | Category (`Constant.Category`)   | Folder                                       |
| --------- | -------------------------------- | -------------------------------------------- |
| `BCC1xxx` | `Encapsulation`                  | `Feature/`                                   |
| `BCC2xxx` | `SingleResponsibility`           | `Feature/SingleResponsibility/`              |
| `BCC3xxx` | `GoodPractice`                   | `Feature/GoodPractice/`                      |
| `BCC4xxx` | `CodeSmell`                      | `Feature/CodeSmell/`                         |

Rules:

* A new ID MUST take the **next free number of its block**; see
  [Appendix A](#appendix-a--diagnostic-id-registry) for the current state.
* An existing ID MUST NEVER be renumbered, reused, or reassigned to a different rule. IDs are a
  public contract: users write `// Disable BCC4002` in their source and reference IDs in
  `.editorconfig`.
* If a rule is removed, keep its constant, mark it `[Obsolete]` or add a comment saying it is
  retired, and document the removal in `changelog.md`. The number stays reserved forever.
* The constant **name** MUST describe the rule in PascalCase, without the `BCC` prefix and
  without the word `Id`/`Rule`/`Analyzer` (for example `MethodShouldNotHaveManyReturnStatements`).
* Use `public const string`, aligned in a single column inside a block, with a blank line
  between the category comment and the first constant of that block.

```csharp
// Good practice
public const string ReturnNull = "BCC3000";
public const string StaticClass = "BCC3001";
public const string NameTooLong = "BCC3007";
```

### 4.3 `Constant.ListOf`

`Constant.ListOf.Id` is built by **reflection** over every public field of `Constant.Id`, and it
feeds `FixableDiagnosticIds` of the code-fix provider. Consequences:

* Do not add a constant to `Constant.Id` that is *not* a diagnostic ID — it would silently
  become a fixable ID.
* Do not add fields of any type other than `const string` to `Constant.Id`.
* If you ever need a non-ID constant, put it in a different nested class.

### 4.4 No magic values in analyzer code

Numeric thresholds, word lists and string literals used by an analyzer MUST NOT be written
inline in the analyzer. They belong either to `Constant` (when they are part of the diagnostic
contract) or to `AnalyzerSettings` (when they are tunable) — see §7.

```csharp
// Wrong
if (name.Length > 26) { ... }

// Wrong - the compiled-in default is not the value the user configured
if (name.Length > Settings.MaxNameLength) { ... }

// Right
var settings = Settings.Resolve(context);
if (name.Length > settings.MaxNameLength) { ... }
```

---

## 5. Adding or changing an analyzer

A new analyzer is one logical change and MUST be delivered as one PR that touches, in this
order:

1. **`Constant.cs`** — add the ID (and a category constant if a new category is introduced).
2. **`Feature/<Category>/<Name>FeatureAnalyze.cs`** — add the analyzer class.
3. **`Feature/Base/`** — only if a genuinely new analysis strategy is needed; extend an existing
   base class otherwise.
4. **`BlowinCleanCodeAnalyzer.cs`** — register the feature in `Features`, under the comment of
   its category.
5. **`BlowinCleanCode.Test/<Category>/<Name>FeatureTest.cs`** — add the tests (§8).
6. **`README.md`** — add the rule to the correct list under *Available analyses* (§9).
7. **`changelog.md`** — add an entry under *New analyzers* (§9).
8. **`Constant.Option`, `AnalyzerSettings` and `AnalyzerSettingsReader`** — only when the rule
   needs a new tunable threshold (§7.3).

Checklist for the new class:

```csharp
namespace BlowinCleanCode.Feature.GoodPractice
{
    public sealed class MyRuleFeatureAnalyze : FeatureSyntaxNodeAnalyzerBase<MethodDeclarationSyntax>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; } = new DiagnosticDescriptor(
            Constant.Id.MyRule,
            title: "Short statement of the problem.",
            messageFormat: "The method '{0}' does something that is not allowed.",
            Constant.Category.GoodPractice,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "The method should be simpler.");
    }
}
```

* The class MUST be `sealed` unless it is designed for inheritance.
* The class name MUST be `<RuleName>FeatureAnalyze` for syntax analyzers and
  `<RuleName>FeatureSymbolAnalyze` for symbol analyzers, matching the file name.
* `DiagnosticDescriptor` MUST be created in a property initializer, with **named arguments** for
  `title`, `messageFormat`, `isEnabledByDefault` (and `description` when present), following the
  existing files.
* The category MUST be `Constant.Category.<...>`, not a string literal.
* The severity MUST be `DiagnosticSeverity.Warning` unless the PR explains why it differs.
* The `title` MUST be a sentence ending with a period, in English, describing what is wrong.
* The `messageFormat` MUST be localizable and use `{0}`, `{1}`, … placeholders only for names
  and values; numbers formatted by hand MUST be passed as arguments, not interpolated into the
  descriptor.
* Do not return diagnostics for compiler-generated code; `BlowinCleanCodeAnalyzer.Initialize`
  already calls `ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)`.
* Analyzers MUST honour the `// Disable BCCxxxx` skip mechanism by deriving from a
  `Feature*Base` class (which wires `SkipAnalyze`). If you implement `IFeature` directly you
  MUST call `SkipAnalyze` yourself.

---

## 6. Feature implementation rules

### 6.1 Choosing a base class

| Base class                                  | Use when                                                        |
| ------------------------------------------- | --------------------------------------------------------------- |
| `FeatureSymbolAnalyzeBase<TSymbol>`          | the rule is about a symbol (`IFieldSymbol`, `IMethodSymbol`, …)  |
| `FeatureSyntaxNodeAnalyzerBase<TSyntaxNode>` | the rule is about one syntax node kind                           |
| `FeatureSyntaxNodeAnalyzerBase`              | the rule needs several node kinds registered manually            |
| `TypeDeclarationSyntaxNodeAnalyzerBase`      | the rule is about class/struct declarations                      |
| `IdentifierNameSyntaxAnalyzerBase`           | the rule is about *names* (variables, parameters, arguments, members, type members) |
| `LambdaSyntaxNodeAnalyzerBase`               | the rule is about lambdas                                        |

Prefer the most specific base class. Do not copy the `Register` body of another analyzer.

### 6.2 Code style of the analyzer project

* Target framework `netstandard2.0`, C# 7.3-compatible syntax only (no `record`, no
  `switch` expressions, no nullable reference types). The analyzer project is compiled against
  `Microsoft.CodeAnalysis` 3.3.1 and MUST keep that compatibility.
* Use block-scoped namespaces and 4-space indentation, matching the existing files.
* Use `if (!x)` / early `return` instead of nested `else` blocks; analyzers are hot paths.
* Do not allocate in the analysis path when it can be avoided; prefer `ImmutableArray`,
  `StringSlice`, and the existing `Extension/` helpers over LINQ chains that allocate.
* Every syntax/semantic query MUST pass `context.CancellationToken` where the Roslyn API
  accepts one.
* New helper logic goes into `Model/` or `Extension/`, not into the analyzer class, if it is
  used by more than one analyzer.
* Extension classes MUST be named `<Target>Ext` and placed in
  `Extension/`, `Extension/SymbolExtension/` or `Extension/SyntaxExtension/` according to the
  extended type.

---

## 7. Settings, thresholds and analyzer options

### 7.1 The model

There is exactly one way for an analyzer to learn a threshold, and it is the same way for the
compiled-in default and for a user override:

```
Constant.Option.<name>          the key, declared once (see §4.1)
        |
AnalyzerSettingsReader          merges defaults with '.editorconfig' for one file
        ^
AnalyzerSettings.Resolve(...)   the only entry point
        ^
AnalyzerSettings.Instance       the compiled-in defaults
```

* **Defaults** live in `Model/Settings/AnalyzerSettings.cs`, one property per option, with a sane
  value.
* **Keys** live in `Constant.Option`, declared once for the whole solution. An analyzer never
  builds a key from a string, and never passes a key to `TryGetValue` itself.
* **Reading** always goes through `Settings.Resolve(context)`, which returns an
  `AnalyzerSettingsReader`. `context` is a `SyntaxNodeAnalysisContext` or a
  `SymbolAnalysisContext` — overloads exist for both, and the value is resolved from the file that
  contains the node or declares the symbol.

```csharp
protected override void Analyze(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax syntaxNode)
{
    var settings = Settings.Resolve(context);
    if (syntaxNode.Identifier.Text.Length > settings.MaxNameLength)
        ReportDiagnostic(context, syntaxNode.Identifier.GetLocation(), syntaxNode.Identifier.Text);
}
```

### 7.2 Rules

* An analyzer MUST NOT read `AnalyzerSettings.Instance.<Property>` directly. It reads the reader.
* Resolve once per analyzed node and store the reader in a local; do not call `Resolve` inside a
  loop over descendants or invocations, and do not read an option inside a LINQ lambda.
* An analyzer MUST NOT mutate `AnalyzerSettings`, and a test MUST NOT either — the singleton is
  process-wide, so a mutation leaks into every other test in the run.
* If an option is needed by more than one rule, do not invent a shared key ad hoc: propose a
  scoped key scheme in the PR first.
* A malformed value MUST NOT throw; `AnalyzerSettingsReader` falls back to the default. Keep that
  behaviour when adding a new option type.

### 7.3 Adding an option

1. Add the key to `Constant.Option`, built by concatenation of the prefix, the diagnostic ID of the
   rule the option belongs to, and the option name:

   ```csharp
   public const string MaxReturnStatement =
       Prefix + "." + Id.MethodShouldNotHaveManyReturnStatements + ".max_return_statement";
   ```

   The prefix comes from `Constant.Option.Prefix` and the identifier comes from `Constant.Id`, so
   neither is ever written as a literal. The resulting key is `bcc.BCC4006.max_return_statement`:
   it always names the rule it configures, and a rule that is renumbered cannot leave a stale key
   behind. The option name itself is lower case, in `snake_case`.
2. Add the default property to `AnalyzerSettings`.
3. Add the matching property to `AnalyzerSettingsReader`, reading the key and the default.
4. Read it in the analyzer through `Settings.Resolve(context)`.
5. Document the key in the option table of `README.md`.
6. Cover it with tests (§8.2).

### 7.4 Option keys are a public contract

Once released, an option key is as permanent as a diagnostic ID:

* an existing key MUST NEVER be renamed, and MUST NEVER change its meaning or its unit;
* a removed option keeps its constant, marked as retired, until the next major release;
* a new key takes a new name, following the format above.

---

## 8. Tests

**No analyzer, code fix or behaviour change may be merged without tests. This is not
negotiable.**

### 8.1 Location and naming

* Tests live in `BlowinCleanCode.Test`, in the folder matching the analyzer category
  (`CodeSmell/`, `GoodPractice/`, `SingleResponsibility/`, and the project root for
  `Encapsulation`).
* The test file MUST be named after the analyzer: `<RuleName>FeatureTest.cs`, containing the
  class `<RuleName>FeatureTest`.
* Pure unit tests of a helper type MUST be named after that type (`ComplexityWalkerTest`,
  `StringExtTest`).
* Test data MUST use xUnit `[Theory]` + `[InlineData]` with a verbatim string that contains a
  complete, compilable C# snippet, and the `{|#0:...|}` markup to mark the expected location.
* Expected diagnostics MUST be built from `Constant.Id.*`, never from a literal ID:

```csharp
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

[Theory]
[InlineData(@"... class {|#0:Test|} { ... }", "Test")]
public async Task Invalid(string test, string argument)
{
    var expected = VerifyCS.Diagnostic(Constant.Id.NameTooLong)
        .WithLocation(0)
        .WithArguments(argument);

    await VerifyCS.VerifyAnalyzerAsync(test, expected);
}

[Theory]
[InlineData(@"... clean snippet ...")]
public async Task Valid(string test)
{
    await VerifyCS.VerifyAnalyzerAsync(test);
}
```

### 8.2 Required coverage

Every PR that adds or changes an analyzer MUST include:

1. **Positive cases** — at least one snippet per supported syntax shape that *must* report the
   diagnostic, with the exact expected location and arguments.
2. **Negative cases** — snippets that look similar but MUST NOT report, especially:
   * the documented opt-out (`// Disable BCCxxxx` above the type or method),
   * names/members ending with the accepted suffix where the rule allows one,
   * struct vs class, expression-bodied vs block-bodied members, lambdas and local functions.
3. **Boundary cases** — exactly at the threshold (must not report) and one over it (must
   report), when the rule is threshold-based.
4. **Regression cases** — when the PR fixes a false positive or a false negative, the exact
   snippet from the issue MUST be added as a test case.
5. **Option cases** — when the PR adds or changes an analyzer option, the test MUST show that the
   option changes the outcome: the same source produces no diagnostic with the default and the
   expected diagnostic with the option set in `.editorconfig`.

Option behaviour is tested through the verifier overload that takes an `.editorconfig` body, and
the option keys themselves are covered by `AnalyzerOptionTest` and `AnalyzerSettingsReaderTest`:

```csharp
var editorConfig = "[*.cs]\n" + Constant.Option.MaxReturnStatementForReturnBool + " = 1";

await VerifyCS.VerifyAnalyzerAsync(source, editorConfig, expected);
```

Every PR that changes the code-fix provider or the comment-skip mechanism MUST include a
`CSharpCodeFixVerifier` test that verifies the round trip: analyzer reports → code fix applies →
analyzer no longer reports. The comment-skip path compares trivia text by exact equality, so the
round trip is the only way to prove the feature works.

### 8.3 Test quality rules

* Tests MUST NOT depend on the order of execution, on `AnalyzerSettings` mutation, or on files
  on disk.
* Tests MUST NOT assert on diagnostic counts alone; they MUST use `WithLocation` and
  `WithArguments`.
* Do not disable or delete an existing test to make a change pass. If an existing expectation is
  genuinely wrong, explain in the PR why, and keep the change visible.
* Keep one test file per analyzer. If a file exceeds roughly 1000 lines, split the data into
  partial test classes rather than one huge file.

---

## 9. Documentation

### 9.1 `README.md`

`README.md` is user-facing and MUST stay in sync with the code in the same PR.

* Every new analyzer MUST be added to the correct list under *Available analyses*, using the
  same wording style as the neighbors (short imperative phrase, no trailing period).
  The list is grouped by category and the grouping MUST match `Constant.Category`.
* Every analyzer removed or renamed MUST be removed from that list in the same PR.
* Any user-visible change of behaviour, threshold, message or default MUST be reflected in the
  README if the README describes it.
* Every new option MUST be added to the option table under *Configuring the thresholds*, with its
  key, its default and the rule it belongs to. The table and `Constant.Option` MUST list the same
  keys.
* The README MUST remain the source of truth for: how to install the package/extension, what
  each rule does, how to configure a threshold in `.editorconfig`, and how to suppress a rule
  (`// Disable BCCxxxx`). When you add a suppression mechanism, settings option or severity
  override, document it here.

### 9.2 `changelog.md`

`changelog.md` follows [Keep a Changelog](https://keepachangelog.com/) loosely:

```markdown
## [<version>] - <YYYY-MM-DD>

### New analyzers:

- <Rule name> ([#<issue>](https://github.com/blowin/BlowinCleanCode/issues/<issue>))

### Bug fixes:

- <What was broken and what it is now> ([#<issue>](...))

### Improvements:

- <What improved> ([commit](...))
```

* Add the entry in the **same PR** as the code change, under the next unreleased version.
* Every bullet MUST link either the issue (`#<n>`) or the commit.
* A bullet MUST describe user-visible behaviour, not implementation details.
* Do not reorder or rewrite existing released sections.
* If a change is not user-visible (internal refactoring, tests, CI), it SHOULD NOT get a
  changelog entry.

---

## 10. Packaging and versions

* The version number appears in several files and they MUST be updated together:
  * `BlowinCleanCode/BlowinCleanCode.Package/BlowinCleanCode.Package.csproj` (`PackageVersion`),
  * `BlowinCleanCode/BlowinCleanCode.Vsix/source.extension.vsixmanifest` (`Identity/@Version`),
  * `BlowinCleanCode/BlowinCleanCode.Analyzer.Vsix.VS22/source.extension.vsixmanifest`
    (`Identity/@Version`).
* Version bumps are done by the maintainer, in a dedicated `App - Updated version (release x.y.z)`
  commit that also adds the changelog heading. Feature PRs MUST NOT bump the version.
* Do not change the target frameworks, `PackageReference` versions of
  `Microsoft.CodeAnalysis`/`Microsoft.CodeAnalysis.CSharp`, or the VSIX installation ranges in a
  feature PR. Those are compatibility decisions and need their own PR with a rationale.
* The analyzer and code-fix assemblies are shipped as
  `analyzers/dotnet/cs/*.dll` inside the NuGet package. Do not add assemblies to that folder
  without updating `_AddAnalyzersToOutput` and the VSIX manifests.

---

## 11. Branches and commits

### Branch names

* Feature/fix branches: `<issue-number>` or `<topic>` (for example `#87`, `deeply-nested-fix`).
* Contributors working from a fork: `<nickname>/<topic>` (for example `webbrain/issue-91`).
* `master` is the only long-lived branch. A release MAY use a short-lived `vX.Y.Z` branch.

### Commit messages

The repository uses the following format:

```
<Area>[, <Component>] - <Action> <what changed> [#<issue>]
```

Examples from the history:

```
Analyzer, DeeplyNested - Improved * Highlight previous line with OpenBraceToken
CodeAnalyzer, MagicValue - Improved analyzer, added analysis of logical expressions #96
CodeAnalyzer - Fixed disable DeeplyNestedCodeFeatureAnalyze for root block
Documentation - Updated changelog.md
Application, Cleanup - Moved the settings to a folder and divided them into separate files
App - Updated version (release 2.6.0)
```

Rules:

* `<Area>` MUST be one of: `Analyzer`, `CodeAnalyzer`, `Application`, `Solution`, `Documentation`,
  `CI`, `App`.
* `<Component>` is optional and, for `Analyzer`/`CodeAnalyzer`, SHOULD name the affected feature
  class or rule (`MagicValue`, `Control Flag`, `DeeplyNested`).
* `<Action>` MUST start with an imperative past-tense verb used in the history: `Added`,
  `Fixed`, `Improved`, `Changed`, `Moved`, `Updated`, `Cleaned up`, `Removed`.
* Reference the issue number as `#<n>` at the end when one exists.
* The subject line MUST be at most 72 characters, imperative, and MUST NOT end with a period.
* Conventional Commits prefixes (`feat:`, `fix:`) are **not** used in this repository.
* One commit MUST contain one logical change. Do not mix a refactoring with a behaviour change
  or a formatting sweep.

---

## 12. Pull requests

### 12.1 Before opening

1. Rebase on the current `master`.
2. Run the tests locally (§3) and make sure they pass.
3. Walk through the [Definition of Done](#13-definition-of-done).
4. Make sure the diff contains no unrelated changes: no `*.csproj.user` files, no IDE settings,
   no `bin`/`obj`, no version bumps, no re-formatting of untouched files.

### 12.2 Title

The PR title MUST use the same format as a commit message:

```
CodeAnalyzer, MiddleMan - Fixed false positive for adapters #42
Documentation - Updated changelog.md
```

### 12.3 Description

The PR description MUST follow the template in
[Appendix B](#appendix-b--pr-description-template): what changed, why, how it was verified, and
which issue it closes. A PR without a description that explains the *why* will be sent back.

### 12.4 Size and scope

* One PR = one logical change (one new analyzer, one bug fix, one refactoring).
* A PR that adds an analyzer MUST NOT also refactor unrelated analyzers.
* Structural moves (renaming, moving files between folders) MUST be separate from behaviour
  changes so that reviewers can verify them mechanically.
* A PR larger than ~600 changed lines of production code SHOULD be split, unless it is a
  mechanical rename.

### 12.5 Documentation and tests in the same PR

A PR is incomplete — and will be rejected — if any of the following is missing:

* tests for the new/changed behaviour (§8);
* the `README.md` entry for a new/removed/renamed rule (§9.1);
* the `changelog.md` entry for a user-visible change (§9.2);
* a description of any new setting or suppression mechanism in `README.md`.

### 12.6 Review

* The `Build` workflow MUST be green.
* At least one maintainer approval is required. The automated
  `.github/workflows/code-review.yml` comment is advisory only; it does not replace a human
  review.
* Reviewers check, in this order: correctness of the analysis (no false positives on common
  code), ID/category correctness, test coverage, README/changelog updates, structure and
  naming.
* Address every review comment with a new commit or an explicit reply. Do not force-push over a
  review in progress without a note.
* Keep the branch up to date with `master` before merging.

---

## 13. Definition of Done

A change is done when **all** of the following hold:

- [ ] The analyzer is in the folder matching its `Constant.Category` and its ID block.
- [ ] The new ID is the next free number in its block and is registered in `Constant.Id`.
- [ ] The class name matches the file name and follows the
      `<Rule>FeatureAnalyze` / `<Rule>FeatureSymbolAnalyze` convention.
- [ ] The feature is registered in `BlowinCleanCodeAnalyzer.Features`, under the comment of its
      category, and the category comment matches `Constant.Category`.
- [ ] `DiagnosticDescriptor` uses `Constant.Id.*` and `Constant.Category.*`, named arguments and
      a `Warning` severity.
- [ ] No threshold, word list or user-visible string is inlined in the analyzer.
- [ ] Every threshold is read through `Settings.Resolve(context)`, never from
      `AnalyzerSettings.Instance` directly, and never inside a loop.
- [ ] A new option has a key in `Constant.Option`, a default in `AnalyzerSettings`, a property in
      `AnalyzerSettingsReader` and a row in the `README.md` option table.
- [ ] Tests prove that the option changes the outcome (§8.2).
- [ ] The analyzer honours the `// Disable BCCxxxx` comment.
- [ ] Tests cover positive, negative and boundary cases, and any false positive from the issue.
- [ ] `dotnet test BlowinCleanCode/BlowinCleanCode.Test/BlowinCleanCode.Test.csproj` passes.
- [ ] `README.md` lists the rule under the correct category.
- [ ] `changelog.md` has an entry under the unreleased version.
- [ ] Commit messages follow §11 and the PR description follows Appendix B.
- [ ] The diff contains no generated files, no user settings files and no unrelated edits.

---

## Appendix A — Diagnostic ID registry

| ID        | Constant (`Constant.Id`)                              | Category                | Rule (README wording)                          |
| --------- | ----------------------------------------------------- | ----------------------- | ---------------------------------------------- |
| `BCC1000` | `PublicStaticField`                                   | Encapsulation           | Don't use public static field                  |
| `BCC2000` | `CognitiveComplexity`                                 | Single responsibility   | Cognitive complexity of the method             |
| `BCC2001` | `ManyParametersMethod`                                | Single responsibility   | Many parameter in method                       |
| `BCC2002` | `MethodContainAnd`                                    | Single responsibility   | Method contain 'And'                           |
| `BCC2003` | `ControlFlag`                                         | Single responsibility   | Control flag                                   |
| `BCC2004` | `MethodContainALotOfDeclaration`                      | Single responsibility   | Method contains a lot of declaration           |
| `BCC2005` | `LongChainCall`                                       | Single responsibility   | Too many chained references                    |
| `BCC2006` | `LargeType`                                           | Single responsibility   | Large class                                    |
| `BCC2007` | `LargeNumberOfFields`                                 | Single responsibility   | Large number of fields in types                |
| `BCC2008` | `LongLambda`                                          | Single responsibility   | Lambda have too many lines                     |
| `BCC2009` | `LongMethod`                                          | Single responsibility   | Long method                                    |
| `BCC3000` | `ReturnNull`                                          | Good practice           | Don't return null                              |
| `BCC3001` | `StaticClass`                                         | Good practice           | Don't use static class                         |
| `BCC3002` | `DisposableMemberInNonDisposable`                     | Good practice           | Disposable member in non disposable class      |
| `BCC3003` | `SwitchStatementsShouldHaveAtLeast2CaseClauses`       | Good practice           | Switch statements should have at least 2 case clauses |
| `BCC3004` | `FinalizersShouldNotBeEmpty`                          | Good practice           | Finalizers should not be empty                 |
| `BCC3005` | `TypeThatProvideEqualsShouldImplementIEquatable`      | Good practice           | Type that provide `Equals` should implement `IEquatable` |
| `BCC3006` | `ThreadStaticFieldsShouldNotBeInitialized`            | Good practice           | `ThreadStatic` fields should not be initialized |
| `BCC3007` | `NameTooLong`                                         | Good practice           | Name is too long                               |
| `BCC3008` | `UseOnlyASCIICharactersForNames`                      | Good practice           | Use only ASCII characters for names            |
| `BCC4000` | `NestedTernaryOperator`                               | Code smell              | Nested ternary operator                        |
| `BCC4001` | `ComplexCondition`                                    | Code smell              | Complex condition                              |
| `BCC4002` | `MagicValue`                                          | Code smell              | Magic value                                    |
| `BCC4003` | `PreserveWholeObject`                                 | Code smell              | Preserve whole object                          |
| `BCC4004` | `HollowTypeName`                                      | Code smell              | Hollow type name                               |
| `BCC4005` | `DeeplyNestedCode`                                    | Code smell              | Deeply nested                                  |
| `BCC4006` | `MethodShouldNotHaveManyReturnStatements`             | Code smell              | Method should not have many return statements  |
| `BCC4007` | `SwitchShouldNotHaveALotOfCases`                      | Code smell              | Switch should not have a lot of cases          |
| `BCC4008` | `SwitchStatementsShouldNotBeNested`                   | Code smell              | Switch statements should not be nested         |
| `BCC4009` | `CatchShouldDoMoreThanRethrow`                        | Code smell              | Catch should do more than rethrow              |
| `BCC4010` | `EmptyDefaultClausesShouldBeRemoved`                  | Code smell              | Empty `default` clauses should be removed      |
| `BCC4011` | `MiddleMan`                                           | Code smell              | Middle man                                     |

**Next free IDs: `BCC1001`, `BCC2010`, `BCC3009`, `BCC4012`.**

---

## Appendix B — PR description template

Copy this into the pull request description and fill in every section.

````markdown
## What

<!-- One or two sentences. What does the code do now that it did not do before? -->

## Why

Closes #<issue>

<!-- The problem from the user's point of view, with a minimal code sample that shows it. -->

```csharp
// code that reproduces the issue / triggers the new rule
```

## How

<!-- Short notes on the approach, the chosen base class, the threshold, and any trade-off. -->

## Verification

- [ ] `dotnet test BlowinCleanCode/BlowinCleanCode.Test/BlowinCleanCode.Test.csproj` passes
- [ ] New/updated tests: `<test file names>`
- [ ] Full solution build verified in a VS 2022 Developer Command Prompt (only needed when the
      VSIX or package projects changed)

## Checklist

- [ ] ID is the next free number of the block and lives in `Constant.Id`
- [ ] Analyzer is in the folder matching `Constant.Category`, file name equals class name
- [ ] Feature registered in `BlowinCleanCodeAnalyzer.Features` under the right category comment
- [ ] Thresholds are read through `Settings.Resolve(context)`, no magic values inlined
- [ ] A new option has a key in `Constant.Option`, a default in `AnalyzerSettings`, a property in
      `AnalyzerSettingsReader` and a row in the `README.md` option table
- [ ] Diagnostic honours the `// Disable BCCxxxx` comment
- [ ] Positive, negative and boundary cases covered by tests
- [ ] Option changes covered by a test that sets the option in `.editorconfig`
- [ ] `README.md` updated
- [ ] `changelog.md` updated (user-visible changes only)
- [ ] No version bump, no unrelated files, no generated files in the diff

## Breaking change

<!-- "No", or describe the impact on users and what they must change. -->
````
