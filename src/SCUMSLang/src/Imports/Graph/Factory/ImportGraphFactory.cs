using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SCUMSLang.SyntaxTree;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.IO;
using SCUMSLang.Compilation;
using SCUMSLang.Imports.Graph.Processors;

namespace SCUMSLang.Imports.Graph.Factory
{
    public class ImportGraphFactory
    {
        public static ImportGraphFactory Default = new ImportGraphFactory();

        public ImportGraphFactorySettings Settings { get; }

        private ModuleDefinition systemModule = new ModuleDefinition().AddSystemTypes();

        public ImportGraphFactory() =>
            Settings = new ImportGraphFactorySettings();

        public ImportGraphFactory(Action<ImportGraphFactorySettings> settingsCallback)
        {
            var options = new ImportGraphFactorySettings();
            settingsCallback?.Invoke(options);
            Settings = options;
        }

        private List<string> GetImportPaths(ImportGraphFactoryParameters parameters)
        {
            static void addImportPath(List<string> importPaths, string importPath)
            {
                if (!PathTools.IsFullPath(importPath)) {
                    importPath = Path.GetFullPath(importPath);
                }

                importPaths.Add(importPath);
            }

            var importPaths = new List<string>();

            foreach (var systemSource in parameters.SystemSources) {
                addImportPath(importPaths, systemSource);
            }

            foreach (var userSource in parameters.UserSources) {
                addImportPath(importPaths, userSource);
            }

            if (!(parameters.ImplicitUInt32PoolSource is null)) {
                addImportPath(importPaths, parameters.ImplicitUInt32PoolSource);
            }

            return importPaths;
        }

        public async Task<ImportGraphFactoryResult> CreateImportGraph(ImportGraphFactoryParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var importPaths = GetImportPaths(parameters);

            ImportGraph importGraph = default;
            List<ImportGraphFactoryError>? compilerErrors = new List<ImportGraphFactoryError>();

            try {
                void ConfigureModuleParameters(ModuleParameters moduleParameters)
                {
                    moduleParameters.LoggerFactory = parameters.LoggerFactory;

                    var referenceResolver = new ReferenceResolverPool();
                    referenceResolver.Add(systemModule.BlockReferenceResolver);
                    moduleParameters.ReferenceResolver = referenceResolver;
                }

                importGraph = (await ImportGraphGenerator.Default.GenerateImportGraphAsync(importPaths, ConfigureModuleParameters))
                    .NextProcess(DirectAcyclicImportGraphParser.Default)
                    .NextProcess(DirectAcyclicImportGraphResolver.Default)
                    .NextProcess(DirectAcyclicImportGraphExpander.Default);
            } catch (Exception error) when (error is IParsingException parsingError) {
                var compilerError = await FilePassageError.CreateFromFilePassageAsync((dynamic)parsingError);
                compilerErrors.Add(compilerError);
            }

            return new ImportGraphFactoryResult(importGraph, compilerErrors);
        }

        public Task<ImportGraphFactoryResult> CompileAsync() =>
            CreateImportGraph(default);
    }
}
