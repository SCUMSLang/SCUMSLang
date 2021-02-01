using System;
using System.Runtime.Serialization;

namespace SCUMSLang.Tokenization
{
    [Serializable]
    internal class TokenizationException : Exception
    {
        public int Position { get; }

        public TokenizationException(int position) =>
            Position = position;

        public TokenizationException(int position, string? message)
            : base(message) =>
            Position = position;

        public TokenizationException(int position, string? message, Exception? innerException)
            : base(message, innerException) =>
            Position = position;

        protected TokenizationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}