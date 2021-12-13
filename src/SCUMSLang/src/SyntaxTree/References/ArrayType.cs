using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public sealed class ArrayType : TypeSpecification
    {
        public override bool IsArray => true;
        public override string Name => base.Name + "[]";
        // We need only one dimension.
        public int Rank => 1;

        public ArrayType(TypeReference elementType)
            : base(elementType) { }

        public new ArrayType Resolve() => this;

        protected override IMember ResolveMember() => Resolve();

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitArrayType(this);
    }
}
