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
                var topologicalSortedNodes = new List<T>();

                // Start nodes which have no incoming edges.
                var startNodes = new List<T>(
                    nodeSet.Where(node =>
                        edgeSet.All(edge =>
                            !comparer.Equals(node, edge.Item2))));

                for (var index = startNodes.Count - 1; index >= 0; index++) {
                    var startNode = startNodes[index];
                    startNodes.Remove(startNode);
                    topologicalSortedNodes.Add(startNode);

                    // For each incoming node with an edge from start node to incoming node do:
                    foreach (var edge in edgeSet.Where(edge => comparer.Equals(startNode, edge.Item1)).ToList()) {
                        var incomingNode = edge.Item2;

                        // Remove edge from the graph.
                        edgeSet.Remove(edge);

                        // If incoming node has no other incoming edges then:
                        if (edges.All(scopedEdge => !comparer.Equals(incomingNode, scopedEdge.Item2))) {
                            // Insert incoming node into list of start nodes.
                            startNodes.Add(incomingNode);
                            index++;
                        }
                    }
                }

                return topologicalSortedNodes;
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
