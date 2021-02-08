using System;

namespace SCUMSLang.AST
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InBuiltTypeAttribute : Attribute
    {
        public InBuiltType InBuiltType { get; }
        public bool IsEnumeration { get; set; }

        public InBuiltTypeAttribute(InBuiltType inBuiltType) =>
            InBuiltType = inBuiltType;
    }
}
