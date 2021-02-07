using System;

namespace SCUMSLang.AST
{
    public class AssignNode : Node
    {
        public override NodeType NodeType => NodeType.Assignment;

        public string DeclarationName { get; }
        public ConstantNode Constant { get; }
        public DeclarationNode Declaration { get; internal set; } = null!;

        public AssignNode(string declarationName, ConstantNode constant)
        {
            DeclarationName = declarationName;
            Constant = constant;
        }

        public override bool Equals(object? obj) =>
            obj is AssignNode node && DeclarationName.Equals(node.DeclarationName) && Constant.Equals(node.Constant);

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, DeclarationName, Constant);
    }
}
