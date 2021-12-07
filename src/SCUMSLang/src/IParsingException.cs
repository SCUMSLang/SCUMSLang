namespace SCUMSLang
{
    public interface IParsingException
    {
        string Message { get; }
        int FilePosition { get; }
        byte FilePositionOffset { get; }
        int FilePositionLength { get; }
        int FileLine { get; }
        int FileLinePosition { get; }
        string? FilePath { get; }
    }
}
