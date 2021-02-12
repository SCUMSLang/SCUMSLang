using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Teronis.Collections.Specialized;

namespace SCUMSLang.AST
{
    public abstract class BlockNode : Node
    {
        public override NodeType NodeType => NodeType.Block;
        public abstract Scope Scope { get; }
        public abstract StaticBlockNode StaticBlock { get; }

        public BlockNode CurrentBlock { get; protected set; }
        public IReadOnlyList<Node> Nodes => nodes;

        internal protected abstract NameReservableNodePool NameReservableNodes { get; }
        protected BlockNode Parent;

        private List<Node> nodes;
        private bool blockClosed;

        public BlockNode()
        {
            nodes = new List<Node>();
            CurrentBlock = this;
            Parent = this;
        }

        private BlockNode(BlockNode parent)
        {
            nodes = new List<Node>();
            CurrentBlock = this;
            Parent = parent;
        }

        public void BeginBlock(BlockNode functionBlock) =>
            CurrentBlock = functionBlock;

        protected IEnumerable<BlockNode> YieldBlocks()
        {
            BlockNode parentBlock = this;

            do {
                yield return parentBlock;
                parentBlock = parentBlock.Parent;
            } while (!ReferenceEquals(StaticBlock, parentBlock));
        }

        public bool TryGetNodesByName(string name, [MaybeNullWhen(false)] out List<Node> foundNodes)
        {
            foundNodes = new List<Node>();

            foreach (var block in YieldBlocks()) {
                if (block.NameReservableNodes.TryGetBucket(name, out ILinkedBucketList<string, Node>? nodes)) {
                    foreach (var node in nodes) {
                        foundNodes.Add(node);
                    }
                }
            }

            return foundNodes.Count != 0;
        }

        /// <summary>
        /// Gets nodes by name casted to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Not all nodes by name <paramref name="name"/> are of type <typeparamref name="T"/>.</exception>
        public List<T>? GetCastedNodesByName<T>(string name)
            where T : Node
        {
            if (TryGetNodesByName(name, out var nodes)) {
                try {
                    var candidates = nodes.Cast<T>().ToList();
                    return candidates;
                } catch (InvalidCastException) {
                    throw new ArgumentException($"A programming structure of another type with the name {name} exists already.");
                }
            }

            return null;
        }

        public bool TryGetFirstNode<T>(IEnumerable<Node> candidates, Func<Node, bool> isFunctionDelegate, [MaybeNullWhen(false)] out T function)
            where T : Node
        {
            foreach (var candiate in candidates) {
                if (isFunctionDelegate(candiate)) {
                    function = (T)candiate;
                    return true;
                }
            }

            function = null;
            return false;
        }

        public bool TryGetFirstNode<T>(string name, Func<Node, bool> isNodeDelegate, [MaybeNullWhen(false)] out T function)
            where T : Node
        {
            if (TryGetNodesByName(name, out var candidates)
                && TryGetFirstNode(candidates, isNodeDelegate, out function)) {
                return true;
            }

            function = null;
            return false;
        }

        public bool TryGetFirstNode<T>(IEnumerable<T> candidates, [MaybeNullWhen(false)] out T function)
            where T : Node =>
            TryGetFirstNode(candidates, (node) => node is T, out function);

        public bool TryGetFirstNode<T>(string name, [MaybeNullWhen(false)] out T function)
            where T : Node =>
            TryGetFirstNode(name, (node) => node is T, out function);

        public bool TryGetFirstNode<T>(T template, [MaybeNullWhen(false)] out T function)
            where T : Node, INameReservableNode =>
            TryGetFirstNode(template.Name, (node) => node.Equals(template), out function);

        public bool TryGetFirstNode<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T function, IEqualityComparer<T>? comparer = null)
            where T : Node, INameReservableNode
        {
            comparer ??= EqualityComparer<T>.Default;
            return TryGetFirstNode(candidates, (node) => node is T typedNode && comparer.Equals(typedNode, template), out function);
        }

        public bool TryGetFirstFunctionNode(
            string name,
            IReadOnlyList<ConstantNode>? genericArguments,
            IReadOnlyList<ConstantNode>? arguments,
            [MaybeNullWhen(false)] out FunctionNode function,
            bool required)
        {
            genericArguments ??= new List<ConstantNode>();
            arguments ??= new List<ConstantNode>();

            bool isFunction(Node node)
            {
                if (!(node is FunctionNode function)) {
                    return false;
                }

                if (function.GenericParameters.Count != genericArguments.Count) {
                    return false;
                }

                var genericParameterIndex = function.GenericParameters.Count;

                while (--genericParameterIndex >= 0) {
                    if (!function.GenericParameters[genericParameterIndex].Type.IsSubsetOf(genericArguments[genericParameterIndex].Type)) {
                        return false;
                    }
                }

                var parameterIndex = function.Parameters.Count;

                if (function.Parameters.Count != arguments.Count) {
                    return false;
                }

                while (--parameterIndex >= 0) {
                    if (!function.Parameters[parameterIndex].Type.IsSubsetOf(arguments[parameterIndex].Type)) {
                        return false;
                    }
                }

                return true;
            }

            if (TryGetFirstNode(name, isFunction, out function)) {
                return true;
            } else if (required) {
                throw new ArgumentException($"Function with name '{name}' and proper overload does not exist");
            }

            return false;
        }

