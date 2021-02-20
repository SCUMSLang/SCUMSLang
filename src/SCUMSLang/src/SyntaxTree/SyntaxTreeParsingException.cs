using SCUMSLang.Tokenization;
using System;
using System.Runtime.Serialization;

namespace SCUMSLang
{
    [Serializable]
    public class SyntaxTreeParsingException : Exception
    {
        public int FilePosition { get; private set; }
        public int FilePositionLength { get; private set; }
        public int FileLine { get; private set; }
        public int FileLinePosition { get; internal set; }
        public string? FilePath { get; private set; }

        protected SyntaxTreeParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public SyntaxTreeParsingException(Token nearToken) =>
            onConstruction(nearToken);

        public SyntaxTreeParsingException(Token nearToken, string? message)
            : base(message) =>
            onConstruction(nearToken);

        public SyntaxTreeParsingException(Token nearToken, string? message, Exception? innerException)
            : base(message, innerException) =>
            onConstruction(nearToken);

        private void onConstruction(Token nearToken)
        {
            FilePosition = nearToken.FilePosition;
            FilePositionLength = nearToken.FilePositionLength;
            FileLine = nearToken.FileLine;
            FileLinePosition = nearToken.FileLinePosition;
            FilePath = nearToken.FilePath;
        }
    }
}
