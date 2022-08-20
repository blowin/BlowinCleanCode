using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace BlowinCleanCode.Test.Verifiers
{
    public static partial class CSharpAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic()"/>
        public static DiagnosticResult Diagnostic()
            => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic();

        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(string)"/>
        public static DiagnosticResult Diagnostic(string diagnosticId)
            => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic(diagnosticId);

        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic(descriptor);

        /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
        public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                TestCode = source,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            try
            {
                await test.RunAsync(CancellationToken.None);
            }
            catch(Exception e)
            {
                throw new WithSourceMessageException(source, e);
            }
        }
        
        private sealed class WithSourceMessageException : Exception
        {
            private string _source;
            private Exception _ex;

            public override string Message => _source + Environment.NewLine + _ex.Message;

            public override IDictionary Data => _ex.Data;

            public override string Source => _ex.Source;

            public override string HelpLink
            {
                get => _ex.HelpLink;
                set => _ex.HelpLink = value;
            }

            public override string StackTrace => _ex.StackTrace;

            public WithSourceMessageException(string source, Exception ex)
            {
                _source = source;
                _ex = ex;
            }

            public override Exception GetBaseException() => _ex.GetBaseException();

            public override void GetObjectData(SerializationInfo info, StreamingContext context) => _ex.GetObjectData(info, context);
        }
    }
}
