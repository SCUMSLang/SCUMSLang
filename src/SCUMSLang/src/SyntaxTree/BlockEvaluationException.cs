﻿using System;
using System.Runtime.Serialization;

namespace SCUMSLang.SyntaxTree
{
    public class BlockEvaluationException : Exception, IFilePositionable
    {
        public IFilePosition? FilePosition { get; init; }

        protected BlockEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public BlockEvaluationException() { }

        public BlockEvaluationException(string? message)
            : base(message) { }

        public BlockEvaluationException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
