using System;

namespace SCUMSLang.AST
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SystemTypeAttribute : Attribute
    {
        public SystemType SystemType { get; }
        public bool IsEnumeration { get; set; }

        public SystemTypeAttribute(SystemType definitionType) =>
            SystemType = definitionType;
    }
}
