using System;
using System.Text;
using System.Threading.Tasks;
using SCUMSLang.Imports.Graph;

namespace SCUMSLang.Compilation
{
    public class FilePassageError : ImportGraphFactoryError
    {
        public static async Task<ImportGraphFactoryError> CreateFromFilePassageAsync<T>(T error)
            where T : Exception, IParsingException
        {
            var errorSource = ImportGraphFactoryErrorSource.SyntaxTree;
            var errorMessage = error.Message;
            var errorFilePosition = error.FilePosition;
            ImportGraphFactoryError compilerError;

            if (errorFilePosition?.FilePath is null) {
                compilerError = new ImportGraphFactoryError(errorSource, errorMessage);
            } else {
                var filePath = errorFilePosition.FilePath;
                var filePosition = errorFilePosition.FilePosition;
                var filePositionLength = errorFilePosition.FilePositionLength;
                var fileLine = errorFilePosition.FileLine;

                string? filePassage;

                var filePassageLines = await FilePassageReader.Default.ReadPassageAsync(
                    filePath,
                    filePosition + errorFilePosition.FilePositionOffset,
                    filePositionLength);

                if (!(filePassageLines is null)) {
                    filePassage = "| " + string.Join($"{Environment.NewLine}| ", filePassageLines);
                } else {
                    filePassage = null;
                }

                compilerError = new FilePassageError(
                    errorSource,
                    errorMessage,
                    filePassage,
                    filePath,
                    position: filePosition,
                    positionLength: filePositionLength,
                    line: fileLine,
                    linePosition: errorFilePosition.FileLinePosition);
            }

            compilerError.Exception = new ImportGraphFactoryException(compilerError.ToString(), error);
            return compilerError;
        }

        public string? FilePassage { get; set; }
        public string? FilePath { get; }
        public int Position { get; }
        public int PositionLength { get; }
        public int Line { get; }
        public int LinePosition { get; }

        public FilePassageError(
            ImportGraphFactoryErrorSource errorType,
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
                stringBuilder.Append(FilePassage);
            }

            return stringBuilder.ToString();
        }
    }
}
