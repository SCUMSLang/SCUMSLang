using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.SyntaxTree.References
{
    public class FieldReferenceEqualityComparer : EqualityComparer<FieldReference>
    {
        public new static FieldReferenceEqualityComparer Default = new FieldReferenceEqualityComparer();

        public TypeReferenceEqualityComparer TypeReferenceComparer { get; }

        public FieldReferenceEqualityComparer(TypeReferenceEqualityComparer? typeReferenceComparer = null) =>
            TypeReferenceComparer = typeReferenceComparer ?? TypeReferenceEqualityComparer.Default;

        public override bool Equals([AllowNull] FieldReference x, [AllowNull] FieldReference y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && x.Name == y.Name
            && x.IsStatic == y.IsStatic
            && TypeReferenceComparer.Equals(x.FieldType, y.FieldType)
            && TypeReferenceComparer.Equals(x.DeclaringType, y.DeclaringType);

        public override int GetHashCode([DisallowNull] FieldReference obj) =>
            throw new NotImplementedException();
    }
}
