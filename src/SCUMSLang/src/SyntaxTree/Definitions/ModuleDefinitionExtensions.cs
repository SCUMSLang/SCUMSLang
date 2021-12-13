using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public static class ModuleDefinitionExtensions
    {
        /// <summary>
        /// Adds the system types to the module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static ModuleDefinition AddSystemTypes(this ModuleDefinition module)
        {
            var blockContainer = BlockContainer.WithBlock(module.Block);

            var uint8Type = Reference.CreateTypeDefinition(
                SystemType.Byte,
                allowOverwriteOnce: true,
                blockContainer: blockContainer);

            module.Block.AddNode(uint8Type);
            module.Block.AddNode(Reference.CreateAliasDefinition("byte", uint8Type, blockContainer));

            var uint32Type = Reference.CreateTypeDefinition(
                SystemType.Integer,
                allowOverwriteOnce: true,
                blockContainer: blockContainer);

            module.Block.AddNode(uint32Type);
            module.Block.AddNode(Reference.CreateAliasDefinition("int", uint32Type, blockContainer));

            var stringType = Reference.CreateTypeDefinition(SystemType.String,
                allowOverwriteOnce: true,
                blockContainer: blockContainer);

            module.Block.AddNode(stringType);
            module.Block.AddNode(Reference.CreateAliasDefinition("string", stringType, blockContainer));

            var boolEnumType = Reference.CreateEnumDefinition(
                Sequences.SystemTypes[SystemType.Boolean],
                uint32Type,
                new[] { "false", "true" },
                fieldsAreConstants: true,
                allowOverwriteOnce: true,
                blockContainer: blockContainer);

            module.Block.AddNode(boolEnumType);
            module.Block.AddNode(Reference.CreateAliasDefinition("bool", boolEnumType, blockContainer));
            return module;
        }
    }
}
