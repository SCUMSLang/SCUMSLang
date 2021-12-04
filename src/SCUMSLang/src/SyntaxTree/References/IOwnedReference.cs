using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree.References
{
    public interface IOwnedReference
    {
        ModuleDefinition Module { get; }
    }
}
