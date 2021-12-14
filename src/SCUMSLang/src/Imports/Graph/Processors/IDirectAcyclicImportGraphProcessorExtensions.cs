namespace SCUMSLang.Imports.Graph.Processors
{
    public static class IDirectAcyclicImportGraphProcessorExtensions
    {
        public static ImportGraph NextProcess(this ImportGraph importGraph, IDirectAcyclicImportGraphProcessor processor) =>
            processor.Process(importGraph);
    }
}
