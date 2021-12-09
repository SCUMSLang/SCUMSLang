using System.Collections.Generic;
using SCUMSLang.SyntaxTree.References;

namespace SCUMSLang.SyntaxTree.Definitions
{
    public interface INestedTypesProvider
    {
        bool HasNestedTypes { get; }

        /// <summary>
        /// Own name is not included.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TypeReference> GetNestedTypes();
    }
}
