namespace SCUMSLang.Tokenization
{
    public static class TokenTools
    {
        internal static Token CreateToken(TokenType tokenType, object? value, SpanReader<char> reader) =>
            new Token(tokenType, reader.ReadPosition, reader.ViewReadLength, value);

        internal static Token CreateToken(TokenType tokenType, SpanReader<char> reader) =>
            new Token(tokenType, reader.ReadPosition, reader.ViewReadLength);
    }
}
