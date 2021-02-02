using System;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.Tokenization
{
    public class Token : IEquatable<Token>
    {
        public TokenType TokenType { get; }
        public object? Value { get; }
        public int Position { get; }
        public int Length { get; }
        public int UpperBound => Position + Length;

        public Token(TokenType tokenType, int position, int length, object? value)
        {
            TokenType = tokenType;
            Value = value;
            Position = position;
            Length = length;
        }

        public Token(TokenType tokenType, int position, int length)
        {
            TokenType = tokenType;
            Value = default!;
            Position = position;
            Length = length;
        }

        public bool TryRecognize<T>(TokenType tokenType, [NotNullWhen(true)] out T value)
        {
            if (TokenType == tokenType) {
                value = (T)Value!;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryRecognize(params TokenType[] tokenTypes)
        {
            var index = tokenTypes.Length;

            while (--index >= 0) {
                if (TokenType == tokenTypes[index]) {
                    return true;
                }
            }

            return false;
        }

        public bool TryRecognize(TokenType tokenType, out Token token)
        {
            if (TokenType == tokenType) {
                token = this;
                return true;
            }

            token = default;
            return false;
        }

        public T GetValue<T>() =>
            (T)Value ?? throw new InvalidOperationException();

        public bool TryGetValue<T>([MaybeNullWhen(false)]out T value) {
            if (Value is T typedValue) {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        public bool Equals([AllowNull] Token other) =>
            ReferenceEquals(this, other) || (!(other is null) && TokenType == other.TokenType);

        public override string ToString() =>
            Enum.GetName(typeof(TokenType), TokenType) ?? "";

        public static implicit operator Token(TokenType tokenType) =>
            new Token(tokenType, 0, 0);

        public static bool operator ==(Token token, TokenType tokenType) =>
            token.TokenType == tokenType;

        public static bool operator !=(Token token, TokenType tokenType) =>
            token.TokenType != tokenType;
    }
}
