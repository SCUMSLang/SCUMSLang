using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public class SyntaxNodeResolvingVisitor : SyntaxNodeVisitor
    {
        protected internal override Reference VisitTypeReference(TypeReference type) =>
            type.Resolve();

        protected internal override Reference VisitFieldReference(FieldReference field) =>
            field.Resolve();

        protected internal override Reference VisitMethodReference(MethodReference method) =>
            method.Resolve();

        protected internal override Reference VisitEventHandlerReference(EventHandlerReference eventHandler) =>
            eventHandler.Resolve();
    }
}
