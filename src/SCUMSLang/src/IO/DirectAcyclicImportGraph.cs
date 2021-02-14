using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCUMSLang.AST;
using SCUMSLang.Collections.Generic;

namespace SCUMSLang.IO
{
    public class DirectAcyclicImportGraph
    {
        public static async Task<DirectAcyclicImportGraph> CreateAsync(
            IEnumerable<string> initialImportPaths,
            Action<ModuleParameters>? configureModuleParameters = null) {
            var importGraph = new DirectAcyclicImportGraph(
                initialImportPaths,
                configureModuleParameters);

            await importGraph.LoadImportsRecursivelyAsync();
            return importGraph;
        }

        public IReadOnlyList<ImportEntry> SortedImports => sortedImports;

        private List<ImportEntry> sortedImports;
        private HashSet<string> initialImportPaths;
        private readonly Action<ModuleParameters>? configureModuleParameters;

        public DirectAcyclicImportGraph(
            IEnumerable<string> initialImportPaths,
            Action<ModuleParameters>? configureModuleParameters = null)
        {
            sortedImports = new List<ImportEntry>();
            this.initialImportPaths = new HashSet<string>(initialImportPaths);
            this.configureModuleParameters = configureModuleParameters;
        }

        protected async Task LoadImportsRecursivelyAsync()
        {
            var unloadedImportPaths = initialImportPaths;
            var loadedImports = new Dictionary<string, ImportEntry>();

            do {
                var contextualImports = new List<ImportEntry>();

                foreach (var unloadedImportPath in unloadedImportPaths) {
                    var import = await ImportEntry.CreateAsync(
                        unloadedImportPath,
                        configureModuleParameters);

                    import.LoadDirectImports();
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

            sortedImports.Clear();
            sortedImports = TopologicalSorting.KahnAlgorithm.SortTopologically(loadedImports.Values, importEdges, ImportEntryOnlyPathEqualityComparer.Default);
        }
    }
}
