namespace SCUMSLang.AST
{
    public class TreeParserResult
    {
        public ModuleDefinition Module { get; }
        public int TokenReaderUpperPosition { get; }

        public TreeParserResult(ModuleDefinition staticBlock, int tokenReaderUpperPosition)
        {
            Module = staticBlock;
            TokenReaderUpperPosition = tokenReaderUpperPosition;
        }
    }
}
