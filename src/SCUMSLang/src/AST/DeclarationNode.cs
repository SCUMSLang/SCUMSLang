using System;

namespace SCUMSLang.AST
{
    public class DeclarationNode : Node
    {
        public override NodeType NodeType => NodeType.Declaration;

        public Scope Scope { get; }
        public string Name { get; }

        public DeclarationNode(Scope scope, NodeValueType valueType, string name)
        {
            Scope = scope;
            Name = name;
        }

        public override bool Equals(object? obj) =>
            obj is DeclarationNode declaration && Scope == declaration.Scope && Name == declaration.Name;

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Scope, Name);
    }
}
