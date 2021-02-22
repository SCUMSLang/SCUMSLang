using System;
using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree
{
    public class BlockEvaluatingException : Exception
    {
        protected BlockEvaluatingException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public BlockEvaluatingException() { }

        public BlockEvaluatingException(string? message)
            : base(message) { }

        public BlockEvaluatingException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
