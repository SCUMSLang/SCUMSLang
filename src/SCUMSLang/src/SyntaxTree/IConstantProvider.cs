namespace SCUMSLang.SyntaxTree
{
    public interface IConstantProvider
    {
        bool HasConstant { get; }
        object Constant { get; }
    }
}
