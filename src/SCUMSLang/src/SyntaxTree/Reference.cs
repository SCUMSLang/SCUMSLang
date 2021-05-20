using System;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public abstract partial class Reference : IVisitableReference
    {
        public abstract SyntaxTreeNodeType NodeType { get; }

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
