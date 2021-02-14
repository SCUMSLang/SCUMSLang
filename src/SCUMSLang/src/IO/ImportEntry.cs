using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SCUMSLang.AST;
using SCUMSLang.Tokenization;
using Teronis.IO.FileLocking;

namespace SCUMSLang.IO
{
    public class ImportEntry
    {
        public static async Task<ImportEntry> CreateAsync(
            string importPath,
            ReferencePool nameReservableNodePool)
        {
            var importEntry = new ImportEntry(
                importPath,
                nameReservableNodePool);

            await importEntry.LoadTokensAsync();
            return importEntry;
        }

        /// <summary>
        /// The full import path.
        /// </summary>
        public string ImportPath { get; }
        public string ImportDirectory { get; }
        public IReadOnlyList<Token> Tokens => tokens;
        public ModuleDefinition Module { get; private set; }
        public int TokenReaderUpperPosition { get; private set; }

        private Token[] tokens = null!;

        public IReadOnlyList<string> DirectImportPaths =>
            directImportPaths;

        private List<string> directImportPaths;

        protected ImportEntry(string filePath, ReferencePool nameReservableNodePool)
        {
            directImportPaths = new List<string>();
            filePath = filePath ?? throw new System.ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath)) {
                throw new IOException($"File '{filePath}' does not exist.");
            }

            ImportPath = filePath;

            var moduleParameters = new ModuleParameters() {
                MemberReferences = nameReservableNodePool,
                FilePath = filePath
            };

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
            this.tokens = tokens.ToArray();
        }

        public ReadOnlySpan<Token> TokensAsReadOnlySpan() =>
            tokens.AsSpan();

        public void LoadDirectImports()
        {
            var parser = new TreeParser(options => {
                options.Module = Module;
                options.RecognizableNodes = RecognizableReferences.Import;
                options.EmptyRecognizationResultsIntoWhileBreak = true;
                options.TokenReaderBehaviour.SetNonParserChannelTokenSkipCondition();
            });

            var result = parser.Parse(tokens);
            TokenReaderUpperPosition = result.TokenReaderUpperPosition;

            directImportPaths = Module.Block.ReferenceRecords
                .OfType<ImportDefinition>()
                .Select(import => import.FilePath)
                .ToList();
        }

        public override string ToString() =>
            $"{Path.Combine(new DirectoryInfo(Path.GetDirectoryName(ImportPath) ?? "").Name, Path.GetFileName(ImportPath)) }";
    }
}
