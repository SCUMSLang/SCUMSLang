using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
{
    internal interface IOwnableReference
    {
        [NotNull]
        ModuleDefinition? Module { get; set; }

        [MemberNotNullWhen(true, nameof(Module))]
        bool HasModule { get; }
    }
}
