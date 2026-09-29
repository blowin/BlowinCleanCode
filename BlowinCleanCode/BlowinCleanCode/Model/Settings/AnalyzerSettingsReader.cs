using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Model.Settings
{
    /// <summary>
    /// Effective values of every tunable analyzer option for one source file.
    /// <para>
    /// An instance is produced by
    /// <see cref="AnalyzerSettings.Resolve(Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext)"/> and
    /// merges the compiled-in defaults with the values the user set in '.editorconfig'.
    /// The value of an option is read from the file being analyzed, so two files of the same
    /// solution may use different thresholds.
    /// </para>
    /// <para>
    /// This is a value type: creating a reader costs nothing on the heap, and an option that is
    /// never read is never parsed. An unrecognized or malformed value never throws - the compiled-in
    /// default is used instead.
    /// </para>
    /// </summary>
    public readonly struct AnalyzerSettingsReader
    {
        private readonly AnalyzerSettings _defaults;
        private readonly AnalyzerConfigOptions _options;

        internal AnalyzerSettingsReader(AnalyzerSettings defaults, AnalyzerConfigOptions options)
        {
            _defaults = defaults;
            _options = options;
        }

        /// <summary>
        /// Compiled-in values. Also makes <c>default(AnalyzerSettingsReader)</c> behave like a reader
        /// that has no configuration at all.
        /// </summary>
        private AnalyzerSettings Defaults => _defaults ?? AnalyzerSettings.Instance;

        // Single responsibility

        public int MaxMethodParameter => ReadInt32(Constant.Option.MaxMethodParameter, Defaults.MaxMethodParameter);

        public int MaxMethodDeclaration => ReadInt32(Constant.Option.MaxMethodDeclaration, Defaults.MaxMethodDeclaration);

        public int MaxCountOfLinesInMethod => ReadInt32(Constant.Option.MaxCountOfLinesInMethod, Defaults.MaxCountOfLinesInMethod);

        public int MaxLambdaCountOfLines => ReadInt32(Constant.Option.MaxLambdaCountOfLines, Defaults.MaxLambdaCountOfLines);

        public int MaxNumberOfField => ReadInt32(Constant.Option.MaxNumberOfField, Defaults.MaxNumberOfField);

        public int MaxSwitchCaseCount => ReadInt32(Constant.Option.MaxSwitchCaseCount, Defaults.MaxSwitchCaseCount);

        public AnalyzerChainCallSettings ChainCallSettings => new AnalyzerChainCallSettings
        {
            MaxCall = ReadInt32(Constant.Option.ChainCallMaxCall, Defaults.ChainCallSettings.MaxCall),
            MaxFluentInterfaceCall = ReadNullableInt32(Constant.Option.ChainCallMaxFluentInterfaceCall, Defaults.ChainCallSettings.MaxFluentInterfaceCall),
            MaxCallMustIncludeFluentInterfaceCall = ReadBoolean(Constant.Option.ChainCallMustIncludeFluentInterfaceCall, Defaults.ChainCallSettings.MaxCallMustIncludeFluentInterfaceCall),
        };

        public CognitiveComplexitySettings CognitiveComplexity => new CognitiveComplexitySettings
        {
            MinLowComplexity = ReadInt32(Constant.Option.CognitiveComplexityMinLowComplexity, Defaults.CognitiveComplexity.MinLowComplexity),
            MinMiddleComplexity = ReadInt32(Constant.Option.CognitiveComplexityMinMiddleComplexity, Defaults.CognitiveComplexity.MinMiddleComplexity),
            MinHighComplexity = ReadInt32(Constant.Option.CognitiveComplexityMinHighComplexity, Defaults.CognitiveComplexity.MinHighComplexity),
        };

        public AnalyzerLargeClassSettings LargeClass => new AnalyzerLargeClassSettings
        {
            MaxMethodThreshold = ReadInt32(Constant.Option.LargeClassMaxMethodThreshold, Defaults.LargeClass.MaxMethodThreshold),
            PrivateMethodThreshold = ReadDouble(Constant.Option.LargeClassPrivateMethodThreshold, Defaults.LargeClass.PrivateMethodThreshold),
            NonPrivateMethodThreshold = ReadDouble(Constant.Option.LargeClassNonPrivateMethodThreshold, Defaults.LargeClass.NonPrivateMethodThreshold),
        };

        // Good practice

        public int MaxNameLength => ReadInt32(Constant.Option.MaxNameLength, Defaults.MaxNameLength);

        // Code smell

        public int MaxCountOfCondition => ReadInt32(Constant.Option.MaxCountOfCondition, Defaults.MaxCountOfCondition);

        public int MaxDeeplyNested => ReadInt32(Constant.Option.MaxDeeplyNested, Defaults.MaxDeeplyNested);

        public int MaxPreserveWholeObjectCount => ReadInt32(Constant.Option.MaxPreserveWholeObjectCount, Defaults.MaxPreserveWholeObjectCount);

        public int MaxReturnStatement => ReadInt32(Constant.Option.MaxReturnStatement, Defaults.MaxReturnStatement);

        public int MaxReturnStatementForReturnBool => ReadInt32(Constant.Option.MaxReturnStatementForReturnBool, Defaults.MaxReturnStatementForReturnBool);

        /// <summary>
        /// Words that make a type name hollow. Each entry says whether the word is only accepted
        /// as a full match.
        /// <para>
        /// Setting either of the two options replaces that part of the list, so
        /// <c>bcc.hollow_type_name.suffix_words =</c> (an empty value) disables the suffix check.
        /// </para>
        /// </summary>
        public (string Word, bool ValidateWhenFullMatch)[] HollowTypeNameDictionary
        {
            get
            {
                var fullMatchWords = ReadWords(Constant.Option.HollowTypeNameFullMatchWords);
                var suffixWords = ReadWords(Constant.Option.HollowTypeNameSuffixWords);

                if (fullMatchWords == null && suffixWords == null)
                    return Defaults.HollowTypeNameDictionary;

                var result = new List<(string Word, bool ValidateWhenFullMatch)>();

                foreach (var word in fullMatchWords ?? WordsOf(Defaults.HollowTypeNameDictionary, true))
                    result.Add((word, true));

                foreach (var word in suffixWords ?? WordsOf(Defaults.HollowTypeNameDictionary, false))
                    result.Add((word, false));

                return result.ToArray();
            }
        }

        private static List<string> WordsOf((string Word, bool ValidateWhenFullMatch)[] dictionary, bool validateWhenFullMatch)
        {
            var result = new List<string>();
            foreach (var (word, fullMatch) in dictionary)
            {
                if (fullMatch == validateWhenFullMatch)
                    result.Add(word);
            }

            return result;
        }

        private bool TryGetValue(string key, out string value)
        {
            if (_options == null)
            {
                value = null;
                return false;
            }

            return _options.TryGetValue(key, out value) && value != null;
        }

        private int ReadInt32(string key, int defaultValue)
        {
            if (TryGetValue(key, out var raw) &&
                int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                return value;

            return defaultValue;
        }

        private int? ReadNullableInt32(string key, int? defaultValue)
        {
            if (!TryGetValue(key, out var raw))
                return defaultValue;

            // An explicit empty value means "not set".
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private double ReadDouble(string key, double defaultValue)
        {
            if (TryGetValue(key, out var raw) &&
                double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return value;

            return defaultValue;
        }

        private bool ReadBoolean(string key, bool defaultValue)
        {
            if (TryGetValue(key, out var raw) && bool.TryParse(raw.Trim(), out var value))
                return value;

            return defaultValue;
        }

        /// <summary>Returns <c>null</c> when the option is not set at all.</summary>
        private List<string> ReadWords(string key)
        {
            if (!TryGetValue(key, out var raw))
                return null;

            var result = new List<string>();
            foreach (var part in raw.Split(','))
            {
                var word = part.Trim();
                if (word.Length != 0)
                    result.Add(word);
            }

            return result;
        }
    }
}
