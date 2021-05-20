using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public class ForInDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType => 
            SyntaxTreeNodeType.ForInDefinition;

        public ParameterDefinition Parameter { get; }
        public IReadOnlyList<ConstantDefinition> Arguments { get; }

        public ForInDefinition(ParameterDefinition parameter, IReadOnlyList<ConstantDefinition> arguments)
        {
            Parameter = parameter;
            Arguments = arguments;
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitForInDefinition(this);

        public ForInDefinition Update(ParameterDefinition parameter, IReadOnlyList<ConstantDefinition> arguments) {
            if (ReferenceEquals(Parameter, parameter) && Enumerable.SequenceEqual(Arguments, arguments, ReferenceEqualityComparer.Instance)) {
                return this;
            }

            return new ForInDefinition(parameter, arguments);
        }
    }
}
