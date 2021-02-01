using SCUMSLang.Tokenization;
using System;
using System.Runtime.Serialization;

namespace SCUMSLang
{
    public class MissingTokenException : ParseException
    {
        public TokenType MissingTokenType { get; }

        public MissingTokenException(Token lastToken, TokenType missingTokenType)
            : base(lastToken) =>
            MissingTokenType = missingTokenType;

        public MissingTokenException(Token lastToken, TokenType missingTokenType, string? message)
            : base(lastToken, message) =>
            MissingTokenType = missingTokenType;

        public MissingTokenException(Token lastToken, TokenType missingTokenType, string? message, Exception? innerException)
            : base(lastToken, message, innerException) =>
            MissingTokenType = missingTokenType;

        protected MissingTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
