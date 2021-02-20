using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SCUMSLang.SyntaxTree;
using SCUMSLang.IO;
using System.IO;

namespace SCUMSLang.Compilation
{
    public class Compiler
    {
        public static Compiler Default = new Compiler();

        public CompilerSettings Settings { get; }

        public Compiler() =>
            Settings = new CompilerSettings();

        public Compiler(Action<CompilerSettings> settingsCallback)
        {
            var options = new CompilerSettings();
            settingsCallback?.Invoke(options);
            Settings = options;
        }

        public async Task<CompilerResult> CompileAsync(Action<CompilerParameters>? parametersCallback)
        {
            var parameters = new CompilerParameters();
            parametersCallback?.Invoke(parameters);

            var importPaths = new List<string>();

            static void addImportPath(List<string> importPaths, string importPath)
            {
                if (!PathTools.IsFullPath(importPath)) {
                    importPath = Path.GetFullPath(importPath);
                }

                importPaths.Add(importPath);
            }

            foreach (var systemSource in parameters.SystemSources) {
                addImportPath(importPaths, systemSource);
            }

            foreach (var userSource in parameters.UserSources) {
                addImportPath(importPaths, userSource);
            }

            if (!(parameters.ImplicitUInt32PoolSource is null)) {
                addImportPath(importPaths, parameters.ImplicitUInt32PoolSource);
            }

            var systemBlock = new ModuleDefinition()
                .AddSystemTypes();

            DirectAcyclicImportGraph? importGraph = null;
            List<CompilerError>? compilerErrors = new List<CompilerError>();

            try {
                importGraph = await DirectAcyclicImportGraph.LoadImportGraphAsync(importPaths);

                foreach (var import in importGraph.TopologizedImports) {
                    import.ParseToEnd();
                }
            } catch (SyntaxTreeParsingException error) {
                var compilerError = await FilePassageError.CreateFromFilePassageAsync(error);
                compilerErrors.Add(compilerError);
            }

            return new CompilerResult(importGraph, compilerErrors);
        }

        public Task<CompilerResult> CompileAsync() =>
            CompileAsync(default(Action<CompilerParameters>));
    }
}
