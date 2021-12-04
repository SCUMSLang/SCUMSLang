using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree.References
{
    internal interface IOwnableReference
    {
        [NotNull]
        BlockDefinition? ParentBlock { get; set; }

        [MemberNotNullWhen(true, nameof(ParentBlock))]
        bool HasParentBlock { get; }
    }
}
