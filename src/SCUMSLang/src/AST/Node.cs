using System;

namespace SCUMSLang.AST
{
    public abstract class Node
    {
        public abstract NodeType NodeType { get; }

        public override bool Equals(object? obj) => 
            obj is Node node && NodeType == node.NodeType;

        public override int GetHashCode() =>
            HashCode.Combine(NodeType);
    }
}
