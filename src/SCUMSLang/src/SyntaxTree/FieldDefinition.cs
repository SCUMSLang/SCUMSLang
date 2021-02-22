using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class FieldDefinition : FieldReference
    {
        public object? Value { get; set; }

        public FieldDefinition(string name, TypeReference fieldType)
            : base(name, fieldType) { }

        public FieldDefinition(string name, TypeReference fieldType, TypeReference declaringType)
            : base(name, fieldType, declaringType) { }

        protected override IMemberDefinition ResolveDefinition() =>
            Resolve();

        public new virtual FieldDefinition Resolve()
        {
            ResolveDependencies();
            return this;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj) && obj is FieldDefinition field
                && Equals(field.Value, Value);
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitFieldDefinition(this);
    }
}
