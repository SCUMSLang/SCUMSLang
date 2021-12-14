using System;
using System.Collections.Generic;

namespace SCUMSLang.Imports.Graph
{
    public readonly struct ImportGraph
    {
        public ImportGraphState State { get; internal init; }

        public IReadOnlyList<Import> TopologizedImports => topologizedImports ?? throw new InvalidOperationException("Imports are not generated");

        private readonly IReadOnlyList<Import>? topologizedImports;

        internal ImportGraph(IReadOnlyList<Import> topologizedImports)
        {
            State = ImportGraphState.Created;
            this.topologizedImports = topologizedImports;
        }

        internal ImportGraph WithState(ImportGraphState state) =>
            new ImportGraph(TopologizedImports) { State = state };
    }
}
