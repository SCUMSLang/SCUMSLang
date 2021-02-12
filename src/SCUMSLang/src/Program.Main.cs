using System;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using SCUMSLang.CommandLine;
using SCUMSLang.Compilation;

namespace SCUMSLang
{
    public partial class Program
    {
        static async Task<int> Main(string[] args)
        {
            var commandLineParser = new Parser(settings => {
                settings.EnableDashDash = true;
            });

            Options options = null!;

            var exitCode = commandLineParser.ParseArguments<Options>(args)
                .MapResult(
                    parsedOptions => {
                        options = parsedOptions;
                        return 0;
                    },
                    errors => {
                        var sentenceBuilder = SentenceBuilder.Create();

                        foreach (var error in errors) {
                            Console.WriteLine(sentenceBuilder.FormatError(error));
                        }

                        return 1;
                    });

            if (exitCode > 0) {
                return exitCode;
            }

            await Compiler.Default.CompileAsync(options.SystemSources, options.UserSource);
            return 0;
        }
    }
}
