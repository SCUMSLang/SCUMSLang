using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCUMSLang.AST
{
    public abstract class CollectionNode : Node, IReadOnlyList<Node>
    {
        private Collection<Node> nodes;

        public CollectionNode() =>
            nodes = new Collection<Node>();

        public Node this[int index] => 
            ((IReadOnlyList<Node>)nodes)[index];

        public int Count => 
            ((IReadOnlyCollection<Node>)nodes).Count;

        public IEnumerator<Node> GetEnumerator() =>
            ((IEnumerable<Node>)nodes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)nodes).GetEnumerator();
    }
}
