using System;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public class ParameterReference : Reference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.ParameterReference;

        public TypeReference ParameterType { get; protected set; } = null!;

        internal ParameterReference(TypeReference parameterType) =>
            ParameterType = parameterType ?? throw new ArgumentNullException(nameof(parameterType));

        protected internal override Reference Accept(NodeVisitor visitor) =>
            visitor.VisitParameterReference(this);
    }
}
