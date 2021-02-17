using System.Collections.Generic;

namespace SCUMSLang.SyntaxTree
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
