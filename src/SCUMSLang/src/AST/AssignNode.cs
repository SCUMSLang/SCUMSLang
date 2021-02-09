using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class AssignNode : Node
    {
        public override NodeType NodeType => NodeType.Assignment;

        public ConstantNode Constant { get; }
        public DeclarationNode Declaration { get; }

        public AssignNode(DeclarationNode declaration, ConstantNode constant)
        {
            Declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is AssignNode assignment)) {
                return false;
            }

            var equals = Declaration.Equals(assignment.Declaration)
                && Constant.Equals(assignment.Constant);

            Trace.WriteLineIf(!equals, $"{nameof(AssignNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Declaration, Constant);
    }
}
