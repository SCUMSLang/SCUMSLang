using System;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public sealed class ConstantDefinition : Reference, IResolvableDependencies
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

        protected void ResolveDependencies() =>
            ValueType.Resolve();

        void IResolvableDependencies.ResolveDependencies() =>
            ResolveDependencies();

        public override bool Equals(object? obj) =>
            base.Equals(obj) && obj is ConstantDefinition constant
            && Equals(constant.Value, Value)
            && Equals(constant.ValueType, ValueType);

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, ValueType, Value);

        public ConstantDefinition Rewrite(TypeReference valueType) {
            if (ReferenceEquals(ValueType, valueType)) {
                return this;
            }

            return new ConstantDefinition(valueType, Value);
        }

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitConstantDefinition(this);
    }
}
