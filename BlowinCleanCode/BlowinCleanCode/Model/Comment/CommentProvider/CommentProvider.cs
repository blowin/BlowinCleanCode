﻿namespace BlowinCleanCode.Model.Comment.CommentProvider
{
    public sealed class CommentProvider : ICommentProvider
    {
        public static readonly ICommentProvider Instance = new CacheCommentProvider(
            new CommentProvider()
        );
        
        public string SkipComment(string diagnosticId) => "// Disable " + diagnosticId;
    }
}