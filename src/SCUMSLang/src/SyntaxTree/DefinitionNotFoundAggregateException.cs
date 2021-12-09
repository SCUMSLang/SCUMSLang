using System;
using System.Collections.Generic;

namespace SCUMSLang.SyntaxTree
{
    public class DefinitionNotFoundAggregateException : AggregateException
    {
        public DefinitionNotFoundAggregateException() : base()
        {
        }

        public DefinitionNotFoundAggregateException(IEnumerable<Exception> innerExceptions) : base(innerExceptions)
        {
        }

        public DefinitionNotFoundAggregateException(params Exception[] innerExceptions) : base(innerExceptions)
        {
        }

        public DefinitionNotFoundAggregateException(string? message) : base(message)
        {
        }

        public DefinitionNotFoundAggregateException(string? message, IEnumerable<Exception> innerExceptions) : base(message, innerExceptions)
        {
        }

        public DefinitionNotFoundAggregateException(string? message, Exception innerException) : base(message, innerException)
        {
        }

        public DefinitionNotFoundAggregateException(string? message, params Exception[] innerExceptions) : base(message, innerExceptions)
        {
        }
    }
}

