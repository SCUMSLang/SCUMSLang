﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using SCUMSLang.SyntaxTree.Definitions;
using SCUMSLang.SyntaxTree.References;
using SCUMSLang.Tokenization;

namespace SCUMSLang.SyntaxTree.Parser
{
    public class SyntaxTreeParserOptions
    {
        public ModuleDefinition Module {
            get {
                if (module is null) {
                    module = new ModuleDefinition();
                }

                return module;
            }

            set => module = value;
        }

        public bool AutoResolve { get; set; }
        public RecognizableReferences RecognizableNodes { get; set; }
        public TruthyReferenceHandler? WhileContinueDelegate { get; set; }
        public TruthyReferenceHandler? WhileBreakDelegate { get; set; }
        public bool EmptyRecognizationResultsIntoReturn { get; set; }
        public ILoggerFactory? LoggerFactory { get; set; }

        public SpanReaderBehaviour<Token> TokenReaderBehaviour {
            get {
                if (tokenReaderBehaviour is null) {
                    tokenReaderBehaviour = SpanReaderBehaviour<Token>.Default;
                }

                return tokenReaderBehaviour;
            }

            set => tokenReaderBehaviour = value;
        }

        public int TokenReaderStartPosition { get; set; }

        private SpanReaderBehaviour<Token>? tokenReaderBehaviour;
        private ModuleDefinition? module;

        public SyntaxTreeParserOptions() =>
            RecognizableNodes = (RecognizableReferences)Enum
                .GetValues(typeof(RecognizableReferences))
                    .Cast<int>()
                    .Sum(); // All.
    }
}
