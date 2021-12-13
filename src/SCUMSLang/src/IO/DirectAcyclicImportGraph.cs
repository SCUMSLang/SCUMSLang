using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCUMSLang.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.IO
{
    public class DirectAcyclicImportGraph
    {
        public static async Task<DirectAcyclicImportGraph> GenerateImportGraphAsync(
            IEnumerable<string> initialImportPaths,
            Action<ModuleParameters>? moduleParametersConfigurer = null)
        {
            var importGraph = new DirectAcyclicImportGraph(
                initialImportPaths,
                moduleParametersConfigurer);

            await importGraph.LoadImportsRecursivelyAsync();
            return importGraph;
        }

        public IReadOnlyList<Import> TopologizedImports => topologizedImports;

        private List<Import> topologizedImports;
        private HashSet<string> initialImportPaths;
        private readonly Action<ModuleParameters>? moduleParametersConfigurer;

        private DirectAcyclicImportGraph(
            IEnumerable<string> initialImportPaths,
            Action<ModuleParameters>? moduleParametersConfigurer = null)
        {
            topologizedImports = new List<Import>();
            this.initialImportPaths = new HashSet<string>(initialImportPaths);
            this.moduleParametersConfigurer = moduleParametersConfigurer;
        }

        protected async Task LoadImportsRecursivelyAsync()
        {
            var unloadedImportPaths = initialImportPaths;
            var loadedImports = new Dictionary<string, Import>();

            do {
                var contextualImports = new List<Import>();

                foreach (var unloadedImportPath in unloadedImportPaths) {
                    var import = await Import.ParseImportsAsync(
                        unloadedImportPath,
                        moduleParametersConfigurer);

                    contextualImports.Add(import);
                    loadedImports.Add(unloadedImportPath, import);
                }

                unloadedImportPaths.Clear();

                foreach (var contextualImport in contextualImports) {
                    foreach (var directImportPath in contextualImport.DirectImportPaths) {
                        if (loadedImports.ContainsKey(directImportPath)) {
                            continue;
                        }

                        unloadedImportPaths.Add(directImportPath);
                    }
                }
            } while (unloadedImportPaths.Count > 0);

            var importEdges = loadedImports.Values
                .SelectMany(x => x.DirectImportPaths
                    .Select(y => (x, loadedImports[y])))
                .ToList();

            topologizedImports.Clear();

            topologizedImports = TopologicalSorting.KahnAlgorithm.SortTopologically(
                loadedImports.Values,
                importEdges,
                ImportOnlyPathEqualityComparer.Default);
        }
    }
}
