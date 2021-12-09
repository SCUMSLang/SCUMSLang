using System;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public abstract partial class Reference : IVisitableReference, IFilePositionable
    {
        public abstract SyntaxTreeNodeType NodeType { get; }
        public IFilePosition? FilePosition { get; init; }

        public override bool Equals(object? obj)
        {
            return obj is Reference node
                && NodeType == node.NodeType;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType);

        internal protected abstract Reference Accept(SyntaxNodeVisitor visitor);

        void IVisitableReference.Accept(SyntaxNodeVisitor visitor) =>
            throw new NotImplementedException();
    }
}
