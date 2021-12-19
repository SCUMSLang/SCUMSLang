using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SCUMSLang.SyntaxTree;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Visitors;

namespace SCUMSLang.Imports.Graph.Processors
{
    public class ResolvingProcessor : IImportGraphProcessor
    {
        private readonly static ResolvingVisitor ResolvingVisitor = new ResolvingVisitor(_ => throw new NotSupportedException());

        private static void ResolveModule(ModuleDefinition module, ResolvingVisitor visitor) =>
            visitor.Visit(module);

        private static Import GetImportByPath(IEnumerable<Import> imports, string importPath)
        {
            var import = imports.SingleOrDefault(x => x.ImportPath == importPath);

            if (import is null) {
                throw SyntaxTreeThrowHelper.ModuleNotFound(importPath, stackTrace: Environment.StackTrace);
            }

            return import;
        }

        public static void ResolveModule(ModuleDefinition module, ImportGraph importGraph = default, ILoggerFactory? loggerFactory = null)
        {
            ResolvingVisitor resolvingVisitor;

            if (importGraph.Equals(default)) {
                resolvingVisitor = ResolvingVisitor;
            } else {
                resolvingVisitor = new ResolvingVisitor(importPath => GetImportByPath(importGraph.TopologizedImports, importPath).Module) {
                    Logger = loggerFactory?.CreateLogger<ResolvingVisitor>()
                };
            }

            ResolveModule(module, resolvingVisitor);
        }

        public ILoggerFactory? LoggerFactory { get; init; }

        public ImportGraph ResolveModules(ImportGraph importGraph)
        {
            if (importGraph.State != ImportGraphState.Parsed) {
                throw new InvalidOperationException("The import graph need to be parsed");
            }

            foreach (var import in importGraph.TopologizedImports) {
                ResolveModule(import.Module, importGraph, LoggerFactory);
            }

            return importGraph.WithState(ImportGraphState.Resolved);
        }

        ImportGraph IImportGraphProcessor.Process(ImportGraph importGraph) =>
            ResolveModules(importGraph);
    }
}
