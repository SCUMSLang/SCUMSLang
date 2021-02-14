using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class ConstantDefinition : Reference, IResolvableDependencies
    {
        public override TreeTokenType TokenType => TreeTokenType.ConstantReference;
        public TypeReference ValueType { get; }
        public virtual object? Value { get; }

        public ConstantDefinition(TypeReference valueType, object? value)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is ConstantDefinition node)) {
                return false;
            }

            var equals = ValueType.Equals(node.ValueType)
                && Equals(Value, node.Value);

            Trace.WriteLineIf(!equals, $"{nameof(ConstantDefinition)} not equals.");
            return equals;
        }

        protected void ResolveDependencies() =>
            ValueType.Resolve();

        void IResolvableDependencies.ResolveDependencies() =>
            ResolveDependencies();

        public override int GetHashCode() =>
            HashCode.Combine(TokenType, ValueType, Value);
    }
}
