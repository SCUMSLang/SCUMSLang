using SCUMSLang.Tokenization;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang
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
            ConsumeNext(ref reader, tokenType, out _);

        public static bool PeekNext(this ref SpanReader<Token> reader, int count, TokenType tokenType)
        {
            if (reader.PeekNext(count, tokenType, TokenOnlyTypeComparer.Default)) {
                return true;
            }

            return false;
        }

        public static bool PeekNext(this ref SpanReader<Token> reader, TokenType tokenType) =>
            PeekNext(ref reader, 1, tokenType);
    }
}
