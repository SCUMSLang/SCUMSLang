using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    internal static class IOwnableReferenceExtensions
    {
        [return: NotNullIfNotNull("ownableReference")]
        public static T? WithModule<T>(this T? ownableReference, ModuleDefinition? module)
            where T : IOwnableReference
        {
            if (ownableReference is not null) {
                ownableReference.Module = module;
            }

            return ownableReference;
        }

        [return: NotNullIfNotNull("reference")]
        public static T? WithModuleRecursively<T>(this T? reference, ModuleDefinition? module)
            where T : Reference
        {
            if (reference is not null) {
                return SyntaxNodeModuleFillingVisitor.VisitAndConvert(reference, module);
            }

            return reference;
        }
    }
}
