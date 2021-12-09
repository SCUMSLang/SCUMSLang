using System;
using System.Runtime.Serialization;

namespace SCUMSLang.Tokenization
{
    [Serializable]
    internal class TokenParsingException : Exception, IFilePosition, IParsingException
    {
        public int FilePosition { get; }
        public byte FilePositionOffset { get; init; }
        public int FileLine { get; internal set; }
        public int FileLinePosition { get; init; }
        public string? FilePath { get; internal set; }

        int IFilePosition.FilePositionLength => 1;

        IFilePosition? IFilePositionable.FilePosition => this;

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
