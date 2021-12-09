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
            module.Block.AddNode(Reference.CreateTypeDefinition(
                SystemType.Integer,
                blockContainer: null,
                allowOverwriteOnce: true));

            module.Block.AddNode(Reference.CreateTypeDefinition(SystemType.String,
                blockContainer: null,
                allowOverwriteOnce: true));

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
