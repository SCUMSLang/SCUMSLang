using System;
using System.Collections.Generic;

namespace SCUMSLang.Tokenization
{
    public static class TokenTypeLibrary
    {
        internal static Type TypeOfTokenType;
        internal static List<(TokenType TokenType, string Keyword)> TokenAscendedKeywords { get; }
        internal static TokenType[] ValueTypes { get; }

        static TokenTypeLibrary()
        {
            TypeOfTokenType = typeof(TokenType);
            TokenAscendedKeywords = new List<(TokenType TokenType, string Keyword)>();
            var tokenTypeValues = Enum.GetValues(TypeOfTokenType);
            var index = tokenTypeValues.Length;
            var keywordAttributeType = typeof(TokenKeywordAttribute);
            var valueTypeAttributeType = typeof(TokenValueTypeAttribute);
            var valueTypeTokens = new List<TokenType>();

            while (--index >= 0) {
                var tokenType = (TokenType)tokenTypeValues.GetValue(index)!;
                var tokenTypeName = Enum.GetName(TypeOfTokenType, tokenType)!;
                var memberInfo = TypeOfTokenType.GetField(tokenTypeName)!;
                var keywordAttributes = Attribute.GetCustomAttribute(memberInfo, keywordAttributeType) as TokenKeywordAttribute;

                if (!(keywordAttributes is null)) {
                    foreach (var keyword in keywordAttributes.Keywords) {
                        TokenAscendedKeywords.Add((tokenType, keyword));
                    }
                }

                var valueTypeAttributes = Attribute.GetCustomAttribute(memberInfo, valueTypeAttributeType) as TokenValueTypeAttribute;

                if (!(keywordAttributes is null)) {
                    valueTypeTokens.Add(tokenType);
                }
            }

            TokenAscendedKeywords.Sort(Comparer<(TokenType TokenType, string Keyword)>.Create((x, y) => x.Keyword.CompareTo(y.Keyword)));
            ValueTypes = valueTypeTokens.ToArray();
        }
    }
}
