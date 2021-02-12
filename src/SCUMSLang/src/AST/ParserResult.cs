namespace SCUMSLang.AST
{
    public class ParserResult
    {
        public StaticBlockNode StaticBlock { get; }
        public int TokenReaderUpperPosition { get; }

        public ParserResult(StaticBlockNode staticBlock, int tokenReaderUpperPosition)
        {
            StaticBlock = staticBlock;
            TokenReaderUpperPosition = tokenReaderUpperPosition;
        }
    }
}
