using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.AST
{
    public class ConstantDefinitionEqualityComparer : EqualityComparer<ConstantDefinition>
    {
        public new static ConstantDefinitionEqualityComparer Default = new ConstantDefinitionEqualityComparer();

        public override bool Equals([AllowNull] ConstantDefinition x, [AllowNull] ConstantDefinition y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Equals(y);

        public override int GetHashCode([DisallowNull] ConstantDefinition obj) =>
            throw new System.NotImplementedException();

        public class OverloadComparer : EqualityComparer<ConstantDefinition>
        {
            public new static OverloadComparer Default = new OverloadComparer();

            public TypeReferenceEqualityComparer TypeReferenceComparer {
                get {
                    if (typeReferenceComparer is null) {
                        typeReferenceComparer = TypeReferenceEqualityComparer.Default;
                    }

                    return typeReferenceComparer;
                }

                set => typeReferenceComparer = value;
            }

            private TypeReferenceEqualityComparer? typeReferenceComparer;

            public override bool Equals([AllowNull] ConstantDefinition x, [AllowNull] ConstantDefinition y)
            {
                if (ReferenceEquals(x, y)) {
                    return true;
                }

                if (x is null || y is null) {
                    return false;
                }

                var xValueTypeDefinition = x.ValueType.ResolveNonAlias();
                var yValueTypeDefinition = y.ValueType.ResolveNonAlias();

                if (!TypeReferenceComparer.Equals(xValueTypeDefinition, yValueTypeDefinition)) {
                    return false;
                }

                if (Equals(x.Value, y.Value)) {
                    return true;
                } else if (x.Value is null || y.Value is null) {
                    return false;
                }

                if (xValueTypeDefinition.IsEnum && yValueTypeDefinition.IsEnum) {
                    var xEnumFieldName = xValueTypeDefinition.GetEnumerationName(x.Value);
                    var yEnumFieldName = yValueTypeDefinition.GetEnumerationName(y.Value);

                    return !(xEnumFieldName is null) && !(yEnumFieldName is null)
                        && Equals(xEnumFieldName, yEnumFieldName);
                }

                return false;
            }

            public override int GetHashCode([DisallowNull] ConstantDefinition obj) => throw new System.NotImplementedException();
        }
    }
}
