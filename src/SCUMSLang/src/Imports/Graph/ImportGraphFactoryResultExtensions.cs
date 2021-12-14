namespace SCUMSLang.Imports.Graph
{
    public static class ImportGraphFactoryResultExtensions
    {
        public static void ThrowOnError(this ImportGraphFactoryResult result)
        {
            if (result.HasErrors) {
                throw result.Errors[0].Exception;
            }
        }
    }
}
