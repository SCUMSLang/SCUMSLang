namespace SCUMSLang.SyntaxTree
{
    public interface IReferenceResolver
    {
        //void ResolveUsingStaticDirectives();
        TypeDefinition Resolve(TypeReference type);
        FieldDefinition Resolve(FieldReference field);
        MethodDefinition Resolve(MethodReference method);
        EventHandlerDefinition Resolve(EventHandlerReference eventHandler);
    }
}
