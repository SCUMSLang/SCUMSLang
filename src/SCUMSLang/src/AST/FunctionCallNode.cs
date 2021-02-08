using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class FunctionCallNode : Node
    {
        public override NodeType NodeType => NodeType.FunctionCall;

        public FunctionNode Function { get; }
        public IReadOnlyList<ConstantNode> GenericArguments { get; }
        public IReadOnlyList<ConstantNode> Arguments { get; }

        public FunctionCallNode(FunctionNode function, IReadOnlyList<ConstantNode>? genericArguments, IReadOnlyList<ConstantNode>? arguments)
        {
            Function = function;
            GenericArguments = genericArguments ?? new List<ConstantNode>();
            Arguments = arguments ?? new List<ConstantNode>();
        }
    }
}
