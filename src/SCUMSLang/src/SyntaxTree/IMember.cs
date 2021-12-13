namespace SCUMSLang.SyntaxTree
{
    public interface IMember
    {
        string Name { get; }

        IMember Resolve();
    }
}
