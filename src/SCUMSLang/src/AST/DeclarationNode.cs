using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class DeclarationNode : Node, INameReservableNode, IScopableNode
    {
        public override NodeType NodeType => NodeType.Declaration;

        public Scope Scope { get; }
        public TypeDefinitionNode Type { get; }
        public string Name { get; }

        public DeclarationNode(Scope scope, TypeDefinitionNode type, string name)
        {
            Scope = scope;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is DeclarationNode declaration)) {
                return false;
            }

            var equals = Scope == declaration.Scope
                && Type.Equals(declaration.Type)
                && Name == declaration.Name;

            Trace.WriteLineIf(!equals, $"{nameof(DeclarationNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Scope, Type, Name);
    }
}
