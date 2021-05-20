namespace SCUMSLang.SyntaxTree
{
    internal interface IBlockHolder
    {
        bool IsExpandable { get; }
        BlockDefinition? Block { get; set; }
    }
}
