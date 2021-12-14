using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.Imports
{
    public class ImportOnlyPathEqualityComparer : EqualityComparer<Import>
    {
        public new static ImportOnlyPathEqualityComparer Default = new ImportOnlyPathEqualityComparer();

        public override bool Equals([AllowNull] Import x, [AllowNull] Import y) =>
            ReferenceEquals(x, y) || x is null || y is null || x.ImportPath.Equals(y.ImportPath);

        public override int GetHashCode([DisallowNull] Import obj) =>
            obj.ImportPath.GetHashCode();
    }
}
