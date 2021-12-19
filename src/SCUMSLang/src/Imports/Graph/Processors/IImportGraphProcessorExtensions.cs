namespace SCUMSLang.Imports.Graph.Processors
{
    public static class IImportGraphProcessorExtensions
    {
        public static ImportGraph NextProcess(this ImportGraph importGraph, IImportGraphProcessor processor) =>
            processor.Process(importGraph);
    }
}
