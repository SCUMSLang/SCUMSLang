using System;
using System.Runtime.Serialization;

namespace SCUMSLang.Tokenization
{
    [Serializable]
    internal class TokenParsingException : Exception
    {
        public int FilePosition { get; }
        public int FileLine { get; internal set; }
        public int FileLinePosition { get; internal set; }

        public TokenParsingException(int position) =>
            FilePosition = position;

        public TokenParsingException(int position, string? message)
            : base(message) =>
            FilePosition = position;

        public TokenParsingException(int position, string? message, Exception? innerException)
            : base(message, innerException) =>
            FilePosition = position;

        protected TokenParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
