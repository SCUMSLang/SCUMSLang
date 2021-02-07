using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class FunctionCallNode : Node
    {
        public override NodeType NodeType => NodeType.FunctionCall;

        public string FunctionName { get; }
        public IReadOnlyList<ConstantNode> GenericArguments { get; }
        public IReadOnlyList<ConstantNode> Arguments { get; }

        public FunctionCallNode(string functionName, IReadOnlyList<ConstantNode>? genericArguments, IReadOnlyList<ConstantNode>? arguments)
        {
            FunctionName = functionName ?? throw new System.ArgumentNullException(nameof(functionName));
            GenericArguments = genericArguments ?? new List<ConstantNode>();
            Arguments = arguments ?? new List<ConstantNode>();
        }
    }
}
