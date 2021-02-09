using SCUMSLang.Tokenization;

namespace SCUMSLang.AST
{
    public enum DefinitionType
    {
        [Sequence("Int32")]
        Integer,
        [Sequence("String")]
        String,
        [Sequence("Boolean")]
        Boolean,
        [Sequence("enum")]
        Enumeration,
        EnumerationMember
    }
}
