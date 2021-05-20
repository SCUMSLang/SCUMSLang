using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed class AttributeDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => SyntaxTreeNodeType.Attribute;
        public MethodCallDefinition MethodCall { get; }

        public AttributeDefinition(MethodCallDefinition functionCall) =>
            MethodCall = functionCall;

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitAttributeDefinition(this);
    }
}
