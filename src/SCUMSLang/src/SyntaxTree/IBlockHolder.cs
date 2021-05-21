namespace SCUMSLang.SyntaxTree
{
    internal interface IBlockHolder
    {
        BlockDefinition? Block { get; }
        /// <summary>
        /// Indicates whether the block
        /// holder can own a block.
        /// </summary>
        bool IsBlockOwnable { get; }

        void SetupBlock(BlockDefinition parentBlock);
    }
}
