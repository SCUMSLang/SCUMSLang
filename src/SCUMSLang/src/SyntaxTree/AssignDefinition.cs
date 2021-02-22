using System;
using System.Diagnostics;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed class AssignDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.AssignmentDefinition;

        public string FieldName { get; }
        public ConstantDefinition Constant { get; }

        public AssignDefinition(string fieldName, ConstantDefinition constant)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is AssignDefinition assignment)) {
                return false;
            }

            var equals = FieldName == assignment.FieldName
                && Constant.Equals(assignment.Constant);

            Trace.WriteLineIf(!equals, $"{nameof(AssignDefinition)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, FieldName, Constant);

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitAssignDefinition(this);
    }
}
