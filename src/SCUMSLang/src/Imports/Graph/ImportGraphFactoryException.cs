using System;

namespace SCUMSLang.Imports.Graph
{
    public class ImportGraphFactoryException : Exception
    {
        public ImportGraphFactoryException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
