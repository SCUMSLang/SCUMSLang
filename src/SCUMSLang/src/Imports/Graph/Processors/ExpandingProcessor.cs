using System;

namespace SCUMSLang.Imports.Graph.Processors
{
    public class ExpandingProcessor : IImportGraphProcessor
    {
        public readonly static ExpandingProcessor Default = new ExpandingProcessor();

        public ImportGraph ExpandModules(ImportGraph importGraph)
        {
            if (importGraph.State != ImportGraphState.Resolved) {
                throw new InvalidOperationException("The import graph need to be resolved");
            }

            return importGraph.WithState(ImportGraphState.Expanded);
        }

        ImportGraph IImportGraphProcessor.Process(ImportGraph importGraph) =>
            ExpandModules(importGraph);
    }
}
