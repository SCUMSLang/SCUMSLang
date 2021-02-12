using System.Collections.Generic;
using CommandLine;

namespace SCUMSLang.CommandLine
{
    public class Options
    {
        [Option("system-sources", Default = null, HelpText = "A list of paths that contains all system relevant imports. If unset, it is chosen automatically.")]
        public IEnumerable<string> SystemSources { get; } = null!;

        [Option('i', "user-source", Required = true, Default = null, HelpText = "A path of an user defined source file.")]
        public string UserSource { get; } = null!;

        public Options(IEnumerable<string> systemSources, string userSource)
        {
            SystemSources = systemSources;
            UserSource = userSource;
        }
    }
}
