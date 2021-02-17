using SCUMSLang.Tokenization;
using System;
using System.Runtime.Serialization;

namespace SCUMSLang
{
    [Serializable]
    public class SyntaxTreeException : Exception
    {
        public int Position { get; }
        public int Length { get; }

        public SyntaxTreeException(Token nearToken)
        {
            Position = nearToken.Position;
            Length = nearToken.Length;
        }

        public SyntaxTreeException(Token nearToken, string? message)
            : base(message)
        {
            Position = nearToken.Position;
            Length = nearToken.Length;
        }

        public SyntaxTreeException(Token nearToken, string? message, Exception? innerException)
            : base(message, innerException)
        {
            Position = nearToken.Position;
            Length = nearToken.Length;
        }

        public SyntaxTreeException(int position, int length)
        {
            Position = position;
            Length = length;
        }

        public SyntaxTreeException(int position, int length, string? message)
            : base(message)
        {
            Position = position;
            Length = length;
        }

        public SyntaxTreeException(int position, int length, string? message, Exception? innerException)
            : base(message, innerException)
        {
            Position = position;
            Length = length;
        }

        protected SyntaxTreeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
