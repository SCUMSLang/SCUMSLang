using SCUMSLang.IO;

namespace SCUMSLang.Compilation
{
    public class CompilerResult
    {
        public DirectAcyclicImportGraph ImportGraph { get; }

        public CompilerResult(DirectAcyclicImportGraph importGraph) =>
            ImportGraph = importGraph;
    }
}
