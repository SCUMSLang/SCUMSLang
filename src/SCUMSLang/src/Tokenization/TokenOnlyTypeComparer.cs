using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.Tokenization
{
    public class TokenOnlyTypeComparer : EqualityComparer<Token>
    {
        public new static TokenOnlyTypeComparer Default = new TokenOnlyTypeComparer();

        public override bool Equals([AllowNull] Token x, [AllowNull] Token y) =>
            ReferenceEquals(x, y) || x is null || y is null || x.TokenType == y.TokenType;

        public override int GetHashCode([DisallowNull] Token obj) =>
            obj.TokenType.GetHashCode();
    }
}
