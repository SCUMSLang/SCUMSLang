using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.Tokenization;

namespace SCUMSLang.Imports
{
    public sealed class Import
    {
        /// <summary>
        /// The full import path.
        /// </summary>
        public ReadOnlyMemory<Token> Tokens { get; }
        public ModuleDefinition Module { get; }
        public string ImportPath => Module.FilePath;
        public int TokenReaderUpperPositionAfterParsingModuleImports { get; }
        public IReadOnlyList<string> ModuleImportPaths => moduleImportPaths ??= GetModulePathImports();

        private List<string>? moduleImportPaths;

        internal Import(ReadOnlyMemory<Token> tokens, ModuleDefinition module, int tokenReaderUpperPositionAfterParsingModuleImports)
        {
            Tokens = tokens;
            Module = module;
            TokenReaderUpperPositionAfterParsingModuleImports = tokenReaderUpperPositionAfterParsingModuleImports;
        }

        private List<string> GetModulePathImports() =>
           Module.Block.BookkeptReferences
               .OfType<ImportDefinition>()
               .Select(import => import.FilePath)
               .ToList();

        public override string ToString() =>
            $"{Path.Combine(new DirectoryInfo(Path.GetDirectoryName(ImportPath) ?? "").Name, Path.GetFileName(ImportPath)) }";
    }
}
