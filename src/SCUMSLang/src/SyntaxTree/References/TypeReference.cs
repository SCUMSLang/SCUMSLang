using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class TypeReference : MemberReference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeReference;

        public virtual bool IsArray { get; internal init; }
        public virtual bool IsAlias { get; internal init; }
        public virtual TypeReference? BaseType { get; internal init; }

        public TypeReference(string? name)
            : base(name) { }

        private T ResolveNonAlias<T>(T typeDefinition)
            where T : TypeReference
        {
            if (!typeDefinition.IsAlias) {
                goto exit;
            }

            do {
                if (typeDefinition.BaseType is null) {
                    throw SyntaxTreeThrowHelper.InvalidOperation(this, "Type alias has no target type specified.");
                }

                typeDefinition = ParentBlock.Module.Resolve<T>(typeDefinition.BaseType).Value;
            } while (typeDefinition.IsAlias);

            exit:
            return typeDefinition;
        }

        public T Resolve<T>()
            where T : TypeReference =>
            ResolveNonAlias(ParentBlock.Module.Resolve<T>(this).Value);

        public new TypeReference Resolve() =>
            Resolve<TypeReference>();

        protected override IMember ResolveMember() => Resolve();

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitTypeReference(this);
    }
}

namespace SCUMSLang.SyntaxTree.References
{
    partial class Reference
    {
        public static TypeReference CreateTypeReference(string name, BlockContainer? blockContainer) =>
            new TypeReference(name) { ParentBlockContainer = blockContainer };
    }
}
