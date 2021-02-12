using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SCUMSLang.IO
{
    public class ImportEntryOnlyPathEqualityComparer : EqualityComparer<ImportEntry>
    {
        public new static ImportEntryOnlyPathEqualityComparer Default = new ImportEntryOnlyPathEqualityComparer();

        public override bool Equals([AllowNull] ImportEntry x, [AllowNull] ImportEntry y) =>
            ReferenceEquals(x, y) || x is null || y is null || x.ImportPath.Equals(y.ImportPath);

        public override int GetHashCode([DisallowNull] ImportEntry obj) =>
            obj.ImportPath.GetHashCode();
    }
}
