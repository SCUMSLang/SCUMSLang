namespace SCUMSLang.SyntaxTree
{
    public class ArrayType : TypeSpecification
    {
        public override bool IsArray => true;

        // We need only one dimension.
        public int Rank => 1;

        public ArrayType(TypeReference elementType)
            : base(elementType) { }
    }
}
