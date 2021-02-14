namespace SCUMSLang.AST
{
    public class AttributeDefinition : Reference
    {
        public override TreeTokenType TokenType => TreeTokenType.Attribute;
        public MethodDefinition Function { get; internal set; } = null!;
        public MethodCallDefinition FunctionCall { get; }

        public AttributeDefinition(MethodCallDefinition functionCall) =>
            FunctionCall = functionCall;
    }
}
