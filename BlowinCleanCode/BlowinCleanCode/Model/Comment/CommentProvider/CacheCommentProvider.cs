using System.Collections.Concurrent;

namespace BlowinCleanCode.Model.Comment.CommentProvider
{
    public sealed class CacheCommentProvider : ICommentProvider
    {
        private readonly ICommentProvider _origin;
        private readonly ConcurrentDictionary<string, string> _cache;
            
        public CacheCommentProvider(ICommentProvider origin)
        {
            _origin = origin;
            _cache = new ConcurrentDictionary<string, string>();
        }
        
        public string SkipComment(string diagnosticId) => _cache.GetOrAdd(diagnosticId, s => _origin.SkipComment(s));
    }
}