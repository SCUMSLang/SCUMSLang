using SCUMSLang.Tokenization;

namespace SCUMSLang.SyntaxTree.Parser
{
    public static class SyntaxTreeParserExtensions
    {
        public static SyntaxTreeParserResult Parse(this SyntaxTreeParser parser, string content)
        {
            var tokens = Tokenizer.Tokenize(content).ToArray();
            return parser.Parse(tokens);
        }
    }
}
