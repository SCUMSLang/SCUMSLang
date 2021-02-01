using System;

namespace SCUMSLang.Tokenization
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TokenValueTypeAttribute : Attribute
    {
        public TokenValueTypeAttribute() { }
    }
}
