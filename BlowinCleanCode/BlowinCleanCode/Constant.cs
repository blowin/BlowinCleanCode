using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlowinCleanCode
{
    public static class Constant
    {
        public static class ListOf
        {
            private static readonly Lazy<HashSet<string>> IdSet = new Lazy<HashSet<string>>(() => new HashSet<string>(typeof(Constant.Id).GetFields().Select(e => (string) e.GetValue(null))), LazyThreadSafetyMode.None);
            
            public static HashSet<string> Id => IdSet.Value;
        }

        public static class Id
        {
            // Encapsulation
            public const string PublicStaticField = "BCC1000";

            // Single responsibility
            public const string CognitiveComplexity = "BCC2000";
            public const string ManyParametersMethod = "BCC2001";
            public const string MethodContainAnd = "BCC2002";
            public const string ControlFlag = "BCC2003";
            public const string MethodContainALotOfDeclaration = "BCC2004";
            public const string LongChainCall = "BCC2005";
            public const string LargeType = "BCC2006";
            public const string LargeNumberOfFields = "BCC2007";
            public const string LongLambda = "BCC2008";
            public const string LongMethod = "BCC2009";

            // Good Practice
            public const string ReturnNull = "BCC3000";
            public const string StaticClass = "BCC3001";
            public const string DisposableMemberInNonDisposable = "BCC3002";
            public const string SwitchStatementsShouldHaveAtLeast2CaseClauses ="BCC3003";
            public const string FinalizersShouldNotBeEmpty ="BCC3004";
            public const string TypeThatProvideEqualsShouldImplementIEquatable ="BCC3005";
            public const string ThreadStaticFieldsShouldNotBeInitialized ="BCC3006";
            public const string NameTooLong = "BCC3007";
            public const string UseOnlyASCIICharactersForNames = "BCC3008";

            // Code smell
            public const string NestedTernaryOperator = "BCC4000";
            public const string ComplexCondition = "BCC4001";
            public const string MagicValue = "BCC4002";
            public const string PreserveWholeObject = "BCC4003";
            public const string HollowTypeName = "BCC4004";
            public const string DeeplyNestedCode = "BCC4005";
            public const string MethodShouldNotHaveManyReturnStatements = "BCC4006";
            public const string SwitchShouldNotHaveALotOfCases = "BCC4007";
            public const string SwitchStatementsShouldNotBeNested = "BCC4008";
            public const string CatchShouldDoMoreThanRethrow = "BCC4009";
            public const string EmptyDefaultClausesShouldBeRemoved = "BCC4010";
            public const string MiddleMan = "BCC4011";
        }

        public static class Category
        {
            public const string Encapsulation = "Encapsulation";

            public const string SingleResponsibility = "Single responsibility";
            
            public const string GoodPractice = "Good practice";
            
            public const string CodeSmell = "Code smell";
        }

        /// <summary>
        /// Keys of the options a user can redefine in '.editorconfig'.
        /// This is the only place where an option name is declared: an analyzer reads the value
        /// through <see cref="Model.Settings.AnalyzerSettings.Resolve(Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext)"/>
        /// and never builds a key by hand.
        /// <para>
        /// A key is built by concatenation as
        /// <c><see cref="Prefix"/> + "." + <see cref="Id"/> of the rule + "." + option name</c>,
        /// for example <c>bcc.BCC4006.max_return_statement = 6</c>. Neither the prefix nor the
        /// identifier is ever inlined, so a key follows the rule it configures automatically.
        /// The lookup is case-insensitive, so any casing works in '.editorconfig'.
        /// </para>
        /// </summary>
        public static class Option
        {
            /// <summary>Prefix of every option key. A key is never declared with a literal prefix.</summary>
            public const string Prefix = "bcc";

            // Encapsulation has no tunable option.

            // Single responsibility
            public const string MaxMethodParameter = Prefix + "." + Id.ManyParametersMethod + ".max_method_parameter";
            public const string MaxMethodDeclaration = Prefix + "." + Id.MethodContainALotOfDeclaration + ".max_method_declaration";
            public const string MaxCountOfLinesInMethod = Prefix + "." + Id.LongMethod + ".max_count_of_lines_in_method";
            public const string MaxLambdaCountOfLines = Prefix + "." + Id.LongLambda + ".max_lambda_count_of_lines";
            public const string MaxNumberOfField = Prefix + "." + Id.LargeNumberOfFields + ".max_number_of_field";
            public const string MaxSwitchCaseCount = Prefix + "." + Id.SwitchShouldNotHaveALotOfCases + ".max_switch_case_count";
            public const string ChainCallMaxCall = Prefix + "." + Id.LongChainCall + ".max_call";
            public const string ChainCallMaxFluentInterfaceCall = Prefix + "." + Id.LongChainCall + ".max_fluent_interface_call";
            public const string ChainCallMustIncludeFluentInterfaceCall = Prefix + "." + Id.LongChainCall + ".must_include_fluent_interface_call";
            public const string CognitiveComplexityMinLowComplexity = Prefix + "." + Id.CognitiveComplexity + ".min_low_complexity";
            public const string CognitiveComplexityMinMiddleComplexity = Prefix + "." + Id.CognitiveComplexity + ".min_middle_complexity";
            public const string CognitiveComplexityMinHighComplexity = Prefix + "." + Id.CognitiveComplexity + ".min_high_complexity";
            public const string LargeClassMaxMethodThreshold = Prefix + "." + Id.LargeType + ".max_method_threshold";
            public const string LargeClassPrivateMethodThreshold = Prefix + "." + Id.LargeType + ".private_method_threshold";
            public const string LargeClassNonPrivateMethodThreshold = Prefix + "." + Id.LargeType + ".non_private_method_threshold";

            // Good practice
            public const string MaxNameLength = Prefix + "." + Id.NameTooLong + ".max_name_length";

            // Code smell
            public const string MaxCountOfCondition = Prefix + "." + Id.ComplexCondition + ".max_count_of_condition";
            public const string MaxDeeplyNested = Prefix + "." + Id.DeeplyNestedCode + ".max_deeply_nested";
            public const string MaxPreserveWholeObjectCount = Prefix + "." + Id.PreserveWholeObject + ".max_preserve_whole_object_count";
            public const string MaxReturnStatement = Prefix + "." + Id.MethodShouldNotHaveManyReturnStatements + ".max_return_statement";
            public const string MaxReturnStatementForReturnBool = Prefix + "." + Id.MethodShouldNotHaveManyReturnStatements + ".max_return_statement_for_return_bool";
            public const string HollowTypeNameFullMatchWords = Prefix + "." + Id.HollowTypeName + ".full_match_words";
            public const string HollowTypeNameSuffixWords = Prefix + "." + Id.HollowTypeName + ".suffix_words";
        }
    }
}