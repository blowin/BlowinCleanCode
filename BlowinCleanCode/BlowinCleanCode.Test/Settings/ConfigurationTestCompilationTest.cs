using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;

namespace BlowinCleanCode.Test.Settings
{
    /// <summary>
    /// The configuration tests of <see cref="AnalyzerReadsConfigurationFeatureTest"/> run the analyzer
    /// on a synthesized compilation. This test proves that the compilation is healthy, so that a rule
    /// is never skipped for a reason unrelated to the option under test.
    /// </summary>
    public class ConfigurationTestCompilationTest
    {
        [Fact]
        public void The_source_used_by_the_configuration_tests_compiles()
        {
            var diagnostics = AnalyzerReadsConfigurationFeatureTest.DiagnosticsOfConfigurationSource();

            diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
                .Should().BeEmpty("a source with errors makes analyzers skip parts of the tree");
        }
    }
}
