using System;

namespace SCUMSLang
{
    public readonly ref struct ReaderPosition<T>
    {
        public int LowerReaderPosition { get; }
        public ReadOnlySpan<T> View { get; }

        public int UpperViewPosition => View.Length - 1;
        public T Value => View[UpperViewPosition];
        public int UpperReaderPosition => LowerReaderPosition + UpperViewPosition;

        public ReaderPosition(int lowerReaderPosition, ReadOnlySpan<T> view)
        {
            LowerReaderPosition = lowerReaderPosition;
            View = view;
        }

        public static implicit operator T(ReaderPosition<T> position) =>
            position.Value;
    }
}
