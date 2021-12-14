using Microsoft.Extensions.Logging;
using SCUMSLang.Imports.Graph.Factory;

namespace SCUMSLang.CommandLine
{
    public class OptionsMapper
    {
        public ImportGraphFactoryParameters ToImportGraphFactoryParameters(Options options)
        {
            var parameters = new ImportGraphFactoryParameters();
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

            return parameters;
        }
    }
}
