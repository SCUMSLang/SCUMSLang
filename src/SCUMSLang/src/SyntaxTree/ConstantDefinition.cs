using System;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
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

        public ConstantDefinition Update(TypeReference valueType)
        {
            if (ReferenceEquals(ValueType, valueType)) {
                return this;
            }

            return new ConstantDefinition(valueType, Value);
        }
    }
}
