namespace SCUMSLang.SyntaxTree
{
    public static class SystemTypeExtensions
    {
        public static string Sequence(this SystemType systemType) =>
            Sequences.SystemTypes[systemType];
    }
}
