using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class TypeAliasNode : TypeDefinitionNode
    {
        public override NodeType NodeType => SourceType.NodeType;
        public override DefinitionType DefinitionType => SourceType.DefinitionType;
        public override TypeDefinitionNode SourceType { get; }

        public TypeAliasNode(string name, TypeDefinitionNode sourceType)
            : base(name) =>
            SourceType = sourceType;

        public override bool IsSubsetOf(object? obj) {
            // We expect at least an object of this type:
            if (!(obj is TypeDefinitionNode node)) {
                return false;
            }

            var equals = NodeType == node.NodeType
                && SourceType.Name == node.SourceType.Name
                && DefinitionType == node.DefinitionType;

            Trace.WriteLineIf(!equals, $"{nameof(TypeAliasNode)} not equals.");
            return equals;
        }
    }
}
