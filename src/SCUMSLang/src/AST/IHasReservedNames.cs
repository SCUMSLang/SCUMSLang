using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public interface IHasReservedNames
    {
        bool HasReservedNames { get; }

        /// <summary>
        /// Own name is not included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(string, Node)> GetReservedNames();
    }
}
