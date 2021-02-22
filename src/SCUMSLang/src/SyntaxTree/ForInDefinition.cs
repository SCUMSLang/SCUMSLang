using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class ForInDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => 
            SyntaxTreeNodeType.ForInDefinition;

        public TypeReference Parameter { get; }
        public IReadOnlyList<ConstantDefinition> Arguments { get; }

        public ForInDefinition(TypeReference parameter, IReadOnlyList<ConstantDefinition> arguments)
        {
            Parameter = parameter;
            Arguments = arguments;
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitForInDefinition(this);
    }
}
