﻿using System.Linq;

namespace SCUMSLang.SyntaxTree
{
    public static class TypeDefinitionExtensions
    {
        public static FieldDefinition GetField(this TypeDefinition type, string name) =>
            type.Fields.First(f => f.Name == name);
    }
}
