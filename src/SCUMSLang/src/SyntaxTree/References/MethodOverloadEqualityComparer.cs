using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.SyntaxTree.References
{
    /// <summary>
    /// Compares the equality of two methods by comparing their resolved method signature.
    /// </summary>
    public class MethodOverloadEqualityComparer : EqualityComparer<MethodReference>
    {
        public new readonly static MethodOverloadEqualityComparer Default = new MethodOverloadEqualityComparer();

        private readonly IReferenceResolver? referenceResolver;

        private MethodOverloadEqualityComparer() { }

        public MethodOverloadEqualityComparer(IReferenceResolver referenceResolver) =>
            this.referenceResolver = referenceResolver ?? throw new ArgumentNullException(nameof(referenceResolver));

        private IMember Resolve(TypeReference type)
        {
            if (referenceResolver is null) {
                return type.AsMember().Resolve();
            }

            return referenceResolver.Resolve(type).Value;
        }

        public override bool Equals(MethodReference? x, MethodReference? y)
        {
            // We don't want check reference equality but null equality.
            if (x is null && y is null) {
                return true;
            }

            if (x is null || y is null) {
                return false;
            }

            return x.Name == y.Name
                && x.GenericParameters.Select(paramter => Resolve(paramter.ParameterType)).SequenceEqual(
                    y.GenericParameters.Select(paramter => Resolve(paramter.ParameterType)))
                && x.Parameters.Select(paramter => Resolve(paramter.ParameterType)).SequenceEqual(
                    y.Parameters.Select(paramter => Resolve(paramter.ParameterType)));
        }

        public override int GetHashCode([DisallowNull] MethodReference obj) =>
            throw new NotImplementedException();
    }
}
