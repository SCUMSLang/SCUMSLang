using System;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class TypeReference : MemberReference
    {
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

namespace SCUMSLang.SyntaxTree.References
{
    partial class Reference
    {
        public static TypeReference CreateTypeReference(string name, BlockContainer? blockContainer) =>
            new TypeReference(name) { ParentBlockContainer = blockContainer };

        public static TypeReference CreateTypeReference(SystemType systemType, BlockContainer? blockContainer) =>
            CreateTypeReference(SystemTypeLibrary.Sequences[systemType], blockContainer);
    }
}
