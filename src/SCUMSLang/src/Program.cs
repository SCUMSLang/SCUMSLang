﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommandLine;
using SCUMSLang.CommandLine;
using SCUMSLang.Compilation;

namespace SCUMSLang
{
    public partial class Program
    {
        static async Task<int> Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var commandLineParser = new Parser(options => {
                options.HelpWriter = Console.Out;
            });

            Options options = null!;

            var result = commandLineParser.ParseArguments<Options>(args);

            if (result.Value is null) {
                return 1;
            } else {
                options = result.Value;
            }

            try {
                await Compiler.Default.CompileAsync(parameters => {
                    if (options.SystemSources.Count != 0) {
                        parameters.SystemSources.AddRange(options.SystemSources);
                    }

                    parameters.UserSources.AddRange(options.UserSources);

                    if (options.NoImplicitUInt32Pool) {
                        parameters.NoImplicitUInt32Pool = true;
                    } else if (!(options.ImplicitUInt32PoolSource is null)) {
                        parameters.ImplicitUInt32PoolSource = options.ImplicitUInt32PoolSource;
                    }
                });

                Console.WriteLine($"Finished in {stopwatch.Elapsed.TotalMinutes}m {stopwatch.Elapsed.Seconds}s");
            } catch (Exception error) {
                Console.WriteLine(error.Message);
            } finally {
                stopwatch.Stop();
            }

            return 0;
        }
    }
}
