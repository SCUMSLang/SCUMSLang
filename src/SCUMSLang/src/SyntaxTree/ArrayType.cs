using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed class ArrayType : TypeSpecification
    {
        public override bool IsArray => true;

        // We need only one dimension.
        public int Rank => 1;

        public ArrayType(TypeReference elementType)
            : base(elementType) { }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitArrayType(this);
    }
}
