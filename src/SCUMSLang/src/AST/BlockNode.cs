using System;
using System.Collections.Generic;
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

        protected BlockNode Parent;
        protected abstract LinkedBucketList<string, Node> NamedNodes { get; }

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

        public bool IsNameCrossBlockAssigned(string name)
        {
            foreach (var block in YieldBlocks()) {
                if (block.NamedNodes.TryGetBucket(name, out _)) {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetNodes(string name, [MaybeNullWhen(false)] out List<Node> foundNodes)
        {
            foundNodes = new List<Node>();

            foreach (var block in YieldBlocks()) {
                if (block.NamedNodes.TryGetBucket(name, out ILinkedBucketList<string, Node>? nodes)) {
                    foreach (var node in nodes) {
                        foundNodes.Add(node);
                    }
                }
            }

            return foundNodes.Count != 0;
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

        //public bool TryGetFirstNode<T>(string name, Func<Node, bool> isNodeDelegate, [MaybeNullWhen(false)] out T function)
        //    where T : Node
        //{
        //    if (TryGetNodes(name, out var candidates)
        //        && TryGetFirstNode(candidates, isNodeDelegate, out function)) {
        //        return true;
        //    }

        //    function = null;
        //    return false;
        //}

        public bool TryGetFirstNode<T>(string name, Func<Node, bool> isNodeDelegate, [MaybeNullWhen(false)] out T function)
            where T : Node
        {
            if (TryGetNodes(name, out var candidates)
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
            where T : Node, INameableNode =>
            TryGetFirstNode(template.Name, (node) => node.Equals(template), out function);

        public bool TryGetFirstNode<T>(IEnumerable<T> candidates, T template, [MaybeNullWhen(false)] out T function, IEqualityComparer<T>? comparer = null)
            where T : Node, INameableNode
        {
            comparer ??= EqualityComparer<T>.Default;
            return TryGetFirstNode(candidates, (node) => node is T typedNode && comparer.Equals(typedNode, template), out function);
        }

        public bool TryGetFirstFunctionNode(string name, FunctionCallNode functionCall, [MaybeNullWhen(false)] out FunctionNode function)
        {
            bool isFunction(Node node)
            {
                if (!(node is FunctionNode function)) {
                    return false;
                }

                if (function.GenericParameters.Count != functionCall.GenericArguments.Count) {
                    return false;
                }

                var genericParameterIndex = function.GenericParameters.Count;

                while (--genericParameterIndex >= 0) {
                    if (function.GenericParameters[genericParameterIndex].ValueType != functionCall.GenericArguments[genericParameterIndex].ValueType) {
                        return false;
                    }
                }

                var parameterIndex = function.Parameters.Count;

                if (function.Parameters.Count != functionCall.Arguments.Count) {
                    return false;
                }

                while (--parameterIndex >= 0) {
                    if (function.Parameters[parameterIndex].ValueType != functionCall.Arguments[parameterIndex].ValueType) {
                        return false;
                    }
                }

                return true;
            }

            if (TryGetFirstNode(name, isFunction, out function)) {
                return true;
            }

            return false;
        }

        public void AddNode(Node node) =>
            nodes.Add(node);

        public void AddDeclaration(DeclarationNode declaration)
        {
            if (declaration.Scope != Scope) {
                throw new ArgumentException("Declaration has invalid scope.");
            }

            if (IsNameCrossBlockAssigned(declaration.Name)) {
                throw new ArgumentException($"The name '{declaration.Name}' exists already.");
            }

            nodes.Add(declaration);
            NamedNodes.AddLast(declaration.Name, declaration);
        }

        public void AddAssignment(AssignNode assignment)
        {
            if (TryGetFirstNode<DeclarationNode>(assignment.DeclarationName, (node) => node is DeclarationNode, out var declarations)) {
                assignment.Declaration = declarations;
                nodes.Add(assignment);
            } else {
                throw new ArgumentException($"Declaration with name '{assignment.DeclarationName}' does not exist.");
            }
        }

        internal void AddAttribute(AttributeNode attribute)
        {
            if (TryGetFirstFunctionNode(attribute.FunctionCall.FunctionName, attribute.FunctionCall, out var function)) {
                attribute.Function = function;
                nodes.Add(attribute);
            } else {
                throw new ArgumentException($"Function with name '{attribute.FunctionCall.FunctionName}' and proper overload does not exist");
            }
        }

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

        public override bool Equals(object? obj) =>
            obj is BlockNode block
            && blockClosed == block.blockClosed
            && nodes.SequenceEqual(block.nodes);

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Nodes);

        public abstract class LocalBlockNode : BlockNode
        {
            public override Scope Scope => Scope.Local;
            public override BlockNode StaticBlock => Parent.StaticBlock;
            public Node Owner { get; }

            protected override LinkedBucketList<string, Node> NamedNodes => Parent.NamedNodes;

            public LocalBlockNode(BlockNode parent, Node owner)
                : base(parent) =>
                Owner = owner;
        }
    }
}
