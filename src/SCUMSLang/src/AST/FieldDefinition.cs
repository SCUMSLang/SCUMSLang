namespace SCUMSLang.AST
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

        public new virtual FieldDefinition Resolve() {
            ResolveDependencies();
            return this;
        }
    }
}
