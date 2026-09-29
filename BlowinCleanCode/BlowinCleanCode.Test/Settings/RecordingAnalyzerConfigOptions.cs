using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using BlowinCleanCode.Model.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Test.Settings
{
    /// <summary>
    /// An <see cref="AnalyzerConfigOptions"/> that answers from a fixed dictionary and records every
    /// key that was looked up through <see cref="TryGetValue"/>.
    /// <para>
    /// Because <see cref="AnalyzerSettingsReader"/> reads a value only through that method, the
    /// recorder observes a real analyzer run: a key is recorded exactly when the analyzer touched a
    /// reader property that reads it.
    /// </para>
    /// </summary>
    internal sealed class RecordingAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _values;
        private readonly string _keyPrefix;
        private readonly HashSet<string> _requestedKeys = new HashSet<string>();

        /// <param name="values">The configuration to answer from. May be empty.</param>
        /// <param name="keyPrefix">
        /// When set, only keys that start with this prefix are recorded. Roslyn itself asks the
        /// provider for keys such as <c>generated_code</c>, which are not analyzer options.
        /// </param>
        public RecordingAnalyzerConfigOptions(Dictionary<string, string> values, string keyPrefix = null)
        {
            _values = values;
            _keyPrefix = keyPrefix;
        }

        /// <summary>Keys that were looked up, normalized to lower case.</summary>
        public IReadOnlyCollection<string> RequestedKeys => _requestedKeys;

        public override bool TryGetValue(string key, out string value)
        {
            // A null key is not a lookup: Roslyn probes the provider with it and it carries no meaning.
            if (key != null && (_keyPrefix == null || key.StartsWith(_keyPrefix, StringComparison.OrdinalIgnoreCase)))
                _requestedKeys.Add(key.ToLowerInvariant());

            return _values.TryGetValue(key, out value);
        }
    }

    /// <summary>
    /// Helpers that drive <see cref="AnalyzerSettingsReader"/> directly, without an analyzer run.
    /// </summary>
    internal static class AnalyzerSettingsReaderProbe
    {
        /// <summary>
        /// Touches every public option property of the reader and returns the keys it asked for.
        /// </summary>
        public static IReadOnlyCollection<string> ReadAll(AnalyzerSettingsReader reader, RecordingAnalyzerConfigOptions options)
        {
            var properties = typeof(AnalyzerSettingsReader).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
                property.GetValue(reader);

            return options.RequestedKeys;
        }

        public static AnalyzerSettingsReader Reader(RecordingAnalyzerConfigOptions options)
            => AnalyzerSettings.Instance.Resolve(
                new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new FixedAnalyzerConfigOptionsProvider(options)),
                CSharpSyntaxTree.ParseText(string.Empty));
    }

    /// <summary>
    /// An <see cref="AnalyzerConfigOptionsProvider"/> that hands the same options to every file.
    /// </summary>
    internal sealed class FixedAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions _options;

        public FixedAnalyzerConfigOptionsProvider(AnalyzerConfigOptions options) => _options = options;

        public override AnalyzerConfigOptions GlobalOptions => _options;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _options;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _options;
    }

    /// <summary>
    /// The option keys declared in <see cref="Constant.Option"/>, resolved once by reflection.
    /// </summary>
    internal static class AnalyzerOptionKeys
    {
        private static readonly Dictionary<string, string> ByKey = Build();

        public static IReadOnlyCollection<string> All => ByKey.Keys;

        /// <summary>Field name to option key, for example <c>MaxReturnStatement</c> to <c>bcc.BCC4006.max_return_statement</c>.</summary>
        public static IReadOnlyDictionary<string, string> ByFieldName => ByKey;

        private static Dictionary<string, string> Build()
        {
            var result = new Dictionary<string, string>();
            var fields = typeof(Constant.Option).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.Name == nameof(Constant.Option.Prefix) || !field.IsLiteral || field.FieldType != typeof(string))
                    continue;

                var key = (string) field.GetValue(null);
                result.Add(key.ToLowerInvariant(), key);
            }

            return result;
        }
    }
}
