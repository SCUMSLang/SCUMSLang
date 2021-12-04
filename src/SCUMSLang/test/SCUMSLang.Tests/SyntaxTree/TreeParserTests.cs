using System;
using System.Diagnostics;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.Parser;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.Tokenization;

namespace SCUMSLang.SyntaxTree
{
    public partial class TreeParserTests
    {
        public Action<SyntaxTreeParserOptions> ParserChannelParserOptionsCallback { get; }
        public SyntaxTreeParser DefaultParser { get; }
        //public ModuleDefinition ExpectedModule { get; }

        public TypeDefinition UInt32Type { get; }
        public TypeDefinition IntType { get; }
        public TypeDefinition StringType { get; }

        public TreeParserTests()
        {
            Trace.Listeners.Add(new DefaultTraceListener());

            //ExpectedModule = new ModuleDefinition()
            //    .AddSystemTypes();

            ParserChannelParserOptionsCallback = options => {
                options.TokenReaderBehaviour.SetSkipConditionForNonParserChannelTokens();
            };

            UInt32Type = new TypeDefinition("UInt32");
            StringType = new TypeDefinition("String");
            IntType = new TypeDefinition("int") { BaseType = new TypeReference("UInt32") };

            DefaultParser = new SyntaxTreeParser(options => {
                options.Module.AddSystemTypes();
            });
        }
    }
}
