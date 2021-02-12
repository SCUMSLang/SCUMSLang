using System.Diagnostics.CodeAnalysis;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public class NameReservableNodePool
    {
        private LinkedBucketList<string, Reference> nameReservableNodes;

        public NameReservableNodePool() {
            nameReservableNodes = new LinkedBucketList<string, Reference>();
        }

        public bool TryGetBucket(string name, [MaybeNullWhen(false)] out ILinkedBucketList<string, Reference> bucket) =>
            nameReservableNodes.TryGetBucket(name, out bucket);

        public void AddLast(string name, Reference node) =>
            nameReservableNodes.AddLast(name, node);
    }
}
