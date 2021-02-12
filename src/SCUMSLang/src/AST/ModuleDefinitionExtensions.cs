namespace SCUMSLang.AST
{
    public static class ModuleDefinitionExtensions
    {
        public static ModuleDefinition AddSystemTypes(this ModuleDefinition module) {
            module.AddNode(new TypeDefinition(SystemTypeLibrary.Sequences[SystemType.Integer], SystemType.Integer) {
                AllowOverwriteOnce = true
            });

            module.AddNode(new TypeDefinition(SystemTypeLibrary.Sequences[SystemType.String], SystemType.String) {
                AllowOverwriteOnce = true
            });

            module.AddNode(new EnumerationTypeReference(SystemTypeLibrary.Sequences[SystemType.Boolean], hasReservedNames: true, new[] { "false", "true" }, SystemType.Enumeration) {
                AllowOverwriteOnce = true
            });

            return module;
        }
    }
}
