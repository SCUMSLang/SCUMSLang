using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public class SyntaxNodeModuleFillingVisitor : SyntaxNodeVisitor
    {
        [return: NotNullIfNotNull("reference")]
        public static Reference? Visit(Reference? reference, ModuleDefinition? module) =>
            new SyntaxNodeModuleFillingVisitor(module).Visit(reference);

        [return: NotNullIfNotNull("reference")]
        public static T? VisitAndConvert<T>(T? reference, ModuleDefinition? module)
            where T : Reference =>
            new SyntaxNodeModuleFillingVisitor(module).VisitAndConvert(reference);

        private readonly ModuleDefinition? module;

        public SyntaxNodeModuleFillingVisitor(ModuleDefinition? module) =>
            this.module = module;

        [return: NotNullIfNotNull("reference")]
        public override Reference? Visit(Reference? reference)
        {
            if (reference is IOwnableReference ownableReference && !ownableReference.HasModule) {
                ownableReference.Module = module;
            }

            return base.Visit(reference);
        }
    }
}
