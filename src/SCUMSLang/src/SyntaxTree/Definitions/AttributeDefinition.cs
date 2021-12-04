using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public sealed class AttributeDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.Attribute;

        public MethodCallDefinition MethodCall { get; }

        public AttributeDefinition(MethodCallDefinition methodCall) =>
            MethodCall = methodCall;

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitAttributeDefinition(this);
    }
}
