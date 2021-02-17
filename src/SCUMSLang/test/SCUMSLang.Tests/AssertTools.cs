using System;
using System.Collections.Generic;
using Xunit;

namespace SCUMSLang
{
    public class AssertTools
    {
        public static void Equal<T>(IEnumerator<T> expected, IEnumerator<T> actual, IEqualityComparer<T> equalityComparer)
        {
            bool enumeratorMoveNextResult;
            bool otherEnumeratorMoveNextResult;

            do {
                enumeratorMoveNextResult = expected.MoveNext();
                otherEnumeratorMoveNextResult = actual.MoveNext();

                if (!enumeratorMoveNextResult && !otherEnumeratorMoveNextResult) {
                    return;
                }

                Assert.True(enumeratorMoveNextResult, "Length of expected/actual enumerator is smaller/greater.");
                Assert.True(otherEnumeratorMoveNextResult, "Length of actual/expected enumerator is smaller/greater.");
                Assert.Equal(expected.Current, actual.Current, equalityComparer);
            } while (true);
        }

        public static void Equal<T>(IEnumerator<T> expected, IEnumerator<T> actual) =>
            Equal(expected, actual, EqualityComparer<T>.Default);

        public static void Equal<T>(ReadOnlySpan<T>.Enumerator expected, ReadOnlySpan<T>.Enumerator actual, IEqualityComparer<T> equalityComparer)
        {
            bool enumeratorMoveNextResult;
            bool otherEnumeratorMoveNextResult;

            do {
                enumeratorMoveNextResult = expected.MoveNext();
                otherEnumeratorMoveNextResult = actual.MoveNext();

                if (!enumeratorMoveNextResult && !otherEnumeratorMoveNextResult) {
                    return;
                }

                Assert.True(enumeratorMoveNextResult, "Length of expected/actual enumerator is smaller/greater.");
                Assert.True(otherEnumeratorMoveNextResult, "Length of actual/expected enumerator is smaller/greater.");
                Assert.Equal(expected.Current, actual.Current, equalityComparer);
            } while (true);
        }

        public static void Equal<T>(ReadOnlySpan<T>.Enumerator expected, ReadOnlySpan<T>.Enumerator actual) =>
            Equal(expected, actual, EqualityComparer<T>.Default);

        public static void Equal<T>(ReadOnlySpan<T> expected, ReadOnlySpan<T> actual, IEqualityComparer<T> equalityComparer) =>
            Equal(expected.GetEnumerator(), actual.GetEnumerator(), equalityComparer);

        public static void Equal<T>(ReadOnlySpan<T> expected, ReadOnlySpan<T> actual) =>
            Equal(expected, actual, EqualityComparer<T>.Default);
    }
}
