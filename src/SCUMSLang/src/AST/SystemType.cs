using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public enum SystemType
    {
        [Sequence("UInt32")]
        Integer,
        [Sequence("String")]
        String,
        Constant,
        // Only used for name lookup.
        [Sequence("Boolean")]
        Boolean,
        [Sequence("enum")]
        Enumeration,
        EnumerationMember
    }
}
