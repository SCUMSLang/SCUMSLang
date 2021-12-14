using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCUMSLang.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.Imports.Graph
{
    public sealed class ImportGraphGenerator
    {
        public readonly static ImportGraphGenerator Default = new ImportGraphGenerator();

        private IEnumerable<(Import, Import)> CalculateImportEdges(Dictionary<string, Import> loadedImports) =>
            loadedImports.Values
                .SelectMany(x => x.ModuleImportPaths
                    .Select(y => (x, loadedImports[y])));

        private List<Import> SortImportsTopologically(IEnumerable<Import> loadedImports, IEnumerable<(Import, Import)> importEdges) =>
            TopologicalSorting.KahnAlgorithm.SortTopologically(
                loadedImports,
                importEdges,
                ImportOnlyPathEqualityComparer.Default);

        public async Task<ImportGraph> GenerateImportGraphAsync(
            HashSet<string> initialImportPaths,
            Action<ModuleParameters>? moduleParametersConfigurer = null)
        {
            var unloadedImportPaths = initialImportPaths;
            var allLoadedImports = new Dictionary<string, Import>();

            do {
                var doBlockLoadedImports = new List<Import>();

                foreach (var importPath in unloadedImportPaths) {
                    var import = await ImportFactory.Default.CreateImportAsync(
                        importPath,
                        moduleParametersConfigurer);

                    doBlockLoadedImports.Add(import);
                    allLoadedImports.Add(importPath, import);
                }

                unloadedImportPaths.Clear();

                foreach (var import in doBlockLoadedImports) {
                    foreach (var moduleImportPaths in import.ModuleImportPaths) {
                        if (allLoadedImports.ContainsKey(moduleImportPaths)) {
                            continue;
                        }

                        unloadedImportPaths.Add(moduleImportPaths);
                    }
                }
            } while (unloadedImportPaths.Count > 0);

            var importEdges = CalculateImportEdges(allLoadedImports);
            var topologizedImports = SortImportsTopologically(allLoadedImports.Values, importEdges);
            return new ImportGraph(topologizedImports);
        }

        public Task<ImportGraph> GenerateImportGraphAsync(
            IEnumerable<string> initialImportPaths,
            Action<ModuleParameters>? moduleParametersConfigurer = null) =>
            GenerateImportGraphAsync(new HashSet<string>(initialImportPaths), moduleParametersConfigurer);
    }
}
