namespace SCRUML.Tokenizer
{
    public class Token
    {
        public TokenType TokenType { get; }
        public object? Value { get; }

        public Token(TokenType tokenType, object? value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public Token(TokenType tokenType) =>
            TokenType = tokenType;
    }
}
