using System;
using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree
{
    public class BlockResolutionException : BlockEvaluationException
    {
        protected BlockResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public BlockResolutionException(string? message)
            : base(message) { }

        public BlockResolutionException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
