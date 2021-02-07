namespace SCUMSLang
{
    public class SpanReaderBehaviour<T>
    {
        public static SpanReaderBehaviour<T> Default = new SpanReaderBehaviour<T>();

        //public TruthyDelegate<T>? EndCondition { get; set; }
        public ReaderPositionTruthyDelegate<T>? SkipCondition { get; set; }
    }
}
