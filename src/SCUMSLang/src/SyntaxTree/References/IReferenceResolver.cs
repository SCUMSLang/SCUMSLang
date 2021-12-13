using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree.References
{
    public interface IReferenceResolver
    {
        ResolveResult<T> Resolve<T>(TypeReference type)
            where T : TypeReference;

        ResolveResult<TypeReference> Resolve(TypeReference type);
        ResolveResult<FieldDefinition> Resolve(FieldReference field);
        ResolveResult<MethodDefinition> Resolve(MethodReference method);
        ResolveResult<EventHandlerDefinition> Resolve(EventHandlerReference eventHandler);
        ResolveResult<TypeDefinition> GetType(string typeName);
        ResolveResult<EventHandlerDefinition> GetEventHandler(string eventHandlerName);
        ResolveResult<FieldDefinition> GetField(string fieldName);
        ResolveResult<MethodDefinition> GetMethod(string methodName);
        ResolveResult<MethodDefinition> GetMethod(MethodReference methodReference);
    }
}
