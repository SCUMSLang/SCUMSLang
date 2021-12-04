using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class UsingStaticDirectiveDefinition : MemberReference, IBlockScopedReference, IMemberDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.UsingStaticDirective;

        public BlockScope BlockScope =>
            BlockScope.Module;

        public override string Name =>
            $"using static {ElementType.Name}";

        public TypeReference ElementType { get; }

        public UsingStaticDirectiveDefinition(TypeReference elementType) =>
            ElementType = elementType;

        public new UsingStaticDirectiveDefinition Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitUsingStaticDirective(this);

        public UsingStaticDirectiveDefinition Update(TypeReference elementType)
        {
            if (ReferenceEquals(elementType, ElementType)) {
                return this;
            }

            return new UsingStaticDirectiveDefinition(elementType);
        }
    }
}
