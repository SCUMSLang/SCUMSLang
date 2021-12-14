using System.Collections.Generic;
using CommandLine;
using SCUMSLang.Imports.Graph;

namespace SCUMSLang.CommandLine
{
    public class Options
    {
        [Option("system-sources", HelpText = "Path(s) for system source file(s). If unset it is: " + ImportGraphFactoryParameters.RelativeUMSLFilesIndexPath)]
        public IReadOnlyList<string> SystemSources { get; } = null!;

        [Option('i', "user-sources", Required = true, HelpText = "Path(s) of user defined source file(s).")]
        public IReadOnlyList<string> UserSources { get; } = null!;

        [Option("no-implicit-int-pool", SetName = "implicit-int-pool", HelpText = "When specified the default death counters are not reserved for compiler anymore. Care! You need to set them now manually!")]
        public bool NoImplicitUInt32Pool { get; }

        [Option("implicit-int-pool-source", SetName = "implicit-int-pool", HelpText = "Path for source file that defines the implicit UInt32-pool. If unset it is:" + ImportGraphFactoryParameters.RelativeUMSLFilesUInt32PoolPath)]
        public string ImplicitUInt32PoolSource { get; }

        public Options(IReadOnlyList<string>? systemSources, IReadOnlyList<string> userSources, bool noImplicitUInt32Pool, string implicitUInt32PoolSource)
        {
            SystemSources = systemSources ?? new List<string>();
            UserSources = userSources; // Null exception is handled by command-line parser.
            NoImplicitUInt32Pool = noImplicitUInt32Pool;
            ImplicitUInt32PoolSource = implicitUInt32PoolSource;
        }
    }
}
