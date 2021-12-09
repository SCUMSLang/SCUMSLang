using SCUMSLang.Tokenization;
using System;
using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree.Parser
{
    [Serializable]
    public class SyntaxTreeParsingException : Exception, IParsingException
    {
        public IFilePosition? FilePosition { get; }

        protected SyntaxTreeParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        protected SyntaxTreeParsingException(Token nearToken) =>
            FilePosition = nearToken;

        public SyntaxTreeParsingException(Token nearToken, string? message)
            : base(message) =>
            FilePosition = nearToken;

        public SyntaxTreeParsingException(Token nearToken, string? message, Exception? innerException)
            : base(message, innerException) =>
            FilePosition = nearToken;
    }
}
