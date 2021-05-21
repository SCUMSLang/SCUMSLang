namespace SCUMSLang.SyntaxTree
{
    internal interface IBlockHolder
    {
        BlockDefinition? Block { get; set; }
        /// <summary>
        /// Indicates whether the block
        /// holder can own a block.
        /// </summary>
        bool IsBlockOwnable { get; }
    }
}
