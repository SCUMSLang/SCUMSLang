using System.Collections.Generic;
using static SCUMSLang.SCUMSLangTools;

namespace SCUMSLang.Tokenization
{
    public static class TokenTypeLibrary
    {
        internal static List<(TokenType TokenType, string Keyword)> TokenAscendedKeywords { get; }
        internal static Dictionary<TokenType ,string> TokenKeywords { get; }
        internal static Dictionary<TokenType, string> SequenceDictionary { get; }

        static TokenTypeLibrary()
        {
            TokenAscendedKeywords = new List<(TokenType TokenType, string Keyword)>();
            TokenKeywords = new Dictionary<TokenType, string>();
            SequenceDictionary = new Dictionary<TokenType, string>();

            ForEachEnum<TokenType>(tokenType => {
                var memberInfo = GetEnumField(tokenType);

                if (TryGetAttribute<TokenKeywordAttribute>(memberInfo, out var tokenKeyword)) {
                    foreach (var keyword in tokenKeyword.Keywords) {
                        TokenAscendedKeywords.Add((tokenType, keyword));
                        TokenKeywords[tokenType] = keyword;
                    }
                }

                if (TryGetAttribute<SequenceAttribute>(memberInfo, out var sequence)) {
                    SequenceDictionary.Add(tokenType, sequence.Sequence);
                }
            });

            TokenAscendedKeywords.Sort(Comparer<(TokenType TokenType, string Keyword)>.Create((x, y) => x.Keyword.CompareTo(y.Keyword)));
        }
    }
}
