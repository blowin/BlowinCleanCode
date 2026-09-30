using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace BlowinCleanCode.Test.Verifiers;

public static partial class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic()"/>
    public static DiagnosticResult Diagnostic()
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic();

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(string)"/>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(diagnosticId);

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

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
        catch (Exception e)
        {
            throw new WithSourceMessageException(source, e);
        }
    }

    /// <summary>
    /// Verifies the analyzer against <paramref name="source"/> while <paramref name="editorConfig"/>
    /// is applied to it as a '.editorconfig' file, so that a test can cover the options a user
    /// redefines (see <see cref="BlowinCleanCode.Constant.Option"/>).
    /// </summary>
    public static async Task VerifyAnalyzerAsync(string source, string editorConfig, params DiagnosticResult[] expected)
        => await RunAsync(source, editorConfig, expected);

    private static async Task RunAsync(
        string source,
        string editorConfig,
        DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source,
        };

        test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
        test.ExpectedDiagnostics.AddRange(expected);

        try
        {
            await test.RunAsync(CancellationToken.None);
        }
        catch (Exception e)
        {
            throw new WithSourceMessageException(source, e);
        }
    }

    private sealed class WithSourceMessageException(string source, Exception ex) : Exception
    {
        public override string Message => source + Environment.NewLine + ex.Message;

        public override IDictionary Data => ex.Data;

        public override string Source => ex.Source;

        public override string HelpLink
        {
            get => ex.HelpLink;
            set => ex.HelpLink = value;
        }

        public override string StackTrace => ex.StackTrace;

        public override Exception GetBaseException() => ex.GetBaseException();
    }
}