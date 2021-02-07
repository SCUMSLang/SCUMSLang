using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public static class ParserExtensions
    {
        public static BlockNode Parse(this Parser parser, string content, ParserOptions ? options = null)
        {
            var tokens = Tokenizer.Tokenize(content).ToArray();
            return parser.Parse(tokens, options: options);
        }
    }
}
