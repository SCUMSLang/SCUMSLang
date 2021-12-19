using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.Tokenization;

namespace SCUMSLang.Imports.Graph.Processors
{
    public class ParsingProcessor : IImportGraphProcessor
    {
        public readonly static ParsingProcessor Default = new ParsingProcessor();

        private void ParseModule(Import import)
        {
            var parser = new SyntaxTreeParser(options => {
                options.Module = import.Module;
                options.TokenReaderStartPosition = import.TokenReaderUpperPositionAfterParsingModuleImports + 1;
                options.TokenReaderBehaviour.SetSkipConditionForNonParserChannelTokens();
            });

            parser.Parse(import.Tokens.Span);
        }

        public ImportGraph ParseModules(ImportGraph importGraph)
        {
            foreach (var import in importGraph.TopologizedImports) {
                ParseModule(import);
            }

            return importGraph.WithState(ImportGraphState.Parsed);
        }

        ImportGraph IImportGraphProcessor.Process(ImportGraph importGraph) =>
            ParseModules(importGraph);
    }
}
