using System.Collections.Generic;

namespace BlowinCleanCode.CommentProvider
{
    public sealed class CacheCommentProvider : ICommentProvider
    {
        private readonly ICommentProvider _origin;
        private readonly Dictionary<string, string> _cache;
            
        public CacheCommentProvider(ICommentProvider origin)
        {
            _origin = origin;
            _cache = new Dictionary<string, string>();
        }
        
        public string SkipComment(string diagnosticId)
        {
            if (_cache.TryGetValue(diagnosticId, out var comment))
                return comment;

            comment = _origin.SkipComment(diagnosticId);
            _cache[diagnosticId] = comment;
            return comment;
        }
    }
}