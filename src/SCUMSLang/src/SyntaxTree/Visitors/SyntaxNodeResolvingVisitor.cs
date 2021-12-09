using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Visitors
{
    public class SyntaxNodeResolvingVisitor : SyntaxNodeVisitor
    {
        public readonly static SyntaxNodeResolvingVisitor Default = new SyntaxNodeResolvingVisitor();

        private static T Resolve<T>(T member)
            where T : MemberReference
        {
            _ = member.Resolve();
            return member;
        }

        protected internal override Reference VisitTypeReference(TypeReference type) =>
            Resolve(type);

        protected internal override Reference VisitFieldReference(FieldReference field) =>
            Resolve(field);

        protected internal override Reference VisitMethodReference(MethodReference method) =>
            Resolve(method);

        protected internal override Reference VisitEventHandlerReference(EventHandlerReference eventHandler) =>
            Resolve(eventHandler);
    }
}
