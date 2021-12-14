using System.Collections.Generic;

namespace SCUMSLang.Imports.Graph.Factory
{
    public class ImportGraphFactoryResult
    {
        public ImportGraph? ImportGraph { get; }
        public IReadOnlyList<ImportGraphFactoryError> Errors { get; }

        public bool HasErrors =>
            Errors.Count > 0;

        public ImportGraphFactoryResult(ImportGraph importGraph, IReadOnlyList<ImportGraphFactoryError>? errors)
        {
            ImportGraph = importGraph;
            Errors = errors ?? new ImportGraphFactoryError[0];
        }
    }
}
