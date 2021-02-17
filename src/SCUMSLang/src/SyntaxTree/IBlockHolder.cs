namespace SCUMSLang.SyntaxTree
{
    internal interface IBlockHolder
    {
        string Name { get; }
        bool IsExandable { get; }
        BlockDefinition? Block { get; set; }
        TypeReference DeclaringType { get; }
    }
}
