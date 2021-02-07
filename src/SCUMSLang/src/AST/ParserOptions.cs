using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public class ParserOptions
    {
        private SpanReaderBehaviour<Token>? tokenReaderBehaviour;

        public NodeTruthyHandler? NodeSkipDelegate { get; set; }
        public NodeTruthyHandler? NodeEndDelegate { get; set;  }

        public SpanReaderBehaviour<Token> TokenReaderBehaviour {
            get {
                if (tokenReaderBehaviour is null) {
                    tokenReaderBehaviour = SpanReaderBehaviour<Token>.Default;
                }

                return tokenReaderBehaviour;
            }

            set => tokenReaderBehaviour = value;
        }
    }
}
