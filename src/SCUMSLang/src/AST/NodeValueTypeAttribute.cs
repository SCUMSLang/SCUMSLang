using System;

namespace SCUMSLang.AST
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NodeValueTypeAttribute : Attribute
    {
        public NodeValueType ValueType { get; }
        public bool IsDeclarable { get; set; }

        public NodeValueTypeAttribute(NodeValueType valueType) =>
            ValueType = valueType;
    }
}
