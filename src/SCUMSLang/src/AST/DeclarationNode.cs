using System;

namespace SCUMSLang.AST
{
    public class DeclarationNode : Node, INameableNode
    {
        public override NodeType NodeType => NodeType.Declaration;

        public Scope Scope { get; }
        public NodeValueType ValueType { get; }
        public string Name { get; }

        public DeclarationNode(Scope scope, NodeValueType valueType, string name)
        {
            Scope = scope;
            ValueType = valueType;
            Name = name;
        }

        public override bool Equals(object? obj) =>
            obj is DeclarationNode declaration && Scope == declaration.Scope && Name == declaration.Name;

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Scope, Name);
    }
}
