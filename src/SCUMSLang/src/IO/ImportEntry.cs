using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SCUMSLang.SyntaxTree;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.Tokenization;
using Teronis.IO.FileLocking;

namespace SCUMSLang.IO
{
    public class ImportEntry
    {
        public static async Task<ImportEntry> LoadDirectImportsAsync(
            string importPath,
            Action<ModuleParameters>? moduleParametersConfigurer = null)
        {
            var importEntry = new ImportEntry(
                importPath,
                moduleParametersConfigurer);

            await importEntry.LoadTokensAsync();
            importEntry.parseDirectImports();
            return importEntry;
        }

        /// <summary>
        /// The full import path.
        /// </summary>
        public string ImportPath { get; }
        public ReadOnlyMemory<Token> Tokens => tokens;
        public ModuleDefinition Module { get; }
        public int TokenReaderUpperPosition { get; private set; }

        private ReadOnlyMemory<Token> tokens = null!;

        public IReadOnlyList<string> DirectImportPaths =>
            directImportPaths;

        private List<string> directImportPaths;

        protected ImportEntry(string filePath, Action<ModuleParameters>? moduleParametersConfigurer = null)
        {
            directImportPaths = new List<string>();
            filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath)) {
                throw new IOException($"File '{filePath}' does not exist.");
            }

            ImportPath = filePath;

            var moduleParameters = new ModuleParameters();
            moduleParametersConfigurer?.Invoke(moduleParameters);
            moduleParameters.FilePath = filePath;
            Module = new ModuleDefinition(moduleParameters);
        }

        protected async Task LoadTokensAsync(CancellationToken cancellationToken = default)
        {
            using var fileStream = FileStreamLocker.Default.WaitUntilAcquired(ImportPath,
                3 * 1000,
                fileMode: FileMode.Open,
                fileAccess: FileAccess.Read,
                fileShare: FileShare.Read,
                cancellationToken: cancellationToken)!;

            var tokens = await Tokenizer.TokenizeAsync(fileStream);
            this.tokens = tokens;
        }

        private void parseDirectImports()
        {
            var parser = new SyntaxTreeParser(options => {
                options.Module = Module;
                options.RecognizableNodes = RecognizableReferences.Import;
                options.EmptyRecognizationResultsIntoWhileBreak = true;
                options.TokenReaderBehaviour.SetSkipConditionForNonParserChannelTokens();
            });

            var result = parser.Parse(tokens.Span);
            TokenReaderUpperPosition = result.TokenReaderUpperPosition;

            directImportPaths = Module.Block.BookkeptReferences
                .OfType<ImportDefinition>()
                .Select(import => import.FilePath)
                .ToList();
        }

        public override string ToString() =>
            $"{Path.Combine(new DirectoryInfo(Path.GetDirectoryName(ImportPath) ?? "").Name, Path.GetFileName(ImportPath)) }";

        public void ReadModule()
        {
            var parser = new SyntaxTreeParser(options => {
                options.Module = Module;
                options.TokenReaderStartPosition = TokenReaderUpperPosition + 1;
                options.TokenReaderBehaviour.SetSkipConditionForNonParserChannelTokens();
            });

            parser.Parse(Tokens.Span);
        }

        public void ResolveModule() =>
            Module.ResolveRecursively();
    }
}
