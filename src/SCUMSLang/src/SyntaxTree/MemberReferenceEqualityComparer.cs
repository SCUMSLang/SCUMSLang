using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
{
    public class MemberReferenceEqualityComparer
    {
        public class ShallowComparer : EqualityComparer<MemberReference> {
            public new static ShallowComparer Default = new ShallowComparer();

            public override bool Equals([AllowNull] MemberReference x, [AllowNull] MemberReference y) =>
                ReferenceEquals(x, y) || !(x is null) && !(y is null)
                && x.NodeType == y.NodeType
                && x.LongName == y.LongName
                && Default.Equals(x.DeclaringType, y.DeclaringType);

            public override int GetHashCode([DisallowNull] MemberReference obj) => 
                throw new NotImplementedException();
        }
    }
}
