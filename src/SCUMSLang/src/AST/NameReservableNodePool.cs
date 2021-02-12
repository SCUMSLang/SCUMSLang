using System.Diagnostics.CodeAnalysis;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public class NameReservableNodePool
    {
        private LinkedBucketList<string, Node> nameReservableNodes;

        public NameReservableNodePool() {
            nameReservableNodes = new LinkedBucketList<string, Node>();
        }

        public bool TryGetBucket(string name, [MaybeNullWhen(false)] out ILinkedBucketList<string, Node> bucket) =>
            nameReservableNodes.TryGetBucket(name, out bucket);

        public void AddLast(string name, Node node) =>
            nameReservableNodes.AddLast(name, node);
    }
}
