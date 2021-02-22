using System;
using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree
{
    [Serializable]
    public class BadBlockScopeException : BlockEvaluatingException
    {
        protected BadBlockScopeException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public BadBlockScopeException() { }

        public BadBlockScopeException(string? message) 
            : base(message) { }

        public BadBlockScopeException(string? message, Exception? innerException) 
            : base(message, innerException) { }
    }
}
