using SCUMSLang.Tokenization;
using System;
using System.Runtime.Serialization;

namespace SCUMSLang
{
    [Serializable]
    public class ParseException : Exception
    {
        public int Position { get; }
        public int Length { get; }

        public ParseException(Token nearToken)
        {
            Position = nearToken.Position;
            Length = nearToken.Length;
        }

        public ParseException(Token nearToken, string? message)
            : base(message)
        {
            Position = nearToken.Position;
            Length = nearToken.Length;
        }

        public ParseException(Token nearToken, string? message, Exception? innerException)
            : base(message, innerException)
        {
            Position = nearToken.Position;
            Length = nearToken.Length;
        }

        public ParseException(int position, int length)
        {
            Position = position;
            Length = length;
        }

        public ParseException(int position, int length, string? message)
            : base(message)
        {
            Position = position;
            Length = length;
        }

        public ParseException(int position, int length, string? message, Exception? innerException)
            : base(message, innerException)
        {
            Position = position;
            Length = length;
        }

        protected ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
