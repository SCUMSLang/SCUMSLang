using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SCUMSLang
{
    public ref struct SpanReader<T>
    {
        public readonly ReadOnlySpan<T> Span { get; }

        public readonly ReadOnlySpan<T> View {
            get {
                Debug.Assert(viewLastPosition.View.Length >= 0, "Content read length should not be lesser than zero.");
                return viewLastPosition.View;
            }
        }

        public readonly T ViewLastValue => View[View.Length - 1];
        public readonly int ReadPosition => viewLastPosition.LowerReaderPosition;
        public readonly int ViewReadLength => viewLastPosition.View.Length;
        public readonly ReaderPosition<T> ViewLastPosition => viewLastPosition;

        /// <summary>
        /// The position which takes length into account.
        /// </summary>
        public readonly int UpperPosition {
            get {
                if (ViewReadLength == 0) {
                    return ReadPosition;
                }

                return ReadPosition + ViewReadLength - 1;
            }
        }

        private ReaderPosition<T> viewLastPosition;
        private readonly SpanReaderBehaviour<T> behaviour;

        public SpanReader(ReadOnlySpan<T> span, SpanReaderBehaviour<T>? behaviour)
        {
            Span = span;
            viewLastPosition = default;
            this.behaviour = behaviour ?? SpanReaderBehaviour<T>.Default;
        }

        public SpanReader(ReadOnlySpan<T> span)
        {
            Span = span;
            viewLastPosition = default;
            behaviour = SpanReaderBehaviour<T>.Default;
        }

        private void setCurrentPosition(int readPosition, int viewReadLength) =>
            viewLastPosition = new ReaderPosition<T>(readPosition, Span.Slice(readPosition, viewReadLength));

        public bool ConsumeNext(int count)
        {
            var viewReadLength = ViewReadLength + count;

            do {

                if (ReadPosition + viewReadLength > Span.Length) {
                    return false;
                }

                setCurrentPosition(ReadPosition, viewReadLength);

                if (behaviour.SkipCondition?.Invoke(ref viewLastPosition) ?? false) {
                    viewReadLength++;
                } else {
                    break;
                }
            } while (true);

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

        public void ConsumeUntilNot(ReaderPositionTruthyDelegate<T> untilNot)
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

        public void ConsumeUntil(ReaderPositionTruthyDelegate<T> until)
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
            var viewReadLength = ViewReadLength - valuesRead;
            setCurrentPosition(ReadPosition, viewReadLength);
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

        public bool PeekNext(T value, IEqualityComparer<T> comparer)
        {
            var reader = this;

            if (reader.ConsumeNext(value, comparer)) {
                return true;
            }

            return false;
        }

        public bool SetPositionTo(int newPosition)
        {
            if (newPosition < Span.Length) {
                setCurrentPosition(newPosition, 0);
                return true;
            }

            return false;
        }

        public void SetLengthTo(int newPosition)
        {
            var positionDistance = newPosition - ReadPosition + 1;
            setCurrentPosition(ReadPosition, viewReadLength: positionDistance);
        }
    }
}
