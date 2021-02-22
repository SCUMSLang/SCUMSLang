using System;
using System.Collections.Generic;

namespace SCUMSLang
{
    public static class ReadOnlySpanExtensions
    {
        public static T Last<T>(this ReadOnlySpan<T> span) =>
            span[span.Length - 1];

        public static T Last<T>(this ReadOnlySpan<T> span, T value, IEqualityComparer<T> equalityComparer) {
            var currentIndex = span.Length - 1;

            while (currentIndex > 0) {
                if (equalityComparer.Equals(span[currentIndex], value)) {
                    return span[currentIndex];
                }
            }

            throw new ArgumentException("Value not found.");
        }
    }
}
