using System;

namespace SCUMSLang.Compilation
{
    public class CompilerError
    {
        public CompilerErrorSource ErrorSource { get; }
        public string ErrorMessage { get; }
        public Exception WrappedException { get; internal set; } = null!;

        public CompilerError(CompilerErrorSource errorSource, string errorMessage)
        {
            ErrorSource = errorSource;
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public override string ToString() =>
            $"error[{Enum.GetName(typeof(CompilerErrorSource), ErrorSource)}]: {ErrorMessage}";
    }
}
