namespace SCUMSLang.SyntaxTree
{
    public static class IMemberExtensions
    {
        public static IMember AsMember<T>(this T member)
            where T : IMember =>
            member;
    }
}
