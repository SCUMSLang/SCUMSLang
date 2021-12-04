using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public class ConstantDefinitionEqualityComparer : EqualityComparer<ConstantDefinition>
    {
        public new static ConstantDefinitionEqualityComparer Default = new ConstantDefinitionEqualityComparer();

        public override bool Equals([AllowNull] ConstantDefinition x, [AllowNull] ConstantDefinition y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Equals(y);

        public override int GetHashCode([DisallowNull] ConstantDefinition obj) =>
            throw new NotImplementedException();

        public class ViaModuleResolve : EqualityComparer<ConstantDefinition>
        {
            public new static ViaModuleResolve Default = new ViaModuleResolve();

            public TypeReferenceEqualityComparer.ViaResolveComparer TypeReferenceComparer {
                get {
                    if (typeReferenceComparer is null) {
                        typeReferenceComparer = TypeReferenceEqualityComparer.ViaResolveComparer.Default;
                    }

                    return typeReferenceComparer;
                }

                set => typeReferenceComparer = value;
            }

            private TypeReferenceEqualityComparer.ViaResolveComparer? typeReferenceComparer;

            public override bool Equals([AllowNull] ConstantDefinition x, [AllowNull] ConstantDefinition y)
            {
                if (ReferenceEquals(x, y)) {
                    return true;
                }

                if (x is null || y is null) {
                    return false;
                }

                if (x.Value is null || y.Value is null || !Equals(x.Value, y.Value)) {
                    return false;
                }

                var xValueTypeDefinition = x.ValueType.Resolve();
                var yValueTypeDefinition = y.ValueType.Resolve();

                if (!TypeReferenceComparer.Equals(xValueTypeDefinition, yValueTypeDefinition)) {
                    return false;
                }

                return true;
            }

            public override int GetHashCode([DisallowNull] ConstantDefinition obj) => throw new NotImplementedException();
        }
    }
}
