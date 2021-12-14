using System;

namespace SCUMSLang.Imports.Graph.Processors
{
    public class DirectAcyclicImportGraphExpander : IDirectAcyclicImportGraphProcessor
    {
        public readonly static DirectAcyclicImportGraphExpander Default = new DirectAcyclicImportGraphExpander();

        public ImportGraph ExpandModules(ImportGraph importGraph)
        {
            if (importGraph.State != ImportGraphState.Resolved) {
                throw new InvalidOperationException("The import graph need to be resolved");
            }

            return importGraph.WithState(ImportGraphState.Expanded);
        }

        ImportGraph IDirectAcyclicImportGraphProcessor.Process(ImportGraph importGraph) =>
            ExpandModules(importGraph);
    }
}
