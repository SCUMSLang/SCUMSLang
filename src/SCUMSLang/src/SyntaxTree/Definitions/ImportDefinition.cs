using System;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class ImportDefinition : Reference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.ImportDefinition;

        public string FilePath { get; }

        public ImportDefinition(string filePath) =>
            FilePath = filePath;

        public override bool Equals(object? obj) =>
            obj is ImportDefinition import && FilePath == import.FilePath;

        public override int GetHashCode() =>
            HashCode.Combine(NodeType, FilePath);

        protected internal override Reference Accept(SyntaxNodeVisitor visitor) =>
            visitor.VisitImportDefinition(this);
    }
}
