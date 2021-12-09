using System;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public sealed class MemberAssignmenDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.MemberAssignmentDefinition;

        public FieldReference Field { get; }
        public ConstantDefinition Constant { get; }

        public MemberAssignmenDefinition(FieldReference field, ConstantDefinition constant)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Field, Constant);

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitMemberAssignmentDefinition(this);
    }
}
