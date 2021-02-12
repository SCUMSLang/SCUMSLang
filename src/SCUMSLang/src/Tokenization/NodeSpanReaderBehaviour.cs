namespace SCUMSLang.Tokenization
{
    public static class TokenSpanReaderBehaviourExtensions
    {
        public static SpanReaderBehaviour<Token> SetNonParserChannelTokenSkipCondition(this SpanReaderBehaviour<Token> behaviour) {
            behaviour.SkipCondition = (ref ReaderPosition<Token> tokenPosition) => {
                return tokenPosition.Value.Channel != Channel.Parser;
            };

            return behaviour;
        }
    }
}
