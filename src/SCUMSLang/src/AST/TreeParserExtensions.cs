using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public static class TreeParserExtensions
    {
        public static TreeParserResult Parse(this TreeParser parser, string content)
        {
            var tokens = Tokenizer.Tokenize(content).ToArray();
            return parser.Parse(tokens);
        }
    }
}
