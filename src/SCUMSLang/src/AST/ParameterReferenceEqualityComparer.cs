using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.AST
{
    public class ParameterReferenceEqualityComparer : EqualityComparer<ParameterReference>
    {
        public new static ParameterReferenceEqualityComparer Default = new ParameterReferenceEqualityComparer();

        public override bool Equals([AllowNull] ParameterReference x, [AllowNull] ParameterReference y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Name == y.Name
            && x.ParameterType == y.ParameterType;

        public override int GetHashCode([DisallowNull] ParameterReference obj) => throw new NotImplementedException();

        public class OverloadComparer : EqualityComparer<ParameterReference>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public TypeReferenceEqualityComparer TypeReferenceComparer { get; }

            public OverloadComparer(TypeReferenceEqualityComparer? typeReferenceComparer = null) =>
                TypeReferenceComparer = typeReferenceComparer ?? TypeReferenceEqualityComparer.Default;

            public override bool Equals([AllowNull] ParameterReference x, [AllowNull] ParameterReference y) =>
                ReferenceEquals(x, y) || !(x is null) && !(y is null)
                && TypeReferenceComparer.Equals(x.ParameterType, y.ParameterType);

            public override int GetHashCode([DisallowNull] ParameterReference obj) =>
                HashCode.Combine(obj.TokenType, obj.Name, obj.ParameterType);
        }
    }
}
