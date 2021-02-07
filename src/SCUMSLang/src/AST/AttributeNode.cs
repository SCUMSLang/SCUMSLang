using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class AttributeNode : Node
    {
        public override NodeType NodeType => NodeType.Attribute;
        public FunctionNode Function { get; internal set; } = null!;
        public FunctionCallNode FunctionCall { get; }

        public AttributeNode(FunctionCallNode functionCall) =>
            FunctionCall = functionCall;
    }
}
