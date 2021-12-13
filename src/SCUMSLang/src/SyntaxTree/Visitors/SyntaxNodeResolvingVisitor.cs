using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public class SyntaxNodeResolvingVisitor : SyntaxNodeVisitor
    {
        private readonly ImportResolver importResolver;

        public SyntaxNodeResolvingVisitor(ImportResolver importResolver) =>
            this.importResolver = importResolver ?? throw new System.ArgumentNullException(nameof(importResolver));

        protected internal override Reference VisitTypeReference(TypeReference type)
        {
            _ = type.Resolve();
            return base.VisitTypeReference(type);
        }

        protected internal override Reference VisitFieldReference(FieldReference field)
        {
            _ = field.Resolve();
            return base.VisitFieldReference(field);
        }

        protected internal override Reference VisitMethodReference(MethodReference method)
        {
            _ = method.Resolve();
            return base.VisitMethodReference(method);
        }

        protected internal override Reference VisitEventHandlerReference(EventHandlerReference eventHandler)
        {
            _ = eventHandler.Resolve();
            return base.VisitEventHandlerReference(eventHandler);
        }

        internal override Reference VisitModuleBlockDefinition(ModuleBlockDefinition moduleBlock)
        {
            moduleBlock.ResolveOnce(importResolver);
            return base.VisitModuleBlockDefinition(moduleBlock);
        }
    }
}
