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
            var blockContainer = BlockContainer.WithBlock(module.Block);

            module.Block.AddNode(Reference.CreateTypeDefinition(
                SystemType.Byte,
                allowOverwriteOnce: true,
                blockContainer: blockContainer));

            module.Block.AddNode(Reference.CreateTypeDefinition(
                SystemType.Integer,
                allowOverwriteOnce: true,
                blockContainer: blockContainer));

            module.Block.AddNode(Reference.CreateTypeDefinition(SystemType.String,
                allowOverwriteOnce: true,
                blockContainer: blockContainer));

            var boolEnumType = Reference.CreateEnumDefinition(
                Sequences.SystemTypes[SystemType.Boolean],
                new[] { "false", "true" },
                fieldsAreConstants: true,
                allowOverwriteOnce: true,
                blockContainer: blockContainer);

            module.Block.AddNode(boolEnumType);
            return module;
        }
    }
}
