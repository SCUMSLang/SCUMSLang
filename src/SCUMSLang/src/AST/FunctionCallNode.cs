using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public class FunctionCallNode : Node
    {
        public override NodeType NodeType => NodeType.FunctionCall;

        public string Name { get; }
        public IReadOnlyList<ConstantNode> GenericArguments { get; }
        public IReadOnlyList<ConstantNode> Arguments { get; }

        public FunctionCallNode(string name, IReadOnlyList<ConstantNode> genericArguments, IReadOnlyList<ConstantNode> arguments)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            GenericArguments = genericArguments ?? new List<ConstantNode>();
            Arguments = arguments ?? new List<ConstantNode>();
        }
    }
}
