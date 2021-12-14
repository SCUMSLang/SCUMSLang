using System;

namespace SCUMSLang.Imports.Graph
{
    public class ImportGraphFactoryError
    {
        public ImportGraphFactoryErrorSource ErrorSource { get; }
        public string ErrorMessage { get; }
        public Exception Exception { get; internal set; } = null!;

        public ImportGraphFactoryError(ImportGraphFactoryErrorSource errorSource, string errorMessage)
        {
            ErrorSource = errorSource;
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public override string ToString() =>
            $"error[{Enum.GetName(typeof(ImportGraphFactoryErrorSource), ErrorSource)}]: {ErrorMessage}";
    }
}