        public TypeDefinitionNode GetTypeDefinition(DefinitionType definitionType)
        {
            if (!NameReservableNodes.TryGetBucket(DefinitionTypeLibrary.Sequences[definitionType], out var bucket)) {
                throw new NotSupportedException("This in-built type does not have a one-to one mapping.");
            }

            return (TypeDefinitionNode)bucket.First!.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathFragments"></param>
        /// <returns>Either <see cref="EnumerationMemberNode"/> or <see cref="EnumerationDefinitionNode"/>.</returns>
        public TypeDefinitionNode GetTypeDefinition(IReadOnlyList<string> pathFragments, TypeDefinitionViewpoint viewpoint)
        {
            if (pathFragments.Count == 0) {
                throw new ArgumentException("Insufficient path fragments.");
            }

            if (viewpoint == TypeDefinitionViewpoint.Type && pathFragments.Count > 1) {
                throw new ArgumentException($"You are specifiying a type. You cannot use a member access.");
            }

            if (viewpoint == TypeDefinitionViewpoint.Value && pathFragments.Count > 2) {
                throw new ArgumentException($"A member access across two type definitions are not supported.");
            }

            var nodeName = pathFragments[0];
            var candidates = GetCastedNodesByName<TypeDefinitionNode>(nodeName);

            if (candidates is null) {
                throw new ArgumentException($"The type definition {nodeName} does not exist.");
            } else if (candidates.Count > 1) {
                throw new ArgumentException($"There are two or more type definition named by {nodeName}.");
            }

            var candidate = candidates[0].SourceType;

            if (candidate is EnumerationDefinitionNode enumeration) {
                if (pathFragments.Count == 1) {
                    return enumeration;
                }
                // Enum member is only allowed as value.
                else if (viewpoint == TypeDefinitionViewpoint.Value) {
                    return enumeration.GetMemberByName(pathFragments[1]);
                }
            } else if (pathFragments.Count == 1) {
                return candidate;
            }

            throw new ArgumentException($"The type definition '{nodeName}' does not support member access.");
        }

        public void AddNode(Node node)
        {
            bool handleNameReservation(Node node)
            {
                if (node is INameReservableNode nameReservableNode) {
                    bool hasDuplication = NameReservableNodes.TryGetBucket(nameReservableNode.Name, out _);

                    // If node has name, then it can handle name duplications.
                    if (hasDuplication && node is INameDuplicationHandleableNode nameDuplicationHandleableNode) {
                        var result = nameDuplicationHandleableNode.CanReserveName(this);

                        if (result == ConditionalNameReservationResult.True) {
                            hasDuplication = false;
                        } else if (result == ConditionalNameReservationResult.Skip) {
                            return false;
                        }
                    }

                    if (hasDuplication) {
                        throw new ArgumentException($"The name '{nameReservableNode.Name}' is already reserved.");
                    }

                    NameReservableNodes.AddLast(nameReservableNode.Name, node);
                }

                if (node is INamesReservableNode namesReservableNode && namesReservableNode.HasReservedNames) {
                    foreach (var namedNode in namesReservableNode.GetReservedNames()) {
                        handleNameReservation(namedNode);
                    }
                }

                return true;
            }

            if (node is IScopableNode scopableNode && scopableNode.Scope != Scope) {
                throw new ArgumentException("Declaration has invalid scope.");
            }

            if (handleNameReservation(node)) {
                nodes.Add(node);
            }
        }

        public void AddAssignment(AssignNode assignment) =>
            nodes.Add(assignment);

        public void EndBlock()
        {
            if (blockClosed) {
                throw new InvalidOperationException("You cannot end the block twice.");
            }

            blockClosed = true;

            if (ReferenceEquals(this, StaticBlock)) {
                throw new InvalidOperationException("You cannot end the block of the root block.");
            }

            CurrentBlock = CurrentBlock.Parent;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is BlockNode block)) {
                return false;
            }

            var equals = blockClosed == block.blockClosed
                && nodes.SequenceEqual(block.nodes);

            Trace.WriteLineIf(!equals, $"{nameof(BlockNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Nodes);

        public abstract class LocalBlockNode : BlockNode
        {
            public override Scope Scope => Scope.Local;
            public override StaticBlockNode StaticBlock => Parent.StaticBlock;
            public Node Owner { get; }

            internal protected override NameReservableNodePool NameReservableNodes => Parent.NameReservableNodes;

            public LocalBlockNode(BlockNode parent, Node owner)
                : base(parent) =>
                Owner = owner;
        }
    }
}
