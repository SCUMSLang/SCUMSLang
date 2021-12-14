﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using SCUMSLang.CommandLine;
using SCUMSLang.Imports.Graph;

namespace SCUMSLang
{
    public static class Program
    {
        private static bool TryParseArguments(string[] arguments, [MaybeNullWhen(false)] out Options options)
        {
            var commandLineParser = new Parser(options => {
                options.HelpWriter = Console.Out;
            });

            var result = commandLineParser.ParseArguments<Options>(arguments);

            if (result.Value is null) {
                options = null;
                return false;
            }

            options = result.Value;
            return true;
        }

        private static async Task<int> Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!TryParseArguments(args, out var options)) {
                return 1;
            }

            ImportGraphFactoryResult compilerResult;

            try {
                void ConfigureParserParameters(ImportGraphFactoryParameters parameters)
                {
                    parameters.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

                    if (options.SystemSources.Count != 0) {
                        parameters.SystemSources.AddRange(options.SystemSources);
                    }

                    parameters.UserSources.AddRange(options.UserSources);

                    if (options.NoImplicitUInt32Pool) {
                        parameters.NoImplicitUInt32Pool = true;
                    } else if (!(options.ImplicitUInt32PoolSource is null)) {
                        parameters.ImplicitUInt32PoolSource = options.ImplicitUInt32PoolSource;
                    }
                }

                compilerResult = await ImportGraphFactory.Default.CompileAsync(ConfigureParserParameters);
            } catch (AggregateException error) {
                Console.WriteLine(error.Message);

                error.Handle(innerError => {
                    Console.WriteLine($"--> {innerError.Message}");
                    return true;
                });

                return 1;
            } catch (Exception error) {
                Console.WriteLine(error.Message);
                return 1;
            } finally {
                stopwatch.Stop();
            }

            if (compilerResult.HasErrors) {
                foreach (var error in compilerResult.Errors) {
                    Console.WriteLine(error.ToString());
                }
            } else {
                Console.WriteLine($"Finished in {Math.Truncate(stopwatch.Elapsed.TotalMinutes)}m {stopwatch.Elapsed.Seconds}s");
            }

            return 0;
        }
    }
}
