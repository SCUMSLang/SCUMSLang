using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public static class ReferenceParserExtensions
    {
        public static ReferenceParserResult Parse(this ReferenceParser parser, string content)
        {
            var tokens = Tokenizer.Tokenize(content).ToArray();
            return parser.Parse(tokens);
        }
    }
}
