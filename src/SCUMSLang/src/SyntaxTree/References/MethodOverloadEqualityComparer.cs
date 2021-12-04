using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree.References
{
    /// <summary>
    /// Compares the equality of two methods by comparing their resolved method signature.
    /// </summary>
    public class MethodOverloadEqualityComparer : EqualityComparer<MethodReference>
    {
        public new readonly static MethodOverloadEqualityComparer Default = new MethodOverloadEqualityComparer();

        private readonly ModuleDefinition? module;

        private MethodOverloadEqualityComparer() { }

        public MethodOverloadEqualityComparer(ModuleDefinition module) =>
            this.module = module ?? throw new ArgumentNullException(nameof(module));

        private TypeDefinition ResolveType(TypeReference type)
        {
            if (module is null) {
                return type.Resolve();
            }

            return module.Resolve(type);
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
                && x.GenericParameters.Select(paramter => ResolveType(paramter.ParameterType)).SequenceEqual(
                    y.GenericParameters.Select(paramter => ResolveType(paramter.ParameterType)))
                && x.Parameters.Select(paramter => ResolveType(paramter.ParameterType)).SequenceEqual(
                    y.Parameters.Select(paramter => ResolveType(paramter.ParameterType)));
        }

        public override int GetHashCode([DisallowNull] MethodReference obj) =>
            throw new NotImplementedException();
    }
}
