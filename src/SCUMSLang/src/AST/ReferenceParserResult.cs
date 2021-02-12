namespace SCUMSLang.AST
{
    public class ReferenceParserResult
    {
        public ModuleDefinition Module { get; }
        public int TokenReaderUpperPosition { get; }

        public ReferenceParserResult(ModuleDefinition staticBlock, int tokenReaderUpperPosition)
        {
            Module = staticBlock;
            TokenReaderUpperPosition = tokenReaderUpperPosition;
        }
    }
}
