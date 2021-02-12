using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public static class ParserExtensions
    {
        public static ParserResult Parse(this Parser parser, string content)
        {
            var tokens = Tokenizer.Tokenize(content).ToArray();
            return parser.Parse(tokens);
        }
    }
}
