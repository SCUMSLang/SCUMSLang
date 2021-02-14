﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SCUMSLang.AST;
using SCUMSLang.IO;
using SCUMSLang.Tokenization;

namespace SCUMSLang.Compilation
{
    public class Compiler
    {
        public static Compiler Default = new Compiler();

        public async Task CompileAsync(IEnumerable<string> systemSources, string userSource)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var importPaths = new List<string>(systemSources);

            if (importPaths.Count == 0) {
                var assemblyPath = typeof(Program).Assembly.Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
                var headerIndexPath = Path.Combine(assemblyDirectory, "Headers/Index.umsh");
                importPaths.Add(headerIndexPath);
            }

            //importPaths.Add(options.UserSource);

            var systemBlock = new ModuleDefinition()
                .AddSystemTypes();

            var importGraph = await DirectAcyclicImportGraph.CreateAsync(importPaths);

            foreach (var import in importGraph.SortedImports) {
                var parser = new TreeParser(options => {
                    options.Module = import.Module;
                    options.TokenReaderStartPosition = import.TokenReaderUpperPosition + 1;
                    options.TokenReaderBehaviour.SetSkipConditionForNonParserChannelTokens();
                });

                parser.Parse(import.TokensAsReadOnlySpan());
            }

            stopwatch.Stop();
            Console.WriteLine($"Finished in {stopwatch.Elapsed.TotalMinutes}m {stopwatch.Elapsed.Seconds}s");
        }
    }
}
