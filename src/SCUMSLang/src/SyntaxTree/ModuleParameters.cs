using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree
{
    public class ModuleParameters
    {
        public IReferenceResolver? ReferenceResolver { get; set; }
        public string? FilePath { get; set; }
    }
}
