using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCUMSLang.IO;
using System.IO;
using SCUMSLang.SyntaxTree;
using SCUMSLang.SyntaxTree.Definitions;
using System.Linq;

namespace SCUMSLang.Compilation
{
    public class Compiler
    {
        static Import ResolveImport(IEnumerable<Import> imports, string importPath)
        {
            var import = imports.SingleOrDefault(x => x.ImportPath == importPath);

            if (import is null) {
                throw SyntaxTreeThrowHelper.ModuleNotFound(importPath, stackTrace: Environment.StackTrace);
            }

            return import;
        }

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

        public async Task<CompilerResult> CompileAsync(Action<CompilerParameters>? parametersCallback)
        {
            var compilerParameters = new CompilerParameters();
            parametersCallback?.Invoke(compilerParameters);
            var importPaths = GetImportPaths(compilerParameters);

            DirectAcyclicImportGraph? importGraph = null;
            List<CompilerError>? compilerErrors = new List<CompilerError>();

            try {
                void ConfigureModuleParameters(ModuleParameters moduleParameters)
                {
                    moduleParameters.LoggerFactory = compilerParameters.LoggerFactory;

                    var referenceResolver = new ReferenceResolverPool();
                    referenceResolver.Add(systemModule.BlockReferenceResolver);
                    moduleParameters.ReferenceResolver = referenceResolver;
                }

                importGraph = await DirectAcyclicImportGraph.GenerateImportGraphAsync(importPaths, ConfigureModuleParameters);

                foreach (var import in importGraph.TopologizedImports) {
                    import.ParseModule();
                }

                foreach (var import in importGraph.TopologizedImports) {
                    import.ResolveModule(importPath => ResolveImport(importGraph.TopologizedImports, importPath).Module);
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
