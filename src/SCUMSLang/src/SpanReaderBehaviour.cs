namespace SCUMSLang
{
    public class SpanReaderBehaviour<T>
    {
        public static SpanReaderBehaviour<T> Default = new SpanReaderBehaviour<T>();

        //public TruthyDelegate<T>? EndCondition { get; set; }
        public TruthyReaderPositionDelegate<T>? SkipCondition { get; set; }
    }
}
