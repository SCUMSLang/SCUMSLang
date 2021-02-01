using System;

namespace SCUMSLang.Tokenization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TokenKeywordAttribute : Attribute
    {
        public string[] Keywords { get; }

        public TokenKeywordAttribute(params string[] literals) =>
            Keywords = literals;
    }
}
