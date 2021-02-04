using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCUMSLang
{
    public ref struct Reader<T>
    {
        public readonly ReadOnlySpan<T> Span { get; }

        public readonly ReadOnlySpan<T> View {
            get {
                Debug.Assert(viewReadLength >= 0, "Content read length should not be lesser than zero.");
                return viewLastPosition.View;
            }
        }

        public readonly T ViewLastValue => View[View.Length - 1];

        public readonly int ReadPosition => readPosition;
        public readonly int ViewReadLength => viewReadLength;
        public readonly ReaderPosition<T> ViewLastPosition => viewLastPosition;

        /// <summary>
        /// The position which takes length into account.
        /// </summary>
        public readonly int UpperPosition {
            get {
                if (viewReadLength == 0) {
                    throw new InvalidOperationException("Not a single value has been consumed.");
                }

                return readPosition + viewReadLength - 1;
            }
        }

        private int readPosition;
        private int viewReadLength;
        private ReaderPosition<T> viewLastPosition;

        public Reader(ReadOnlySpan<T> span)
        {
            Span = span;
            readPosition = 0;
            viewReadLength = 0;
            viewLastPosition = default;
        }

        private void setCurrentPosition() =>
            viewLastPosition = new ReaderPosition<T>(readPosition, Span.Slice(readPosition, viewReadLength), viewReadLength - 1);

        public bool ConsumeNext(int count)
        {
            viewReadLength += count;

            if (readPosition + viewReadLength > Span.Length) {
                return false;
            }

            setCurrentPosition();
            return true;
        }

        public bool ConsumeNext() =>
            ConsumeNext(1);

        public bool ConsumeNext(int count, out ReaderPosition<T> position)
        {
            if (ConsumeNext(count)) {
                position = viewLastPosition;
                return true;
            }

            position = default!;
            return false;
        }

        public bool ConsumeNext(out ReaderPosition<T> position) =>
            ConsumeNext(1, out position);

        public bool ConsumeNext(out ReadOnlySpan<T> values)
        {
            if (ConsumeNext()) {
                values = viewLastPosition.View;
                return true;
            }

            values = ReadOnlySpan<T>.Empty;
            return false;
        }

        public bool ConsumeNext(out T value)
        {
            if (ConsumeNext()) {
                value = viewLastPosition.Value;
                return true;
            }

            value = default!;
            return false;
        }

        public bool ConsumeNext(int count, T value, IEqualityComparer<T> comparer)
        {
            comparer ??= EqualityComparer<T>.Default;

            if (ConsumeNext(count) && comparer.Equals(viewLastPosition.View.Last(), value)) {
                return true;
            }

            return false;
        }

        public bool ConsumeNext(T value, IEqualityComparer<T> comparer) =>
            ConsumeNext(1, value, comparer);

        public bool ConsumeNext(bool consume)
        {
            if (consume) {
                return ConsumeNext();
            }

            return true;
        }

        public void ConsumeUntilNot(T value, IEqualityComparer<T> comparer)
        {
            while (PeekNext(value, comparer)) {
                ConsumeNext();
            }
        }

        public void ConsumeUntilNot(UntilDelegate<T> untilNot)
        {
            while (PeekNext(out var position) && untilNot(ref position)) {
                ConsumeNext();
            }
        }

        public void ConsumeUntil(T value, IEqualityComparer<T> comparer)
        {
            while (!PeekNext(value, comparer)) {
                ConsumeNext();
            }
        }

        public void ConsumeUntil(UntilDelegate<T> until)
        {
            while (PeekNext(out ReaderPosition<T> position)) {
                if (until(ref position)) {
                    break;
                }

                _ = ConsumeNext();
            }
        }

        public void ConsumePrevious(int valuesRead)
        {
            viewReadLength -= valuesRead;
            setCurrentPosition();
        }

        public void ConsumePrevious(int amount, out ReaderPosition<T> position)
        {
            ConsumePrevious(amount);
            position = viewLastPosition;
        }

        public void ConsumePrevious(int amount, out ReadOnlySpan<T> values)
        {
            ConsumePrevious(amount);
            values = viewLastPosition.View;
        }

        public bool PeekNext(int count, out ReaderPosition<T> position)
        {
            var reader = this;
            return reader.ConsumeNext(count, out position);
        }

        public bool PeekNext(out ReaderPosition<T> position) =>
            PeekNext(1, out position);

        public bool PeekNext(int count, T value, IEqualityComparer<T> comparer)
        {
            var reader = this;

            if (reader.ConsumeNext(count, value, comparer)) {
                return true;
            }

            return false;
        }

        public bool PeekNext(T value, IEqualityComparer<T> comparer) =>
            PeekNext(1, value, comparer);

        public void TakePositionView(int position)
        {
            readPosition = position;
            viewReadLength = 1;
            setCurrentPosition();
        }

        public void TakePositionView(ReaderPosition<T> position) =>
            TakePositionView(position.UpperReaderPosition);

        public bool TakeNextPositionView()
        {
            if (ConsumeNext()) {
                TakePositionView(viewLastPosition);
                return true;
            }

            return false;
        }

        public bool TakeNextPositionView(T value, IEqualityComparer<T> comparer)
        {
            if (ConsumeNext(value, comparer)) {
                TakePositionView(viewLastPosition);
                return true;
            }

            return false;
        }

        public void SetPositionTo(int newPosition)
        {
            readPosition = newPosition;
            viewReadLength = 0;
            setCurrentPosition();
        }

        public void SetLengthTo(int newPosition)
        {
            var positionDistance = newPosition - readPosition + 1;
            viewReadLength = positionDistance;
            setCurrentPosition();
        }
    }
}
