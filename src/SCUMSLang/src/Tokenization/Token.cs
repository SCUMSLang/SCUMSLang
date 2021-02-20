using System;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.Tokenization
{
    public class Token
    {
        public TokenType TokenType { get; }
        public object? Value { get; }
        public int FilePosition { get; internal set; }
        public int FilePositionLength { get; }
        public int FileLine { get; internal set; }
        public int FileLinePosition { get; internal set; }
        public string? FilePath { get; internal set; }
        /// <summary>
        /// The channel at which the token is belonging. If not other
        /// specified it is by default <see cref="Channel.Parser"/>.
        /// </summary>
        public Channel Channel { get; }
        public int UpperPosition => FilePosition + FilePositionLength - 1;

        public Token(TokenType tokenType, int position, int length, object? value, Channel channel)
        {
            TokenType = tokenType;
            Value = value;
            Channel = channel;
            FilePosition = position;
            FilePositionLength = length;
        }

        public Token(TokenType tokenType, int position, int length, object? value)
            : this(tokenType, position, length, value, Channel.Parser) { }

        public Token(TokenType tokenType, int position, int length, Channel channel)
            : this(tokenType, position, length, default, channel) { }

        public Token(TokenType tokenType, int position, int length)
            : this(tokenType, position, length, default, Channel.Parser) { }

        public bool TryRecognize<T>(TokenType tokenType, [NotNullWhen(true)] out T value)
        {
            if (TokenType == tokenType) {
                value = (T)Value!;
                return true;
            }

            value = default!;
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

        public bool TryRecognize(TokenType tokenType, [MaybeNullWhen(false)] out Token token)
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

        public bool TryGetValue<T>([MaybeNullWhen(false)] out T value)
        {
            if (Value is T typedValue) {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        public override bool Equals(object? obj) =>
            obj is Token other && (ReferenceEquals(this, other) || (!(other is null) && TokenType == other.TokenType));

        public override int GetHashCode() =>
            HashCode.Combine(TokenType, Value, FilePosition, FilePositionLength);

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
