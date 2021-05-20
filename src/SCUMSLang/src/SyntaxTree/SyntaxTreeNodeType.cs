namespace SCUMSLang.SyntaxTree
{
    public enum SyntaxTreeNodeType
    {
        BlockDefinition,
        TypeBlockDefinition,
        ModuleBlockDefinition,

        ImportDefinition,

        MemberReference,
        FieldReference,
        FieldDefinition,
        TypeReference,
        TypeDefinition,
        Attribute,
        DeclarationReference,
        DeclarationDefinition,
        MemberAssignmentDefinition,
        ConstantReference,
        MethodCallDefinition,
        EventHandlerDefinition,
        ParameterReference,
        MethodReference,
        MethodDefinition,
        TypeSpecification,
        UsingStaticDirective,
        ForInDefinition,
        TemplateForInDefinition,
    }
}
