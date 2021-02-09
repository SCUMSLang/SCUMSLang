using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class TypeDefinitionNode : Node, INameReservableNode
    {
        public override NodeType NodeType => NodeType.TypeDefinition;

        public string Name { get; }
        public virtual DefinitionType DefinitionType { get; }

        internal bool AllowOverwriteOnce { get; set; }

        public virtual TypeDefinitionNode SourceType => this;

        protected TypeDefinitionNode(string name) =>
            Name = name ?? throw new ArgumentNullException(nameof(name));

        public TypeDefinitionNode(string name, DefinitionType type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DefinitionType = type;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is TypeDefinitionNode node)) {
                return false;
            }

            var equals = NodeType == node.NodeType
                && Name == node.Name
                && DefinitionType == node.DefinitionType;

            Trace.WriteLineIf(!equals, $"{nameof(TypeDefinitionNode)} not equals.");
            return equals;
        }

        public virtual bool IsSubsetOf(object? obj) =>
            Equals(obj);

        public override int GetHashCode() =>
            HashCode.Combine(base.GetHashCode(), NodeType, Name, DefinitionType);
    }
}
