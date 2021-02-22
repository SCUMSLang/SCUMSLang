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

        public override bool Equals(object? obj) => 
            obj is AttributeDefinition definition && base.Equals(obj) 
            && NodeType == definition.NodeType 
            && EqualityComparer<MethodCallDefinition>.Default.Equals(MethodCall, definition.MethodCall);

        internal protected override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitAttributeDefinition(this);
    }
}
