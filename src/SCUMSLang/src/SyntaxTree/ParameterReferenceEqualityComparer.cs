﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree
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

            public TypeReferenceEqualityComparer.OverloadComparer TypeOverloadComparer {
                get {
                    if (typeOverloadComparer is null) {
                        typeOverloadComparer = TypeReferenceEqualityComparer.OverloadComparer.Default;
                    }

                    return typeOverloadComparer;
                }

                set => typeOverloadComparer = value;
            }

            private TypeReferenceEqualityComparer.OverloadComparer? typeOverloadComparer;

            public override bool Equals([AllowNull] ParameterReference x, [AllowNull] ParameterReference y) =>
                ReferenceEquals(x, y) || !(x is null) && !(y is null)
                && TypeOverloadComparer.Equals(x.ParameterType, y.ParameterType);

            public override int GetHashCode([DisallowNull] ParameterReference obj) =>
                HashCode.Combine(obj.NodeType, obj.Name, obj.ParameterType);
        }
    }
}