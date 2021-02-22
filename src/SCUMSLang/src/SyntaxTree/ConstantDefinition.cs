using System;

namespace SCUMSLang.SyntaxTree
{
    public class ConstantDefinition : Reference, IResolvableDependencies
    {
        public override SyntaxTreeNodeType NodeType => 
            SyntaxTreeNodeType.ConstantReference;

        public TypeReference ValueType { get; }
        public virtual object? Value { get; }

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
    }
}
