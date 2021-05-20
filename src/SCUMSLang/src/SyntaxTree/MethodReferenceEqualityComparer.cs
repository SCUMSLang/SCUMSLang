using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.SyntaxTree
{
    public class MethodReferenceEqualityComparer : EqualityComparer<MethodReference>
    {
        public new static MethodReferenceEqualityComparer Default = new MethodReferenceEqualityComparer();

        public ParameterReferenceEqualityComparer ParameterComparer {
            get {
                if (parameterComparer is null) {
                    parameterComparer = ParameterReferenceEqualityComparer.Default;
                }

                return parameterComparer;
            }

            set => parameterComparer = value;
        }

        private ParameterReferenceEqualityComparer? parameterComparer;

        public override bool Equals([AllowNull] MethodReference x, [AllowNull] MethodReference y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Name == y.Name
            && Enumerable.SequenceEqual(x.GenericParameters, y.GenericParameters, parameterComparer)
            && Enumerable.SequenceEqual(x.Parameters, y.Parameters, parameterComparer);

        public override int GetHashCode([DisallowNull] MethodReference obj) =>
            HashCode.Combine(obj.NodeType, obj.Name, obj.GenericParameters, obj.Parameters);

        public class ViaModuleResolve : EqualityComparer<MethodReference>
        {
            public new static ViaModuleResolve Default = new ViaModuleResolve();

            public ParameterReferenceEqualityComparer.OverloadComparer ParameterOverloadComparer {
                get {
                    if (parameterOverloadComaprer is null) {
                        parameterOverloadComaprer = ParameterReferenceEqualityComparer.OverloadComparer.Default;
                    }

                    return parameterOverloadComaprer;
                }

                set => parameterOverloadComaprer = value;
            }

            private ParameterReferenceEqualityComparer.OverloadComparer? parameterOverloadComaprer;

            public override bool Equals([AllowNull] MethodReference x, [AllowNull] MethodReference y)
            {
                return ReferenceEquals(x, y) || !(x is null) && !(y is null)
                    && x.Name == y.Name
                    && Enumerable.SequenceEqual(x.GenericParameters, y.GenericParameters, ParameterOverloadComparer)
                    && Enumerable.SequenceEqual(x.Parameters, y.Parameters, ParameterOverloadComparer);
            }

            public override int GetHashCode([DisallowNull] MethodReference obj) =>
                HashCode.Combine(obj.NodeType, obj.Name, obj.GenericParameters, obj.Parameters);
        }
    }
}
