using SCUMSLang.Tokenization;
using System;
using System.Runtime.Serialization;

namespace SCUMSLang
{
    [Serializable]
    public class ParseException : Exception
    {
        public Token LastToken { get; }

        public ParseException(Token lastToken)
        {
            LastToken = lastToken;
        }

        public ParseException(Token lastToken, string? message)
            : base(message)
        {
            LastToken = lastToken;
        }

        public ParseException(Token lastToken, string? message, Exception? innerException)
            : base(message, innerException)
        {
            LastToken = lastToken;
        }

        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
