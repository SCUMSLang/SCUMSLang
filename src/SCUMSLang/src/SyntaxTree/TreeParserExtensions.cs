using SCUMSLang.Tokenization;

namespace SCUMSLang.SyntaxTree
{
    public static class TreeParserExtensions
    {
        public static SyntaxTreeParserResult Parse(this SyntaxTreeParser parser, string content)
        {
            var tokens = Tokenizer.Tokenize(content).ToArray();
            return parser.Parse(tokens);
        }
    }
}
