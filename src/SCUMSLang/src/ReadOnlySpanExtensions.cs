using System;

namespace SCUMSLang
{
    public static class ReadOnlySpanExtensions
    {
        public static T Last<T>(this ReadOnlySpan<T> span) =>
            span[span.Length - 1];
    }
}
