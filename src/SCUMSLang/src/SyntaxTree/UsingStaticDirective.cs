using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class UsingStaticDirective : MemberReference, IBlockScopedReference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.UsingStaticDirective;

        public BlockScope BlockScope =>
            BlockScope.Module;

        public override string Name =>
            $"using static {ElementType.LongName}";

        public override string LongName =>
            throw new System.NotImplementedException();

        public TypeReference ElementType { get; }

        public UsingStaticDirective(TypeReference elementType) =>
            ElementType = elementType;

        protected override IMemberDefinition ResolveDefinition() => 
            throw new System.NotImplementedException();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitUsingStaticDirective(this);
    }
}
