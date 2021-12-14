using System;
using System.Collections.Generic;
using System.Linq;
using SCUMSLang.SyntaxTree;

namespace SCUMSLang.Imports.Graph.Processors
{
    public class DirectAcyclicImportGraphResolver : IDirectAcyclicImportGraphProcessor
    {
        public readonly static DirectAcyclicImportGraphResolver Default = new DirectAcyclicImportGraphResolver();

        private Import GetImportByPath(IEnumerable<Import> imports, string importPath)
        {
            var import = imports.SingleOrDefault(x => x.ImportPath == importPath);

            if (import is null) {
                throw SyntaxTreeThrowHelper.ModuleNotFound(importPath, stackTrace: Environment.StackTrace);
            }

            return import;
        }

        public ImportGraph ResolveModules(ImportGraph importGraph)
        {
            if (importGraph.State != ImportGraphState.Parsed) {
                throw new InvalidOperationException("The import graph need to be parsed");
            }

            foreach (var import in importGraph.TopologizedImports) {
                import.Module.ResolveOnce(importPath => GetImportByPath(importGraph.TopologizedImports, importPath).Module);
            }

            return importGraph.WithState(ImportGraphState.Resolved);
        }

        ImportGraph IDirectAcyclicImportGraphProcessor.Process(ImportGraph importGraph) =>
            ResolveModules(importGraph);
    }
}
