using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommandLine;
using SCUMSLang.CommandLine;
using SCUMSLang.Compilation;

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

        private static void ConfigureParserParameters(Options options, CompilerParameters parameters)
        {
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

        private static async Task<int> Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!TryParseArguments(args, out var options)) {
                return 1;
            }

            CompilerResult compilerResult;

            try {
                compilerResult = await Compiler.Default.CompileAsync(parameters => ConfigureParserParameters(options, parameters));
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
                Console.WriteLine($"Finished in {stopwatch.Elapsed.TotalMinutes}m {stopwatch.Elapsed.Seconds}s");
            }

            return 0;
        }
    }
}
