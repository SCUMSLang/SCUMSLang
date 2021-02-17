namespace SCUMSLang.SyntaxTree
{
    public class AttributeDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.Attribute;
        public MethodDefinition Function { get; internal set; } = null!;
        public MethodCallDefinition FunctionCall { get; }

        public AttributeDefinition(MethodCallDefinition functionCall) =>
            FunctionCall = functionCall;
    }
}
