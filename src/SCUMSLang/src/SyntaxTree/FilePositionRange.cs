using System;

namespace SCUMSLang.SyntaxTree
{
    public sealed class FilePositionRange : IFilePosition
    {
        public static FilePositionRange Of(IFilePosition token)
        {
            return new FilePositionRange(
                token.FilePosition,
                token.FilePositionOffset,
                token.FilePositionLength,
                token.FileLine,
                token.FileLinePosition,
                token.FilePath);
        }

        public static FilePositionRange Of(IFilePosition begin, IFilePosition end)
        {
            if (begin.FilePositionOffset != end.FilePositionOffset) {
                throw new ArgumentException("The file position offset must be equal");
            }

            if (begin.FilePath != end.FilePath) {
                throw new ArgumentException("The file path must be equal");
            }

            return new FilePositionRange(
                begin.FilePosition,
                begin.FilePositionOffset,
                end.FilePosition + end.FilePositionLength - begin.FilePosition,
                begin.FileLine,
                begin.FileLinePosition,
                begin.FilePath);
        }

        private FilePositionRange(int filePosition, byte filePositionOffset, int filePositionLength, int fileLine, int fileLinePosition, string? filePath)
        {
            FilePosition = filePosition;
            FilePositionOffset = filePositionOffset;
            FilePositionLength = filePositionLength;
            FileLine = fileLine;
            FileLinePosition = fileLinePosition;
            FilePath = filePath;
        }

        public int FilePosition { get; }
        public byte FilePositionOffset { get; }
        public int FilePositionLength { get; }
        public int FileLine { get; }
        public int FileLinePosition { get; }
        public string? FilePath { get; }
    }
}
