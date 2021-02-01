using System;

namespace SCUMSLang
{
    public readonly ref struct ReaderPosition<T>
    {
        public int LowerReaderPosition { get; }
        public ReadOnlySpan<T> View { get; }
        public int UpperViewPosition { get; }
        public int UpperReaderPosition => LowerReaderPosition + UpperViewPosition;
        public T Value => View[UpperViewPosition];

        public ReaderPosition(int lowerReaderPosition, ReadOnlySpan<T> view, int upperViewPosition)
        {
            LowerReaderPosition = lowerReaderPosition;
            View = view;
            UpperViewPosition = upperViewPosition;
        }

        public static implicit operator T(ReaderPosition<T> position) =>
            position.Value;
    }
}
