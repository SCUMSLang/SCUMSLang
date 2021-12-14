using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.Tokenization;
using Teronis.IO.FileLocking;

namespace SCUMSLang.Imports
{
    public class ImportFactory
    {
        public readonly static ImportFactory Default = new ImportFactory();

        private async Task<ReadOnlyMemory<Token>> TokenizeAsync(string importPath, CancellationToken cancellationToken = default)
        {
            using var fileStream = FileStreamLocker.Default.WaitUntilAcquired(importPath,
                3 * 1000,
                fileMode: FileMode.Open,
                fileAccess: FileAccess.Read,
                fileShare: FileShare.Read,
                cancellationToken: cancellationToken)!;

            return await Tokenizer.TokenizeAsync(fileStream);
        }

        private int ParseImports(ModuleDefinition module, ReadOnlyMemory<Token> tokens)
        {
            var parser = new SyntaxTreeParser(options => {
                options.Module = module;
                options.RecognizableNodes = RecognizableReferences.Import;
                options.EmptyRecognizationResultsIntoReturn = true;
                options.TokenReaderBehaviour.SetSkipConditionForNonParserChannelTokens();
            });

            var result = parser.Parse(tokens.Span);
            return result.TokenReaderUpperPosition;
        }

        private ModuleDefinition CreateModule(string filePath, Action<ModuleParameters>? moduleParametersConfigurer)
        {
            var moduleParameters = new ModuleParameters();
            moduleParametersConfigurer?.Invoke(moduleParameters);
            moduleParameters.FilePath = filePath;
            return new ModuleDefinition(moduleParameters);
        }

        public async Task<Import> CreateImportAsync(string filePath, Action<ModuleParameters>? moduleParametersConfigurer = null, CancellationToken cancellationToken = default)
        {
            var module = CreateModule(filePath, moduleParametersConfigurer);
            var tokens = await TokenizeAsync(filePath, cancellationToken);
            var tokenReaderUpperPositionAfterParsingModuleImports = ParseImports(module, tokens);
            return new Import(tokens, module, tokenReaderUpperPositionAfterParsingModuleImports);
        }
    }
}
