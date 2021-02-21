using System;

namespace SCUMSLang.Compilation
{
    public class CompilerError
    {
        public virtual CompilerErrorType Type { get; } = CompilerErrorType.Error;
        public CompilerErrorSourceType SourceType { get; }
        public string ErrorMessage { get; }
        public Exception WrappedException { get; internal set; } = null!;

        public CompilerError(CompilerErrorSourceType errorType, string errorMessage)
        {
            SourceType = errorType;
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public override string ToString() =>
            $"error[{Enum.GetName(typeof(CompilerErrorSourceType), SourceType)}]: {ErrorMessage}";
    }
}
