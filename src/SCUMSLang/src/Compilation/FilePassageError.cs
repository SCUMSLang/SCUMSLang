using System;
using System.Text;
using System.Threading.Tasks;
using SCUMSLang.IO;

namespace SCUMSLang.Compilation
{
    public class FilePassageError : CompilerError
    {
        public static async Task<CompilerError> CreateFromFilePassageAsync(SyntaxTreeParsingException error)
        {
            var compilerErrorType = CompilerErrorSourceType.SyntaxTree;
            var errorMessage = error.Message;
            CompilerError compilerError;

            if (error.FilePath is null) {
                compilerError = new CompilerError(compilerErrorType, errorMessage);
            } else {
                var filePath = error.FilePath;
                var filePosition = error.FilePosition;
                var filePositionLength = error.FilePositionLength;
                var fileLine = error.FileLine;

                string? filePassage;
                var filePassageLines = await FilePassageReader.Default.ReadPassageAsync(filePath, filePosition, filePositionLength);

                if (!(filePassageLines is null)) {
                    filePassage = "| " + string.Join($"{Environment.NewLine}| ", filePassageLines);
                } else {
                    filePassage = null;
                }

                compilerError = new FilePassageError(
                    compilerErrorType,
                    errorMessage,
                    filePassage,
                    filePath,
                    position: filePosition,
                    positionLength: filePositionLength,
                    line: fileLine,
                    linePosition: error.FileLinePosition);
            }

            var wrappedException = new CompilerException(compilerError.ToString(), error);
            compilerError.WrappedException = wrappedException;
            return compilerError;
        }

        public override CompilerErrorType Type =>
            CompilerErrorType.FilePassageError;

        public string? FilePassage { get; set; }
        public string? FilePath { get; }
        public int Position { get; }
        public int PositionLength { get; }
        public int Line { get; }
        public int LinePosition { get; private set; }

        public FilePassageError(
            CompilerErrorSourceType errorType,
            string errorMessage,
            string? filePassage,
            string filePath,
            int position,
            int positionLength,
            int line,
            int linePosition)
            : base(errorType, errorMessage)
        {
            FilePassage = filePassage;
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Position = position;
            PositionLength = positionLength;
            Line = line;
            LinePosition = linePosition;
        }

        public override string ToString()
        {
            if (FilePath is null) {
                return base.ToString();
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{base.ToString()}{Environment.NewLine}");
            stringBuilder.Append($"--> {FilePath}:{Line}:{LinePosition}{Environment.NewLine}");

            if (!(FilePassage is null)) {
                stringBuilder.Append($"| {Environment.NewLine}");
                stringBuilder.Append(FilePassage);
            }

            return stringBuilder.ToString();
        }
    }
}
