using System;

namespace SCUMSLang.AST
{
    public class ConstantNode : Node
    {
        public override NodeType NodeType => NodeType.Constant;
        public NodeValueType ValueType { get; }
        public object? Value { get; }

        public ConstantNode(NodeValueType valueType, object value)
        {
            ValueType = valueType;
            Value = value;
        }

        public override bool Equals(object? obj) =>
            obj is ConstantNode node && ValueType == node.ValueType && Equals(Value, node.Value);

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, ValueType, Value);
    }
}
