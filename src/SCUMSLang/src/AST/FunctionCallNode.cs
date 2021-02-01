using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class FunctionCallNode : Node
    {
        public override NodeType NodeType => NodeType.FunctionCall;
        public IReadOnlyList<ConstantNode> GenericArguments { get; }
        public IReadOnlyList<ConstantNode> Arguments { get; }

        public FunctionCallNode(IReadOnlyList<ConstantNode> genericArguments, IReadOnlyList<ConstantNode> arguments)
        {
            GenericArguments = genericArguments ?? new List<ConstantNode>();
            Arguments = arguments ?? new List<ConstantNode>();
        }
    }
}