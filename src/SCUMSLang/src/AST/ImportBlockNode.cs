using System;

namespace SCUMSLang.AST
{
    public class ImportNode : Node
    {
        public override NodeType NodeType => NodeType.Import;

        public string FilePath { get; }

        public ImportNode(string filePath) =>
            FilePath = filePath;

        public override bool Equals(object? obj) =>
            obj is ImportNode import && FilePath == import.FilePath;

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, FilePath);
    }
}
