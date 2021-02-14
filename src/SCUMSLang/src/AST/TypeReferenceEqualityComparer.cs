using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.AST
{
    public class TypeReferenceEqualityComparer : EqualityComparer<TypeReference>
    {
        public new static TypeReferenceEqualityComparer Default = new TypeReferenceEqualityComparer();

        public override bool Equals([AllowNull] TypeReference x, [AllowNull] TypeReference y) {
            if (ReferenceEquals(x, y)) {
                return true;
            }

            if (x is null || y is null) {
                return false;
            }

            var xDefinition = x.ResolveNonAlias();
            var yDefinition = y.ResolveNonAlias();

            return xDefinition.TokenType == yDefinition.TokenType
                && xDefinition.Name == yDefinition.Name
                && xDefinition.IsEnum == yDefinition.IsEnum;
        }

        public override int GetHashCode([DisallowNull] TypeReference obj) => throw new NotImplementedException();
    }
}
