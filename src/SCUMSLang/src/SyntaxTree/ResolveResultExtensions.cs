using System;

namespace SCUMSLang.SyntaxTree
{
    public static class ResolveResultExtensions
    {
        public static T? ValueOrDefault<T>(this ResolveResult<T> resolveResult)
        {
            if (resolveResult is null) {
                throw new ArgumentNullException(nameof(resolveResult));
            }

            return resolveResult.HasError ? default : resolveResult.Value;
        }

        public static IMember Resolve<T>(this ResolveResult<T> resolveResult)
            where T : IMember =>
            resolveResult.Value.Resolve();

        public static void ThrowIfError<T>(this ResolveResult<T> resolveResult)
        {
            if (resolveResult.HasError) {
                throw resolveResult.Error;
            }
        }
    }
}
