using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public enum DefinitionType
    {
        [Sequence("UInt32")]
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
