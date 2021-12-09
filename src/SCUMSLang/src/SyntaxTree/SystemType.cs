using SCUMSLang.Tokenization;

namespace SCUMSLang.SyntaxTree
{
    public enum SystemType
    {
        [Sequence("UInt8")]
        Byte,
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
