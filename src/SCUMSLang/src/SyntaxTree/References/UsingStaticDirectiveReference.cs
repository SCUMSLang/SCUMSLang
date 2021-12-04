using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class UsingStaticDirectiveReference : MemberReference, IBlockScopedReference, IMemberDefinition
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.UsingStaticDirective;

        public BlockScope BlockScope =>
            BlockScope.Module;

        public override string Name =>
            $"using static {ElementType.Name}";

        public TypeReference ElementType { get; }

        public UsingStaticDirectiveReference(TypeReference elementType) =>
            ElementType = elementType;

        public new UsingStaticDirectiveReference Resolve() =>
            this;

        protected override IMemberDefinition ResolveMemberDefinition() =>
            Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitUsingStaticDirective(this);

        public UsingStaticDirectiveReference Update(TypeReference elementType)
        {
            if (ReferenceEquals(elementType, ElementType)) {
                return this;
            }

            return new UsingStaticDirectiveReference(elementType);
        }
    }
}
