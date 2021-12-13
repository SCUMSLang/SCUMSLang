using Microsoft.Extensions.Logging;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class ModuleParameters
    {
        public IReferenceResolver? ReferenceResolver { get; set; }
        public string? FilePath { get; set; }
        public ILoggerFactory? LoggerFactory { get; set; }
    }
}
