using System;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
{
    public sealed class ResolveResult<T>
    {
        public Exception? Error { get; }

        [MemberNotNullWhen(true, nameof(Error))]
        [MemberNotNullWhen(false, nameof(value))]
        public bool HasError => Error is not null;

        public T Value => HasError ? throw Error : value;

        private T? value;

        public ResolveResult(T value) =>
            this.value = value;

        public ResolveResult(Exception error) =>
            Error = error;
    }
}
