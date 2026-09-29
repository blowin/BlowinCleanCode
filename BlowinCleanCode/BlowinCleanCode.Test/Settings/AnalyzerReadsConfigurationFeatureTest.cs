using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlowinCleanCode.Feature;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.Settings
{
    /// <summary>
    /// Proves that the analyzers really read their configuration, and that a value set in
    /// '.editorconfig' reaches the analyzer that owns the rule.
    /// <para>
    /// The tests work on the analyzer level: every feature of
    /// <see cref="BlowinCleanCodeAnalyzer"/> is registered on its own, and the option keys it asks
    /// for are observed through <see cref="AnalyzerSettingsReader"/>. The source under analysis is
    /// deliberately trivial — what is tested is the option plumbing, not the rules.
    /// </para>
    /// </summary>
    public class AnalyzerReadsConfigurationFeatureTest
    {
        private const string Source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace ConsoleApplication1
    {
        public class Test
        {
            public int Run(int value)
            {
                var items = new List<int>();
                items.Add(value + 3);

                if (items.Count > 0 && value > 1 && value < 100 && items.Any() && items[0] != 0)
                    items.Clear();

                for (var i = 0; i < items.Count; i++)
                    items.RemoveAt(i);

                switch (value)
                {
                    case 1:
                        return items.Count;
                    case 2:
                        return 0;
                    case 3:
                        return 1;
                    case 4:
                        return 2;
                    case 5:
                        return 3;
                    case 6:
                        return 4;
                    case 7:
                        return 5;
                }

                var call = value.ToString().Trim().ToUpperInvariant().Replace(""a"", ""b"").Substring(0).PadLeft(9);

                // BCC4003 reads its option only for an invocation that passes several members of one
                // object, so the source has to contain such a call.
                var box = new Holder { A = value, B = value + 1, C = value + 2 };
                items.Add(Sum(box.A, box.B, box.C));

                if (items.Count > 0)
                {
                    if (value > 1)
                    {
                        if (value < 100)
                        {
                            items.Clear();
                        }
                    }
                }

                var run = new Func<int, int>(x =>
                {
                    var a = x + 1;
                    var b = a + 1;
                    var c = b + 1;
                    var d = c + 1;
                    var e = d + 1;
                    var f = e + 1;
                    var g = f + 1;
                    var h = g + 1;
                    var i = h + 1;
                    var j = i + 1;
                    items.Add(j);
                    return j;
                });

                if (run(value) > 300)
                    return 300;

                if (run(value) > 200)
                    return 200;

                if (run(value) > 100)
                    return 100;

                if (run(value) > 50)
                    return 50;

                if (run(value) > 25)
                    return 25;

                // The rule reads a second option for a method that returns 'bool', and it also needs an
                // early return of 'false' for that option to be touched.
                if (value > 1000)
                    return 26;

                return call.Length == 0 ? 27 : 28;
            }

            public bool RunBool(int value)
            {
                if (value > 1000)
                    return false;

                return true;
            }

            public static int Sum(int a, int b, int c) => a + b + c;
        }

        public class Holder
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }
    }";

        /// <summary>
        /// The source the configuration tests analyze. It is deliberately trivial, but it has to contain
        /// the syntax each rule needs before that rule reads its option: a switch, a lambda, nested
        /// blocks, a long chain, several returns, a method returning 'bool', an invocation that passes
        /// several members of one object, and so on.
        /// </summary>
        internal static string ConfigurationSource => Source;

        /// <summary>
        /// Compiles the source used by the configuration tests and returns its compiler diagnostics,
        /// so that a broken source is reported as a failing test instead of a silently skipped rule.
        /// </summary>
        internal static ImmutableArray<Diagnostic> DiagnosticsOfConfigurationSource()
            => TestCompilation(Source).GetDiagnostics();

        [Fact]
        public void Every_declared_option_is_read_by_an_analyzer()
        {
            var requestedKeys = RequestedKeysOfEveryFeature(Source);

            var notRead = AnalyzerOptionKeys.All.Except(requestedKeys).ToArray();

            notRead.Should().BeEmpty("every option declared in Constant.Option must be read by some analyzer");
        }

        [Fact]
        public void No_analyzer_reads_an_undeclared_option()
        {
            var requestedKeys = RequestedKeysOfEveryFeature(Source);

            requestedKeys.Should().BeSubsetOf(AnalyzerOptionKeys.All,
                "an analyzer must build its option keys from Constant.Option");
        }

        /// <summary>
        /// Proves that the recorded keys really come from the analyzer run: on a source that contains
        /// no lambda, no deeply nested statements, no switch and no method returning 'bool', those four
        /// options must not be touched, while the options of the same run still are.
        /// </summary>
        [Fact]
        public void An_option_is_read_only_when_the_source_needs_it()
        {
            var requestedKeys = RequestedKeysOfEveryFeature(MinimalSource);

            requestedKeys.Should().NotContain(Constant.Option.MaxLambdaCountOfLines);
            requestedKeys.Should().NotContain(Constant.Option.MaxDeeplyNested);
            requestedKeys.Should().NotContain(Constant.Option.MaxSwitchCaseCount);
            requestedKeys.Should().NotContain(Constant.Option.MaxReturnStatementForReturnBool);

            requestedKeys.Should().NotBeEmpty();
        }

        [Fact]
        public void The_reader_knows_every_declared_option()
        {
            var options = new RecordingAnalyzerConfigOptions(Values(), Constant.Option.Prefix + ".");
            var reader = AnalyzerSettingsReaderProbe.Reader(options);

            var requestedKeys = AnalyzerSettingsReaderProbe.ReadAll(reader, options);

            requestedKeys.Should().BeEquivalentTo(AnalyzerOptionKeys.All);
        }

        [Fact]
        public async Task An_option_of_a_symbol_analyzer_is_applied()
        {
            var source = @"
    using System;

    namespace ConsoleApplication1
    {
        public class Test
        {
            public void {|#0:Run|}(int a, int b, int c)
            {
            }
        }
    }";

            var editorConfig = "[*.cs]\n" + Constant.Option.MaxMethodParameter + " = 2";

            var expected = VerifyCS.Diagnostic(Constant.Id.ManyParametersMethod)
                .WithLocation(0)
                .WithArguments("Run");

            await VerifyCS.VerifyAnalyzerAsync(source, editorConfig, expected);
        }

        /// <summary>
        /// A source that no option of a lambda, a switch, a nesting or a 'bool' return can influence.
        /// </summary>
        private const string MinimalSource = @"
    using System;
    using System.Collections.Generic;

    namespace ConsoleApplication1
    {
        public class Test
        {
            public void Run(int value)
            {
                var items = new List<int>();
                items.Add(value);
            }
        }
    }";

        private const string SourceWithConditionMarkup = @"    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace ConsoleApplication1
    {
        public class Test
        {
            public void Run(int value)
            {
                var items = new List<int>();
                items.Add(value + 3);

                if ({|#0:items.Count > 0 && value > 1|} && value < 100 && items.Any() && items[0] != 0)
                    items.Clear();
            }
        }
    }";

        /// <summary>
        /// Runs every registered feature on its own and collects the option keys it reads.
        /// A feature is registered in isolation so that the recorded keys belong to it and are not
        /// mixed with the keys of the whole analyzer.
        /// </summary>
        private static IReadOnlyCollection<string> RequestedKeysOfEveryFeature(string source)
        {
            var requested = new HashSet<string>();

            foreach (var feature in Features())
            {
                // Roslyn itself asks this provider for "generated_code" and the severity keys; only the
                // keys of this analyzer's own namespace are interesting here.
                var options = new RecordingAnalyzerConfigOptions(Values(), Constant.Option.Prefix + ".");
                var analyzerOptions = new AnalyzerOptions(
                    ImmutableArray<AdditionalText>.Empty,
                    new FixedAnalyzerConfigOptionsProvider(options));

                var compilation = TestCompilation(source);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create<DiagnosticAnalyzer>(new SingleFeatureAnalyzer(feature)),
                    analyzerOptions);

                // The diagnostics themselves are irrelevant here, but the run must actually happen.
                withAnalyzers.GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();

                foreach (var key in options.RequestedKeys)
                    requested.Add(key);
            }

            return requested;
        }

        /// <summary>
        /// Every feature of the analyzer, resolved from the diagnostics the analyzer supports.
        /// </summary>
        private static IEnumerable<IFeature> Features()
        {
            var analyzer = new BlowinCleanCodeAnalyzer();

            return analyzer.SupportedDiagnostics.Select(FindFeature);
        }

        private static IFeature FindFeature(DiagnosticDescriptor descriptor)
        {
            var featureType = typeof(IFeature);
            var assembly = featureType.Assembly;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !featureType.IsAssignableFrom(type))
                    continue;

                var feature = (IFeature) System.Activator.CreateInstance(type);
                if (feature.DiagnosticDescriptor.Id == descriptor.Id)
                    return feature;
            }

            throw new System.InvalidOperationException("No feature found for " + descriptor.Id);
        }

        private static Compilation TestCompilation(string source)
        {
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source, path: "/0/Test0.cs");

            // The whole reference pack is used on purpose: an analyzer run over a compilation with
            // errors skips parts of the tree and would read fewer options than the rule really needs.
            var references = ReferencePaths()
                .Select(path => (MetadataReference) MetadataReference.CreateFromFile(path))
                .ToArray();

            return Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
                "Test",
                new[] { tree },
                references,
                new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        private static IEnumerable<string> ReferencePaths()
        {
            // TRUSTED_PLATFORM_ASSEMBLIES lists the runtime assemblies of the test host, which is the
            // full reference set of the framework the test runs on.
            var trusted = ((string) System.AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
                .Split(System.IO.Path.PathSeparator)
                .Where(path => path.Length != 0);

            return trusted
                .Concat(new[]
                {
                    typeof(object).Assembly.Location,
                    typeof(Enumerable).Assembly.Location,
                    typeof(List<>).Assembly.Location,
                    typeof(Func<>).Assembly.Location,
                })
                .Distinct();
        }

        private static Dictionary<string, string> Values() => new Dictionary<string, string>(AnalyzerConfigOptions.KeyComparer);

        /// <summary>
        /// Wraps a feature into an analyzer that registers only that feature.
        /// </summary>
        private sealed class SingleFeatureAnalyzer : DiagnosticAnalyzer
        {
            private readonly IFeature _feature;

            public SingleFeatureAnalyzer(IFeature feature) => _feature = feature;

            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
                => ImmutableArray.Create(_feature.DiagnosticDescriptor);

            public override void Initialize(AnalysisContext context)
            {
                context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
                context.EnableConcurrentExecution();
                _feature.Register(context);
            }
        }
    }
}
