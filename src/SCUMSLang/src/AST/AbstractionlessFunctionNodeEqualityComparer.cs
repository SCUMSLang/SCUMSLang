using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCUMSLang.AST
{
    public class AbstractionlessFunctionNodeEqualityComparer : EqualityComparer<FunctionNode>
    {
        public new static AbstractionlessFunctionNodeEqualityComparer Default = new AbstractionlessFunctionNodeEqualityComparer();

        public override bool Equals([AllowNull] FunctionNode x, [AllowNull] FunctionNode y) =>
            ReferenceEquals(x, y) || !(x is null) && !(y is null)
            && Enumerable.SequenceEqual(x.GenericParameters, y.GenericParameters)
            && Enumerable.SequenceEqual(x.Parameters, y.Parameters);

        public override int GetHashCode([DisallowNull] FunctionNode obj) =>
            obj.GetHashCode();
    }
}
