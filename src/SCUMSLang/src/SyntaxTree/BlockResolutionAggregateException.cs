using System;
using System.Collections.Generic;

namespace SCUMSLang.SyntaxTree
{
    public class BlockResolutionAggregateException : AggregateException
    {
        public override string Message { get; }
        public override string? StackTrace => stackTrace ?? base.StackTrace;

        private string? stackTrace;

        public BlockResolutionAggregateException() =>
            Message = base.Message;

        public BlockResolutionAggregateException(IEnumerable<Exception> innerExceptions) : base(innerExceptions) =>
            Message = base.Message;

        public BlockResolutionAggregateException(params Exception[] innerExceptions) : base(innerExceptions) =>
            Message = base.Message;

        public BlockResolutionAggregateException(string? message) =>
            Message = message ?? base.Message;

        public BlockResolutionAggregateException(string? message, IEnumerable<Exception> innerExceptions) : base(innerExceptions) =>
            Message = message ?? base.Message;

        public BlockResolutionAggregateException(string? message, Exception innerException) : base(innerException) =>
            Message = message ?? base.Message;

        public BlockResolutionAggregateException(string? message, params Exception[] innerExceptions) : base(innerExceptions) =>
            Message = message ?? base.Message;

        internal void SetStackTrace(string? stackTrace) =>
            this.stackTrace = stackTrace;
    }
}

