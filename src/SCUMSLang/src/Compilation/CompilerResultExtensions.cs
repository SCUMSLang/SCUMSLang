namespace SCUMSLang.Compilation
{
    public static class CompilerResultExtensions
    {
        public static void ThrowOnError(this CompilerResult result)
        {
            if (result.HasErrors) {
                throw result.Errors[0].WrappedException;
            }
        }
    }
}
