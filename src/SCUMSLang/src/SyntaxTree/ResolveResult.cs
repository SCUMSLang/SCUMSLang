using System;

namespace SCUMSLang.SyntaxTree
{
    public sealed record ResolveResult
    {
        public static ResolveResult<T> Success<T>(T value)
        {
            return new ResolveResult<T>(value);
        }

        public static ResolveResult<T> Failed<T>(Exception error)
        {
            return new ResolveResult<T>(error);
        }
    }
}
