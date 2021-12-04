using System.Collections.Generic;
using SCUMSLang.IO;

namespace SCUMSLang.Compilation
{
    public class CompilerResult
    {
        public DirectAcyclicImportGraph? ImportGraph { get; }
        public IReadOnlyList<CompilerError> Errors { get; }

        public bool HasErrors =>
            Errors.Count > 0;

        public CompilerResult(DirectAcyclicImportGraph? importGraph, IReadOnlyList<CompilerError>? errors)
        {
            ImportGraph = importGraph;
            Errors = errors ?? new CompilerError[0];
        }
    }
}
