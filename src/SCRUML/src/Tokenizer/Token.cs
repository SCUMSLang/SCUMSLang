using System;
using System.Diagnostics.CodeAnalysis;

namespace SCRUML.Tokenizer
{
    public class Token : IEquatable<Token>
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

        public bool Equals([AllowNull] Token other) =>
            ReferenceEquals(this, other) || (!(other is null) && TokenType == other.TokenType);

        public override string ToString() =>
            Enum.GetName(typeof(TokenType), TokenType) ?? "";

        public static implicit operator Token(TokenType tokenType) =>
            new Token(tokenType);
    }
}
