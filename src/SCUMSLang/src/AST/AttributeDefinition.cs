namespace SCUMSLang.AST
{
    public class AttributeDefinition : Reference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.Attribute;
        public FunctionReference Function { get; internal set; } = null!;
        public FunctionCallReference FunctionCall { get; }

        public AttributeDefinition(FunctionCallReference functionCall) =>
            FunctionCall = functionCall;
    }
}
