using System;
using System.Diagnostics;

namespace SCUMSLang.AST
{
    public class ConstantNode : Node
    {
        public override NodeType NodeType => NodeType.Constant;
        public TypeDefinitionNode Type { get; }
        public virtual object? Value { get; }

        public ConstantNode(TypeDefinitionNode Type, object? value)
        {
            this.Type = Type ?? throw new ArgumentNullException(nameof(Type));
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is ConstantNode node)) {
                return false;
            }

            var equals = Type.Equals(node.Type)
                && Equals(Value, node.Value);

            Trace.WriteLineIf(!equals, $"{nameof(ConstantNode)} not equals.");
            return equals;
        }

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, Type, Value);
    }
}
