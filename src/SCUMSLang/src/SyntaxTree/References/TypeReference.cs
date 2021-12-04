using System;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class TypeReference : MemberReference
    {
        public static TypeReference CreateString(BlockContainer? blockContainer) =>
            new TypeReference(SystemTypeLibrary.Sequences[SystemType.String]) { ParentBlockContainer = blockContainer };

        public static TypeReference CreateInteger(BlockContainer? blockContainer) =>
            new TypeReference(SystemTypeLibrary.Sequences[SystemType.Integer]) { ParentBlockContainer = blockContainer };

        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeReference;

        public virtual bool IsArray { get; internal set; }

        public TypeReference(string? name)
            : base(name) { }

        protected TypeDefinition ResolveNonAliasDefinition(TypeDefinition typeDefinition)
        {
            if (typeDefinition.IsAlias) {
                do {
                    if (typeDefinition.BaseType is null) {
                        throw new InvalidOperationException("Type alias has no target type specified.");
                    }

                    typeDefinition = ParentBlock.Module.Resolve(typeDefinition.BaseType);
                } while (typeDefinition.IsAlias);
            }

            return typeDefinition;
        }

        public new TypeDefinition Resolve() =>
            ResolveNonAliasDefinition(ParentBlock?.Module.Resolve(this) ?? throw new NotSupportedException());

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitTypeReference(this);
    }
}
