using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public enum InBuiltType
    {
        [SequenceExample("int")]
        Integer,
        [SequenceExample("string")]
        String,
        [SequenceExample("bool")]
        Boolean,
        Enumeration,
        EnumerationMember
    }
}
