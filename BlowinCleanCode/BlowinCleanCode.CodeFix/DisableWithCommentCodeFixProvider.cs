using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BlowinCleanCode.CodeFix
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DisableWithCommentCodeFixProvider)), Shared]
    public class DisableWithCommentCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray<string>.Empty;
        
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            return Task.CompletedTask;
        }
    }
}