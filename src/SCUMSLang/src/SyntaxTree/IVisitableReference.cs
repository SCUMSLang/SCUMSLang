using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree
{
    public interface IVisitableReference
    {
        void Accept(SyntaxNodeVisitor visitor);
    }
}
