namespace SCUMSLang
{
    public interface IFilePosition
    {
        int FilePosition { get; }
        byte FilePositionOffset { get; }
        int FilePositionLength { get; }
        int FileLine { get; }
        int FileLinePosition { get; }
        string? FilePath { get; }
    }
}
