using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public enum SystemType
    {
        [Sequence("UInt32")]
        Integer,
        [Sequence("String")]
        String,
        Name,
        // Only used for name lookup.
        [Sequence("Boolean")]
        Boolean,
        [Sequence("enum")]
        Enumeration,
        EnumerationMember
    }
}
