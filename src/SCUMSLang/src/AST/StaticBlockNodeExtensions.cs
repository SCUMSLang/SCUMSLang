namespace SCUMSLang.AST
{
    public static class StaticBlockNodeExtensions
    {
        public static StaticBlockNode AddSystemTypes(this StaticBlockNode block) {
            block.AddNode(new TypeDefinitionNode(DefinitionTypeLibrary.Sequences[DefinitionType.Integer], DefinitionType.Integer) {
                AllowOverwriteOnce = true
            });

            block.AddNode(new TypeDefinitionNode(DefinitionTypeLibrary.Sequences[DefinitionType.String], DefinitionType.String) {
                AllowOverwriteOnce = true
            });

            block.AddNode(new EnumerationDefinitionNode(DefinitionTypeLibrary.Sequences[DefinitionType.Boolean], hasReservedNames: true, new[] { "false", "true" }, DefinitionType.Enumeration) {
                AllowOverwriteOnce = true
            });

            return block;
        }
    }
}
