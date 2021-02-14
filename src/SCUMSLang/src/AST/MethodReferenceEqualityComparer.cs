using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.AST
{
    public class MethodReferenceEqualityComparer : EqualityComparer<MethodReference>
    {
        public new static MethodReferenceEqualityComparer Default = new MethodReferenceEqualityComparer();

        public ParameterReferenceEqualityComparer ParameterComaprer {
            get {
                if (parameterComaprer is null) {
                    parameterComaprer = ParameterReferenceEqualityComparer.Default;
                }

                return parameterComaprer;
            }

            set => parameterComaprer = value;
        }

        private ParameterReferenceEqualityComparer? parameterComaprer;

        public override bool Equals([AllowNull] MethodReference x, [AllowNull] MethodReference y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Name == y.Name
            && Enumerable.SequenceEqual(x.GenericParameters, y.GenericParameters, parameterComaprer)
            && Enumerable.SequenceEqual(x.Parameters, y.Parameters, parameterComaprer);

        public override int GetHashCode([DisallowNull] MethodReference obj) =>
            HashCode.Combine(obj.TokenType, obj.Name, obj.GenericParameters, obj.Parameters);

        public class OverloadComparer : EqualityComparer<MethodReference>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public ParameterReferenceEqualityComparer.OverloadComparer ParameterOverloadComaprer {
                get {
                    if (parameterOverloadComaprer is null) {
                        parameterOverloadComaprer = ParameterReferenceEqualityComparer.OverloadComparer.Default;
                    }

                    return parameterOverloadComaprer;
                }

                set => parameterOverloadComaprer = value;
            }

            private ParameterReferenceEqualityComparer.OverloadComparer? parameterOverloadComaprer;

            public override bool Equals([AllowNull] MethodReference x, [AllowNull] MethodReference y) =>
                ReferenceEquals(x, y) || !(x is null) && !(y is null)
                && x.Name == y.Name
                && Enumerable.SequenceEqual(x.GenericParameters, y.GenericParameters, ParameterOverloadComaprer)
                && Enumerable.SequenceEqual(x.Parameters, y.Parameters, ParameterOverloadComaprer);

            public override int GetHashCode([DisallowNull] MethodReference obj) =>
                HashCode.Combine(obj.TokenType, obj.Name, obj.GenericParameters, obj.Parameters);
        }
    }
}
