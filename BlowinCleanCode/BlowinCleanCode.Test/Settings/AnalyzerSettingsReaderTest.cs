using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BlowinCleanCode.Model.Settings;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace BlowinCleanCode.Test.Settings
{
    /// <summary>
    /// Covers the merge performed by <see cref="AnalyzerSettingsReader"/>: defaults, overrides,
    /// malformed values and the case-insensitive key lookup that '.editorconfig' guarantees.
    /// </summary>
    public class AnalyzerSettingsReaderTest
    {
        private static AnalyzerSettingsReader Reader(params (string Key, string Value)[] values)
        {
            var map = new Dictionary<string, string>(AnalyzerConfigOptions.KeyComparer);
            foreach (var (key, value) in values)
                map[key] = value;

            return Analyze(map);
        }

        private static AnalyzerSettingsReader Analyze(Dictionary<string, string> map)
        {
            var provider = new FakeAnalyzerConfigOptionsProvider(new FakeAnalyzerConfigOptions(map));
            var options = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, provider);

            return AnalyzerSettings.Instance.Resolve(options, CSharpSyntaxTree.ParseText(string.Empty));
        }

        [Fact]
        public void Without_configuration_the_compiled_in_defaults_are_returned()
        {
            var reader = Reader();
            var defaults = AnalyzerSettings.Instance;

            reader.MaxReturnStatement.Should().Be(defaults.MaxReturnStatement);
            reader.MaxReturnStatementForReturnBool.Should().Be(defaults.MaxReturnStatementForReturnBool);
            reader.MaxNameLength.Should().Be(defaults.MaxNameLength);
            reader.MaxDeeplyNested.Should().Be(defaults.MaxDeeplyNested);
            reader.ChainCallSettings.MaxCall.Should().Be(defaults.ChainCallSettings.MaxCall);
            reader.LargeClass.MaxMethodThreshold.Should().Be(defaults.LargeClass.MaxMethodThreshold);
            reader.CognitiveComplexity.MinHighComplexity.Should().Be(defaults.CognitiveComplexity.MinHighComplexity);
        }

        [Fact]
        public void An_integer_option_overrides_the_default()
        {
            Reader((Constant.Option.MaxReturnStatement, "6")).MaxReturnStatement.Should().Be(6);
        }

        [Fact]
        public void An_option_key_is_case_insensitive()
        {
            Reader((Constant.Option.MaxReturnStatement.ToUpperInvariant(), "6")).MaxReturnStatement.Should().Be(6);
        }

        [Fact]
        public void A_malformed_value_falls_back_to_the_default()
        {
            Reader((Constant.Option.MaxReturnStatement, "not a number")).MaxReturnStatement
                .Should().Be(AnalyzerSettings.Instance.MaxReturnStatement);
        }

        [Fact]
        public void A_double_option_is_parsed_with_the_invariant_culture()
        {
            Reader((Constant.Option.LargeClassPrivateMethodThreshold, "0.25")).LargeClass.PrivateMethodThreshold
                .Should().Be(0.25);
        }

        [Fact]
        public void A_boolean_option_overrides_the_default()
        {
            Reader((Constant.Option.ChainCallMustIncludeFluentInterfaceCall, "true")).ChainCallSettings
                .MaxCallMustIncludeFluentInterfaceCall.Should().BeTrue();
        }

        [Fact]
        public void A_nullable_option_can_be_set()
        {
            Reader((Constant.Option.ChainCallMaxFluentInterfaceCall, "3")).ChainCallSettings
                .MaxFluentInterfaceCall.Should().Be(3);
        }

        [Fact]
        public void A_nullable_option_can_be_unset_with_an_empty_value()
        {
            Reader((Constant.Option.ChainCallMaxFluentInterfaceCall, "")).ChainCallSettings
                .MaxFluentInterfaceCall.Should().BeNull();
        }

        [Fact]
        public void Nested_options_are_read_independently_of_the_parent()
        {
            var reader = Reader(
                (Constant.Option.ChainCallMaxCall, "9"),
                (Constant.Option.CognitiveComplexityMinLowComplexity, "3"));

            reader.ChainCallSettings.MaxCall.Should().Be(9);
            reader.CognitiveComplexity.MinLowComplexity.Should().Be(3);
            reader.CognitiveComplexity.MinMiddleComplexity.Should().Be(AnalyzerSettings.Instance.CognitiveComplexity.MinMiddleComplexity);
        }

        [Fact]
        public void Hollow_type_name_full_match_words_can_be_replaced()
        {
            var dictionary = Reader((Constant.Option.HollowTypeNameFullMatchWords, "Dto, Model")).HollowTypeNameDictionary;

            dictionary.Where(e => e.ValidateWhenFullMatch).Select(e => e.Word)
                .Should().BeEquivalentTo(new[] { "Dto", "Model" });
            dictionary.Where(e => !e.ValidateWhenFullMatch).Select(e => e.Word)
                .Should().BeEquivalentTo(new[] { "Manager" });
        }

        [Fact]
        public void Hollow_type_name_suffix_words_can_be_cleared()
        {
            var dictionary = Reader((Constant.Option.HollowTypeNameSuffixWords, "")).HollowTypeNameDictionary;

            dictionary.Where(e => !e.ValidateWhenFullMatch).Should().BeEmpty();
            dictionary.Where(e => e.ValidateWhenFullMatch).Select(e => e.Word)
                .Should().BeEquivalentTo(new[] { "Helper", "Util", "Utils", "Utility", "Utilities", "Info", "Data" });
        }

        [Fact]
        public void Without_a_syntax_tree_the_defaults_are_returned()
        {
            var provider = new FakeAnalyzerConfigOptionsProvider(new FakeAnalyzerConfigOptions(new Dictionary<string, string>()));
            var options = new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, provider);

            AnalyzerSettings.Instance.Resolve(options, null).MaxReturnStatement
                .Should().Be(AnalyzerSettings.Instance.MaxReturnStatement);
        }

        [Fact]
        public void Without_options_the_defaults_are_returned()
        {
            AnalyzerSettings.Instance.Resolve(null, CSharpSyntaxTree.ParseText(string.Empty)).MaxReturnStatement
                .Should().Be(AnalyzerSettings.Instance.MaxReturnStatement);
        }

        [Fact]
        public void A_default_reader_behaves_like_an_unconfigured_one()
        {
            default(AnalyzerSettingsReader).MaxReturnStatement
                .Should().Be(AnalyzerSettings.Instance.MaxReturnStatement);
        }

        private sealed class FakeAnalyzerConfigOptions : AnalyzerConfigOptions
        {
            private readonly Dictionary<string, string> _values;

            public FakeAnalyzerConfigOptions(Dictionary<string, string> values) => _values = values;

            public override bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value);
        }

        private sealed class FakeAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            private readonly AnalyzerConfigOptions _options;

            public FakeAnalyzerConfigOptionsProvider(AnalyzerConfigOptions options) => _options = options;

            public override AnalyzerConfigOptions GlobalOptions => _options;

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _options;

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _options;
        }
    }
}
