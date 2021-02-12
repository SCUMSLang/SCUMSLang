using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class TypeAliasReference : TypeDefinition
    {
        public override TreeTokenType ReferenceType => SourceType.ReferenceType;
        public override SystemType SystemType => SourceType.SystemType;
        public override TypeDefinition SourceType { get; }

        public TypeAliasReference(string name, TypeDefinition sourceType)
            : base(name) =>
            SourceType = sourceType;

        public override bool IsSubsetOf(object? obj) {
            // We expect at least an object of this type:
            if (!(obj is TypeDefinition node)) {
                return false;
            }

            var equals = ReferenceType == node.ReferenceType
                && SourceType.Name == node.SourceType.Name
                && SystemType == node.SystemType;

            Trace.WriteLineIf(!equals, $"{nameof(TypeAliasReference)} is not subset of {nameof(TypeDefinition)}.");
            return equals;
        }
    }
}
