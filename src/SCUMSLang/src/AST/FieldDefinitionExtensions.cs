﻿namespace SCUMSLang.AST
{
    public static class FieldDefinitionExtensions
    {
        public static T GetValue<T>(this FieldDefinition field) =>
            (T)field.Value!;
    }
}
