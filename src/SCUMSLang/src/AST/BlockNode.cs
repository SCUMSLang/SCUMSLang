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
        public abstract BlockNode StaticBlock { get; }

        public BlockNode CurrentBlock { get; protected set; }
        public IReadOnlyList<Node> Nodes => nodes;

        protected abstract LinkedBucketList<string, Node> ReservedNames { get; }
        protected abstract Dictionary<InBuiltType, TypeDefinitionNode> InBuiltTypeDefinitions { get; }
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

        public bool IsNameCrossBlockReserved(string name)
        {
            foreach (var block in YieldBlocks()) {
                if (block.ReservedNames.TryGetBucket(name, out _)) {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNodesByName(string name, [MaybeNullWhen(false)] out List<Node> foundNodes)
        {
            foundNodes = new List<Node>();

            foreach (var block in YieldBlocks()) {
                if (block.ReservedNames.TryGetBucket(name, out ILinkedBucketList<string, Node>? nodes)) {
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
            where T : Node, INameReservedNode =>
            TryGetFirstNode(template.Name, (node) => node.Equals(template), out function);

        public bool TryGetFirstNode<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T function, IEqualityComparer<T>? comparer = null)
            where T : Node, INameReservedNode
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

        public bool TryGetInBuiltTypeDefinition(InBuiltType inBuiltType, [MaybeNullWhen(false)] out TypeDefinitionNode typeDefinition)
        {
            if (!InBuiltTypeDefinitions.TryGetValue(inBuiltType, out typeDefinition)) {
                return false;
            }

            return true;
        }

        public TypeDefinitionNode GetInBuiltTypeDefinition(InBuiltType inBuiltType)
        {
            if (!TryGetInBuiltTypeDefinition(inBuiltType, out var typeDefinition)) {
                throw new ArgumentException($"A type definition for in-built type {InBuiltTypeLibrary.SequenceExamples[inBuiltType]} is missing.");
            }

            return typeDefinition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathFragments"></param>
        /// <returns>Either <see cref="EnumerationMemberNode"/> or <see cref="EnumerationDefinitionNode"/>.</returns>
        public TypeDefinitionNode GetEnumerationTypeDefinitionFromValue(IReadOnlyList<string> pathFragments)
        {
            if (pathFragments.Count == 0) {
                throw new ArgumentException("Insufficient path fragments.");
            }

            if (pathFragments.Count > 2) {
                throw new ArgumentException($"A member access across two programming structures are not supported.");
            }

            var nodeName = pathFragments[0];
            var candidates = GetCastedNodesByName<TypeDefinitionNode>(nodeName);

            if (candidates is null) {
                throw new ArgumentException($"The programming structure {nodeName} does not exist.");
            } else if (candidates.Count > 1) {
                throw new ArgumentException($"There are two or more programming structures named by {nodeName}.");
            }

            if (candidates[0] is EnumerationDefinitionNode enumeration) {
                if (pathFragments.Count == 1) {
                    return enumeration;
                } else {
                    return enumeration.GetMemberByName(pathFragments[1]);
                }
            } else if (pathFragments.Count == 1 && candidates[0] is EnumerationMemberNode enumerationMember) {
                return enumerationMember;
            }

            throw new ArgumentException($"The programming structure '{nodeName}' does not support member access.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathFragments"></param>
        /// <returns>Either <see cref="EnumerationMemberNode"/> or <see cref="EnumerationDefinitionNode"/>.</returns>
        public TypeDefinitionNode GetEnumerationTypeDefinitionFromType(IReadOnlyList<string> pathFragments)
        {
            if (pathFragments.Count == 0) {
                throw new ArgumentException("Insufficient path fragments.");
            }

            if (pathFragments.Count > 1) {
                throw new ArgumentException($"A member access as type is not allowed.");
            }

            var nodeName = pathFragments[0];
            var candidates = GetCastedNodesByName<TypeDefinitionNode>(nodeName);

            if (candidates is null) {
                throw new ArgumentException($"The programming structure {nodeName} does not exist.");
            } else if (candidates.Count > 1) {
                throw new ArgumentException($"There are two or more programming structures named by {nodeName}.");
            }

            if (pathFragments.Count == 1 && candidates[0] is EnumerationDefinitionNode enumeration) {
                return enumeration;
            }

            throw new ArgumentException($"The programming structure '{nodeName}' does not support member access.");
        }

        public void AddNode(Node node) =>
            nodes.Add(node);

        protected void AddNameReservedNodeBlindly(string name, Node nameableNode)
        {
            nodes.Add(nameableNode);
            ReservedNames.AddLast(name, nameableNode);
        }

        protected void AddNameReservedNodeBlindly<T>(T nameableNode)
            where T : Node, INameReservedNode
        {
            nodes.Add(nameableNode);
            ReservedNames.AddLast(nameableNode.Name, nameableNode);
        }

        protected void AddNonCrossBlockNameReservedNode(string name, Node nameableNode)
        {
            if (IsNameCrossBlockReserved(name)) {
                throw new ArgumentException($"Name '{name} is already reserved.'");
            }

            AddNameReservedNodeBlindly(name, nameableNode);
        }

        protected void AddNonCrossBlockNameReservedNode<T>(T nameableNode)
            where T : Node, INameReservedNode =>
            AddNonCrossBlockNameReservedNode(nameableNode.Name, nameableNode);

        /// <summary>
        /// Adds only reserved names of nodes that implements <see cref="IHasReservedNames"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reservedNamesHavingNode"></param>
        protected void AddNonCrossBlockReservedNames<T>(T reservedNamesHavingNode)
            where T : Node, IHasReservedNames
        {
            if (!reservedNamesHavingNode.HasReservedNames) {
                return;
            }

            foreach (var (Name, Node) in reservedNamesHavingNode.GetReservedNames()) {
                ReservedNames.AddLast(Name, Node);
            }
        }

        public void AddDeclaration(DeclarationNode declaration)
        {
            if (declaration.Scope != Scope) {
                throw new ArgumentException("Declaration has invalid scope.");
            }

            AddNonCrossBlockNameReservedNode(declaration);
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

            Debug.WriteLineIf(!equals, $"{nameof(BlockNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Nodes);

        public abstract class LocalBlockNode : BlockNode
        {
            public override Scope Scope => Scope.Local;
            public override BlockNode StaticBlock => Parent.StaticBlock;
            public Node Owner { get; }

            protected override Dictionary<InBuiltType, TypeDefinitionNode> InBuiltTypeDefinitions => Parent.InBuiltTypeDefinitions;
            protected override LinkedBucketList<string, Node> ReservedNames => Parent.ReservedNames;

            public LocalBlockNode(BlockNode parent, Node owner)
                : base(parent) =>
                Owner = owner;
        }
    }
}
