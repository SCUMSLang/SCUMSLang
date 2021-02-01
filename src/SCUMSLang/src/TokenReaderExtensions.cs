using SCUMSLang.Tokenization;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang
{
    public static class TokenReaderExtensions
    {
        public static bool ConsumeNext(this ref Reader<Token> reader, TokenType tokenType, [MaybeNullWhen(false)] out Token token)
        {
            if (reader.ConsumeNext(tokenType, TokenOnlyTypeComparer.Default))
            {
                token = reader.View.Last();
                return true;
            }

            token = null;
            return false;
        }

        public static bool ConsumeNext(this ref Reader<Token> reader, TokenType tokenType) =>
            ConsumeNext(ref reader, tokenType, out _);

        public static bool PeekNext(this ref Reader<Token> reader, TokenType tokenType)
        {
            if (reader.PeekNext(tokenType, TokenOnlyTypeComparer.Default))
            {
                return true;
            }

            return false;
        }

        public static bool TakeNextPositionView(this ref Reader<Token> reader, TokenType tokenType)
        {
            if (reader.TakeNextPositionView(tokenType, TokenOnlyTypeComparer.Default))
            {
                return true;
            }

            return false;
        }
    }
}
