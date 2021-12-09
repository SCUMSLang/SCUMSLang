using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCUMSLang.IO;
using System.IO;
using SCUMSLang.SyntaxTree;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.Compilation
{
    public class Compiler
    {
        public static Compiler Default = new Compiler();

        public CompilerSettings Settings { get; }

        private ModuleDefinition systemModule = new ModuleDefinition().AddSystemTypes();

        public Compiler() =>
            Settings = new CompilerSettings();

        public Compiler(Action<CompilerSettings> settingsCallback)
        {
            var options = new CompilerSettings();
            settingsCallback?.Invoke(options);
            Settings = options;
        }

        private List<string> GetImportPaths(CompilerParameters parameters)
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

        public void ConfigureModuleParameters(ModuleParameters moduleParameters)
        {
            var referenceResolver = new ReferenceResolverPool();
            referenceResolver.Add(systemModule.BlockReferenceResolver);
            moduleParameters.ReferenceResolver = referenceResolver;
        }

        public async Task<CompilerResult> CompileAsync(Action<CompilerParameters>? parametersCallback)
        {
            var compilerParameters = new CompilerParameters();
            parametersCallback?.Invoke(compilerParameters);
            var importPaths = GetImportPaths(compilerParameters);

            // TODO: !?!?!!??
            //var systemBlock = new ModuleDefinition()
            //    .AddSystemTypes();

            DirectAcyclicImportGraph? importGraph = null;
            List<CompilerError>? compilerErrors = new List<CompilerError>();

            try {
                importGraph = await DirectAcyclicImportGraph.GenerateImportGraphAsync(importPaths, ConfigureModuleParameters);

                foreach (var import in importGraph.TopologizedImports) {
                    import.ReadModule();
                }

                foreach (var import in importGraph.TopologizedImports) {
                    import.ResolveModule();
                }
            } catch (Exception error) when (error is IParsingException parsingError) {
                var compilerError = await FilePassageError.CreateFromFilePassageAsync((dynamic)parsingError);
                compilerErrors.Add(compilerError);
            }

            return new CompilerResult(importGraph, compilerErrors);
        }

        public Task<CompilerResult> CompileAsync() =>
            CompileAsync(default(Action<CompilerParameters>));
    }
}
