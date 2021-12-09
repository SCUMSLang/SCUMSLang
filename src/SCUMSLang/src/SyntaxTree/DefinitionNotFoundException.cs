using System;
using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree
{
    public class DefinitionNotFoundException : BlockEvaluationException
    {
        protected DefinitionNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public DefinitionNotFoundException(string? message)
            : base(message) { }

        public DefinitionNotFoundException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
