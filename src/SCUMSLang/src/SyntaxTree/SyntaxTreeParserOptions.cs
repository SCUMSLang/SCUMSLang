using System;
using System.Linq;
using SCUMSLang.Tokenization;

namespace SCUMSLang.SyntaxTree
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

        public RecognizableReferences RecognizableNodes { get; set; }
        public TruthyReferenceHandler? WhileContinueDelegate { get; set; }
        public TruthyReferenceHandler? WhileBreakDelegate { get; set; }
        public bool EmptyRecognizationResultsIntoWhileBreak { get; set; }

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
                    .Sum();
    }
}
