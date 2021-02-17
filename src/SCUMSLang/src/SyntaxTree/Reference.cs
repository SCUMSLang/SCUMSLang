using System;

namespace SCUMSLang.SyntaxTree
{
    public abstract class Reference
    {
        public abstract SyntaxTreeNodeType NodeType { get; }

        public override bool Equals(object? obj)
        {
            return obj is Reference node 
                && NodeType == node.NodeType;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType);
    }
}
