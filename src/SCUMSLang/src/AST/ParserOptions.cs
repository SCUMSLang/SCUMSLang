using System;
using System.Linq;
using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public class ParserOptions
    {
        public StaticBlockNode StaticBlock {
            get {
                if (staticBlock is null) {
                    staticBlock = new StaticBlockNode();
                }

                return staticBlock;
            }

            set => staticBlock = value;
        }

        public RecognizableNodes RecognizableNodes { get; set; }
        public TruthyNodeHandler? WhileContinueDelegate { get; set; }
        public TruthyNodeHandler? WhileBreakDelegate { get; set; }
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
        private StaticBlockNode? staticBlock;

        public ParserOptions() =>
            RecognizableNodes = (RecognizableNodes)Enum
                .GetValues(typeof(RecognizableNodes))
                    .Cast<int>()
                    .Sum();
    }
}
