using System;

namespace SCUMSLang.AST
{
    public class AssignNode : Node
    {
        public override NodeType NodeType => NodeType.Assignment;

        public string Name { get; }
        public ConstantNode Constant { get; }

        public AssignNode(string name, ConstantNode constant)
        {
            Name = name;
            Constant = constant;
        }

        public override bool Equals(object? obj) =>
            obj is AssignNode node && Name.Equals(node.Name) && Constant.Equals(node.Constant);

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Name, Constant);
    }
}
