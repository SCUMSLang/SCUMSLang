using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.References
{
    public interface IVisitableReference
    {
        void Accept(SyntaxNodeVisitor visitor);
    }
}
