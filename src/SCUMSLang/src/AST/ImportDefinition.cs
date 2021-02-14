using System;

namespace SCUMSLang.AST
{
    public class ImportDefinition : Reference
    {
        public override TreeTokenType TokenType => TreeTokenType.ImportDefinition;

        public string FilePath { get; }

        public ImportDefinition(string filePath) =>
            FilePath = filePath;

        public override bool Equals(object? obj) =>
            obj is ImportDefinition import && FilePath == import.FilePath;

        public override int GetHashCode() =>
            HashCode.Combine(TokenType, FilePath);
    }
}
