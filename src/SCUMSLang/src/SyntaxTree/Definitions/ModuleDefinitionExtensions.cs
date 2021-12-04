using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public static class ModuleDefinitionExtensions
    {
        /// <summary>
        /// Adds the system types to the module.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="definitionOwner"></param>
        /// <returns></returns>
        public static ModuleDefinition AddSystemTypes(this ModuleDefinition module)
        {
            module.Block.AddNode(new TypeDefinition(SystemTypeLibrary.Sequences[SystemType.Integer]) {
                AllowOverwriteOnce = true
            });

            module.Block.AddNode(new TypeDefinition(SystemTypeLibrary.Sequences[SystemType.String]) {
                AllowOverwriteOnce = true
            });

            var boolEnumType = Reference.CreateEnumDefinition(
                SystemTypeLibrary.Sequences[SystemType.Boolean],
                new[] { "false", "true" },
                fieldsAreConstants: true,
                allowOverwriteOnce: true,
                blockContainer: BlockContainer.WithBlock(module.Block));

            module.Block.AddNode(boolEnumType);
            return module;
        }
    }
}
