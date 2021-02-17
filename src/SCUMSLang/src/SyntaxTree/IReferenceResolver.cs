namespace SCUMSLang.SyntaxTree
{
    public interface IReferenceResolver
    {
        TypeDefinition Resolve(TypeReference type);
        FieldDefinition Resolve(FieldReference field);
        MethodDefinition Resolve(MethodReference method);
        EventHandlerDefinition Resolve(EventHandlerReference eventHandler);
    }
}
