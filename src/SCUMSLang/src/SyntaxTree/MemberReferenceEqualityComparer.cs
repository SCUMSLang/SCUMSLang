using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
{
    public class MemberReferenceEqualityComparer : EqualityComparer<MemberReference>
    {
        public new static MemberReferenceEqualityComparer Default = new MemberReferenceEqualityComparer();

        public override bool Equals([AllowNull] MemberReference x, [AllowNull] MemberReference y) =>
                ReferenceEquals(x, y) || !(x is null) && !(y is null)
                && x.NodeType == y.NodeType
                && x.Name == y.Name
                && Equals(x.DeclaringType, y.DeclaringType);

        public override int GetHashCode([DisallowNull] MemberReference obj) =>
            throw new NotImplementedException();
    }
}
