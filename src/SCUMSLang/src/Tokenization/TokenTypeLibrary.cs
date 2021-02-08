using System;
using System.Collections.Generic;
using SCUMSLang.AST;
using static SCUMSLang.SCUMSLangTools;

namespace SCUMSLang.Tokenization
{
    public static class TokenTypeLibrary
    {
        internal static List<(TokenType TokenType, string Keyword)> TokenAscendedKeywords { get; }
        internal static Dictionary<TokenType, string> SequenceExampleDictionary { get; }
        internal static Dictionary<TokenType, InBuiltType> InBuiltTypes { get; }
        internal static Dictionary<TokenType, InBuiltType> InBuiltEnumerableTypes { get; }

        static TokenTypeLibrary()
        {
            TokenAscendedKeywords = new List<(TokenType TokenType, string Keyword)>();
            SequenceExampleDictionary = new Dictionary<TokenType, string>();
            InBuiltTypes = new Dictionary<TokenType, InBuiltType>();
            InBuiltEnumerableTypes = new Dictionary<TokenType, InBuiltType>();

            ForEachEnum<TokenType>(tokenType => {
                var memberInfo = GetEnumField(tokenType);

                if (TryGetAttribute<TokenKeywordAttribute>(memberInfo, out var tokenKeyword)) {
                    foreach (var keyword in tokenKeyword.Keywords) {
                        TokenAscendedKeywords.Add((tokenType, keyword));
                    }
                }

                if (TryGetAttribute<InBuiltTypeAttribute>(memberInfo, out var inBuiltType)) {
                    if (inBuiltType.IsEnumeration) {
                        InBuiltEnumerableTypes.Add(tokenType, inBuiltType.InBuiltType);
                    } else {
                        InBuiltTypes.Add(tokenType, inBuiltType.InBuiltType);
                    }
                }

                if (TryGetAttribute<SequenceExampleAttribute>(memberInfo, out var sequenceExample)) {
                    SequenceExampleDictionary.Add(tokenType, sequenceExample.Sequence);
                }
            });

            TokenAscendedKeywords.Sort(Comparer<(TokenType TokenType, string Keyword)>.Create((x, y) => x.Keyword.CompareTo(y.Keyword)));
        }
    }
}
