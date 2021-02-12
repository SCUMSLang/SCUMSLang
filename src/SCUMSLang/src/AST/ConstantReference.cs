using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class ConstantReference : Reference
    {
        public override TreeTokenType ReferenceType => TreeTokenType.Constant;
        public TypeReference ValueType { get; }
        public virtual object? Value { get; }

        public ConstantReference(TypeReference valueType, object? value)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is ConstantReference node)) {
                return false;
            }

            var equals = ValueType.Equals(node.ValueType)
                && Equals(Value, node.Value);

            Trace.WriteLineIf(!equals, $"{nameof(ConstantReference)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(ReferenceType, ValueType, Value);
    }
}
