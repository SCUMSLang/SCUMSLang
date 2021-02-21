using System;

namespace SCUMSLang.Compilation
{
    public class CompilerException : Exception
    {
        public CompilerException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
