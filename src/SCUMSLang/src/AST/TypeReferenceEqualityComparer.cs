using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.AST
{
    public class TypeReferenceEqualityComparer : EqualityComparer<TypeReference>
    {
        public new static TypeReferenceEqualityComparer Default = new TypeReferenceEqualityComparer();

        public override bool Equals([AllowNull] TypeReference x, [AllowNull] TypeReference y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Equals(y);

        public override int GetHashCode([DisallowNull] TypeReference obj) =>
            obj.GetHashCode();

        public class OverloadComparer : EqualityComparer<TypeReference>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public override bool Equals([AllowNull] TypeReference x, [AllowNull] TypeReference y)
            {
                if (ReferenceEquals(x, y)) {
                    return true;
                }

                if (x is null || y is null) {
                    return false;
                }

                var xDefinition = x.ResolveNonAlias();
                var yDefinition = y.ResolveNonAlias();
                return xDefinition.Equals(yDefinition);
            }

            public override int GetHashCode([DisallowNull] TypeReference obj) => throw new NotImplementedException();
        }
    }
}
