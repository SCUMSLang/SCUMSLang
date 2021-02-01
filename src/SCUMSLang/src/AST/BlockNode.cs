using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        protected abstract Dictionary<string, Node> NamedNodes { get; }

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
            this.Parent = parent;
        }

        protected IEnumerable<BlockNode> YieldBlocks()
        {
            BlockNode parentBlock = this;

            do
            {
                yield return parentBlock;
                parentBlock = parentBlock.Parent;
            } while (!ReferenceEquals(StaticBlock, parentBlock));
        }

        public bool IsNameCrossBlockAssigned(string name)
        {
            foreach (var block in YieldBlocks())
            {
                if (block.NamedNodes.ContainsKey(name))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetDeclarationByName(string name, [NotNullWhen(true)] out DeclarationNode declaration)
        {
            foreach (var block in YieldBlocks())
            {
                if (block.NamedNodes.TryGetValue(name, out Node? node) && node is DeclarationNode declarationNode)
                {
                    declaration = declarationNode;
                    return true;
                }
            }

            declaration = null!;
            return false;
        }

        protected void AddNode(Node node) =>
            nodes.Add(node);

        public void AddDeclaration(DeclarationNode declaration)
        {
            if (declaration.Scope != Scope)
            {
                throw new ArgumentException("Declaration has invalid scope.");
            }

            foreach (var namedNode in NamedNodes)
            {
                if (IsNameCrossBlockAssigned(declaration.Name))
                {
                    throw new ArgumentException($"The name '{declaration.Name}' exists already.");
                }
            }

            nodes.Add(declaration);
            NamedNodes.Add(declaration.Name, declaration);
        }

        public void AddAssignment(AssignNode assignment)
        {
            if (TryGetDeclarationByName(assignment.Name, out var declaration))
            {
                var linkedAssignment = new LinkedAssignment(declaration, assignment);
                nodes.Add(linkedAssignment);
            }
            else
            {
                throw new ArgumentException($"Declaration with name '{assignment.Name}' does not exist.");
            }
        }

        public void EndBlock()
        {
            if (blockClosed) {
                throw new InvalidOperationException("You cannot end the block twice.");
            }

            blockClosed = true;

            if (ReferenceEquals(this, StaticBlock))
            {
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

            protected override Dictionary<string, Node> NamedNodes => Parent.NamedNodes;

            public LocalBlockNode(BlockNode parent, Node owner)
                : base(parent) =>
                Owner = owner;
        }
    }
}
