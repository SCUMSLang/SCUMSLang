namespace SCUMSLang.AST
{
    public interface IMemberResolver
    {
        TypeDefinition Resolve(TypeReference type);
        DeclarationDefinition Resolve(DeclarationReference declaration);
    }
}
