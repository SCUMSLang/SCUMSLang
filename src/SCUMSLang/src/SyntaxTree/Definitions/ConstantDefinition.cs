using System;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public sealed class ConstantDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.ConstantReference;

        public TypeReference ValueType { get; }
        public object? Value { get; }

        public ConstantDefinition(TypeReference valueType, object? value)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Value = value;
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitConstantDefinition(this);

        public ConstantDefinition UpdateDefinition(TypeReference valueType)
        {
            if (ReferenceEquals(ValueType, valueType)) {
                return this;
            }

            return new ConstantDefinition(valueType, Value);
        }
    }
}
