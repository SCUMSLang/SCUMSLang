namespace SCUMSLang.AST
{
    public interface IConstantProvider
    {
        bool HasConstant { get; }
        object Constant { get; }
    }
}
