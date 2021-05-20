using System;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class TypeReference : MemberReference
    {
        public readonly static TypeReference String = new TypeReference(SystemTypeLibrary.Sequences[SystemType.String]);
        public readonly static TypeReference Integer = new TypeReference(SystemTypeLibrary.Sequences[SystemType.Integer]);

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

                    typeDefinition = Module.Resolve(typeDefinition.BaseType);
                } while (typeDefinition.IsAlias);
            }

            return typeDefinition;
        }

        public new TypeDefinition Resolve() =>
            ResolveNonAliasDefinition(Module?.Resolve(this) ?? throw new NotSupportedException());

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitTypeReference(this);
    }
}
