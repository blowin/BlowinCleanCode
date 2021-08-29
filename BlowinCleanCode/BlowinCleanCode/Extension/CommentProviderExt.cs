using BlowinCleanCode.Model.Comment.CommentProvider;
using Microsoft.CodeAnalysis;

namespace BlowinCleanCode.Extension
{
    public static class CommentProviderExt
    {
        public static string SkipComment(this ICommentProvider self, Diagnostic diagnostic) =>
            self.SkipComment(diagnostic.Id);
        
        public static string SkipComment(this ICommentProvider self, DiagnosticDescriptor descriptor) =>
            self.SkipComment(descriptor.Id);
    }
}