using SCUMSLang.AST;
using System;

namespace SCUMSLang.Tokenization
{
    public static class TokenTools
    {
        internal static Token CreateToken(TokenType tokenType, object? value, in Reader<char> reader) =>
            new Token(tokenType, reader.ReadPosition, reader.ViewReadLength, value);

        internal static Token CreateToken(TokenType tokenType, in Reader<char> reader) =>
            new Token(tokenType, reader.ReadPosition, reader.ViewReadLength);
    }
}
