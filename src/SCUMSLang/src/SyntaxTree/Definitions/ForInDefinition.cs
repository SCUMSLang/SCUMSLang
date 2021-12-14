using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
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

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitForInDefinition(this);

        public ForInDefinition Update(ParameterDefinition parameter, IReadOnlyList<ConstantDefinition> arguments)
        {
            if (ReferenceEquals(Parameter, parameter) && Arguments.SequenceEqual(arguments, ReferenceEqualityComparer.Instance)) {
                return this;
            }

            return new ForInDefinition(parameter, arguments);
        }
    }
}
