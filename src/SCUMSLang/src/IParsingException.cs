namespace SCUMSLang
{
    public interface IParsingException : IFilePositionable
    {
        string Message { get; }
    }
}
