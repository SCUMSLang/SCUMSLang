using System;
using System.Diagnostics;
using SCUMSLang.SyntaxTree;
using SCUMSLang.Tokenization;

namespace SCUMSLANG.SyntaxTree
{
    public partial class TreeParserTests
    {
        public Action<SyntaxTreeParserOptions> ParserChannelParserOptionsCallback { get; }
        public SyntaxTreeParser DefaultParser { get; }
        public ModuleDefinition ExpectedModule { get; }

        public TypeDefinition UInt32Type { get; }
        public TypeDefinition IntType { get; }
        public TypeDefinition StringType { get; }

        public TreeParserTests()
        {
            Trace.Listeners.Add(new DefaultTraceListener());

            ExpectedModule = new ModuleDefinition()
                .AddSystemTypes();

            ParserChannelParserOptionsCallback = options => {
                options.TokenReaderBehaviour
                    .SetSkipConditionForNonParserChannelTokens();
            };

            UInt32Type = new TypeDefinition(ExpectedModule, "UInt32");
            StringType = new TypeDefinition(ExpectedModule, "String");
            IntType = new TypeDefinition(ExpectedModule, "int") { BaseType = new TypeReference(ExpectedModule, "UInt32") };

            DefaultParser = new SyntaxTreeParser(options => {
                options.Module.AddSystemTypes();
            });
        }
    }
}
