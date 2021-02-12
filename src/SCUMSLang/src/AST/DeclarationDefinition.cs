namespace SCUMSLang.AST
{
    public class DeclarationDefinition : DeclarationReference, IMemberDefinition
    {
        public override TreeTokenType ReferenceType => TreeTokenType.DeclarationDefinition;

        public DeclarationDefinition(Scope scope, TypeReference type, string name) : base(scope, type, name)
        {
        }
    }
}
