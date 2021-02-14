using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.AST
{
    public class MethodCallDefinitionEqualityComparer : EqualityComparer<MethodCallDefinition>
    {
        public new static MethodCallDefinitionEqualityComparer Default = new MethodCallDefinitionEqualityComparer();

        public MethodReferenceEqualityComparer MethodReferenceComparer {
            get {
                if (methodReferenceComparer is null) {
                    methodReferenceComparer = MethodReferenceEqualityComparer.Default;
                }

                return methodReferenceComparer;
            }

            set => methodReferenceComparer = value;
        }

        public ConstantDefinitionEqualityComparer ConstantDefinitionComparer {
            get {
                if (constantDefinitionComparer is null) {
                    constantDefinitionComparer = ConstantDefinitionEqualityComparer.Default;
                }

                return constantDefinitionComparer;
            }

            set => constantDefinitionComparer = value;
        }

        private MethodReferenceEqualityComparer? methodReferenceComparer;
        private ConstantDefinitionEqualityComparer? constantDefinitionComparer;

        public override bool Equals([AllowNull] MethodCallDefinition x, [AllowNull] MethodCallDefinition y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && MethodReferenceComparer.Equals(x.InferredMethod, y.InferredMethod)
            && Enumerable.SequenceEqual(x.GenericArguments, y.GenericArguments, ConstantDefinitionComparer)
            && Enumerable.SequenceEqual(x.Arguments, y.Arguments, ConstantDefinitionComparer);

        public override int GetHashCode([DisallowNull] MethodCallDefinition obj) =>
            HashCode.Combine(obj.TokenType, obj.InferredMethod, obj.GenericArguments, obj.Arguments);
    }
}
