using System;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public abstract partial class Reference : IVisitableReference, IFilePositionable, IHasParentBlock
    {
        public abstract SyntaxTreeNodeType NodeType { get; }
        public IFilePosition? FilePosition { get; init; }

        [AllowNull]
        public virtual BlockContainer ParentBlockContainer {
            get => parentBlockFeature.ParentBlockContainer;
            init => parentBlockFeature.ParentBlockContainer = value;
        }

        public BlockDefinition ParentBlock => parentBlockFeature.AsParentBlock(ParentBlockContainer.Block);

        [AllowNull]
        BlockDefinition IHasParentBlock.ParentBlock {
            get => ParentBlock;
            set => ParentBlockContainer.Block = value;
        }

        [MemberNotNullWhen(true, nameof(ParentBlock))]
        public virtual bool HasParentBlock =>
            parentBlockFeature.HasParentBlock;

        private ParentBlockFeature parentBlockFeature;

        public Reference() =>
            parentBlockFeature = new ParentBlockFeature(this);

        public override bool Equals(object? obj)
        {
            return obj is Reference node
                && NodeType == node.NodeType;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType);

        internal protected abstract Reference Accept(NodeVisitor visitor);

        void IVisitableReference.Accept(NodeVisitor visitor) =>
            throw new NotImplementedException();
    }
}
