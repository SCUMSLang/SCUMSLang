using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class UsingStaticDirective : MemberReference, IBlockScopedReference, IMemberDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.UsingStaticDirective;

        public BlockScope BlockScope =>
            BlockScope.Module;

        public override string Name =>
            $"using static {ElementType.Name}";

        public TypeReference ElementType { get; }

        public UsingStaticDirective(TypeReference elementType) =>
            ElementType = elementType;

        public new UsingStaticDirective Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitUsingStaticDirective(this);

        public UsingStaticDirective Update(TypeReference elementType)
        {
            if (ReferenceEquals(elementType, ElementType)) {
                return this;
            }

            return new UsingStaticDirective(elementType);
        }
    }
}
