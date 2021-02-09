using System;

namespace SCUMSLang.AST
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefinitionTypeAttribute : Attribute
    {
        public DefinitionType DefinitionType { get; }
        public bool IsEnumeration { get; set; }

        public DefinitionTypeAttribute(DefinitionType definitionType) =>
            DefinitionType = definitionType;
    }
}
