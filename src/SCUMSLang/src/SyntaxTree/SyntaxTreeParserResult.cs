namespace SCUMSLang.SyntaxTree
{
    public class SyntaxTreeParserResult
    {
        public ModuleDefinition Module { get; }
        public int TokenReaderUpperPosition { get; }

        public SyntaxTreeParserResult(ModuleDefinition staticBlock, int tokenReaderUpperPosition)
        {
            Module = staticBlock;
            TokenReaderUpperPosition = tokenReaderUpperPosition;
        }
    }
}
