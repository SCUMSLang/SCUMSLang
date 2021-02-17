using System.Diagnostics.CodeAnalysis;
using Teronis.Collections.Specialized;

namespace SCUMSLang.SyntaxTree
{
    public class ReferencePool
    {
        private LinkedBucketList<string, Reference> referencesByName;

        public ReferencePool() =>
            referencesByName = new LinkedBucketList<string, Reference>();

        public bool TryGetBucket(string name, [MaybeNullWhen(false)] out ILinkedBucketList<string, Reference> bucket) =>
            referencesByName.TryGetBucket(name, out bucket);

        public void AddLast(string name, Reference node) =>
            referencesByName.AddLast(name, node);
    }
}
