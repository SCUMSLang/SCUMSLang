namespace SCUMSLang.AST
{
    public static class ModuleDefinitionExtensions
    {
        public static ModuleDefinition AddSystemTypes(this ModuleDefinition module)
        {
            module.Import(new TypeDefinition(module, SystemTypeLibrary.Sequences[SystemType.Integer]) {
                AllowOverwriteOnce = true
            });

            module.Import(new TypeDefinition(module, SystemTypeLibrary.Sequences[SystemType.String]) {
                AllowOverwriteOnce = true
            });

            var booleanType = TypeDefinition.CreateEnumDefinition(
                module,
                SystemTypeLibrary.Sequences[SystemType.Boolean],
                new[] { "false", "true" },
                usableAsConstants: true,
                allowOverwriteOnce: true);

            module.Import(booleanType);
            return module;
        }
    }
}
