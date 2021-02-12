using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.Collections.Generic
{
    public static class TopologicalSorting
    {
        public static class KahnAlgorithm
        {
            public static List<T> SortTopologically<T>(IEnumerable<T> nodes, IEnumerable<ValueTuple<T, T>> edges, IEqualityComparer<T> comparer)
            {
                comparer = comparer ?? EqualityComparer<T>.Default;
                var valueTupleComparer = new ValueTypeComparer<T>(comparer);
                var nodeSet = new HashSet<T>(nodes, comparer);
                var edgeSet = new HashSet<ValueTuple<T, T>>(edges, valueTupleComparer);
                var L = new List<T>();

                // Start nodes which have no incoming edges.
                var S = new List<T>(
                    nodeSet.Where(node =>
                        edgeSet.All(edge =>
                            comparer.Equals(node, edge.Item2) == false)));

                while (S.Count > 0) {
                    var n = S[0];
                    S.RemoveAt(0);
                    L.Insert(0, n);

                    // Each incoming node with an edge from start node to incoming node.
                    var edges2 = edgeSet.Where(edge => comparer.Equals(n, edge.Item1)).ToList();

                    foreach (var e in edges2) {
                        var m = e.Item2;

                        // Remove edge from the graph.
                        edgeSet.Remove(e);

                        // If incoming node has no other incoming edges then:
                        if (edgeSet.All(scopedEdge => comparer.Equals(m, scopedEdge.Item2) == false)) {
                            // Insert incoming node into list of start nodes.
                            S.Add(m);
                        }
                    }
                }

                return L;
            }

            public class ValueTypeComparer<T> : EqualityComparer<ValueTuple<T, T>>
            {
                public IEqualityComparer<T> TupleItemComparer { get; }

                public ValueTypeComparer(IEqualityComparer<T> tupleItemComparer) =>
                    TupleItemComparer = tupleItemComparer;

                public override bool Equals([AllowNull] (T, T) x, [AllowNull] (T, T) y) =>
                    TupleItemComparer.Equals(x.Item1, y.Item1) && TupleItemComparer.Equals(x.Item2, y.Item2);

                public override int GetHashCode([DisallowNull] (T, T) obj) =>
                    HashCode.Combine(obj.Item1, obj.Item2);
            }
        }
    }
}
