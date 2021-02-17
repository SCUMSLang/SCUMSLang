using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.Tokenization
{
    public static class TokenReaderExtensions
    {
        public static bool ConsumeNext(this ref SpanReader<Token> reader, TokenType tokenType, [MaybeNullWhen(false)] out Token token)
        {
            if (reader.ConsumeNext(tokenType, TokenOnlyTypeComparer.Default)) {
                token = reader.ViewLastValue;
                return true;
            }

            token = null;
            return false;
        }

        public static bool ConsumeNext(this ref SpanReader<Token> reader, TokenType tokenType) =>
            reader.ConsumeNext(tokenType, out _);

        public static bool PeekNext(this ref SpanReader<Token> reader, int count, TokenType tokenType)
        {
            if (reader.PeekNext(count, tokenType, TokenOnlyTypeComparer.Default)) {
                return true;
            }

            return false;
        }

        public static bool PeekNext(this ref SpanReader<Token> reader, TokenType tokenType) =>
            reader.PeekNext(1, tokenType);
    }
}
