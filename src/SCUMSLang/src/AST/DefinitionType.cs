using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public enum DefinitionType
    {
        [Sequence("Int32")]
        Integer,
        [Sequence("String")]
        String,
        // Only used for name lookup.
        [Sequence("Boolean")]
        Boolean,
        [Sequence("enum")]
        Enumeration,
        EnumerationMember
    }
}
