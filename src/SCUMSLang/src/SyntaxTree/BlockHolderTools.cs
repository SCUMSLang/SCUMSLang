namespace SCUMSLang.SyntaxTree
{
    public static class BlockHolderTools
    {
        public static void SetupBlock(ref BlockDefinition? block, BlockDefinition parentBlock, BlockScope blockScope)
        {
            if (block is not null) {
                throw SyntaxTreeThrowHelper.NonNullBlockSetup();
            }

            block = new BlockDefinition.ScopedBlockDefinition(parentBlock, blockScope);
        }
    }
}
