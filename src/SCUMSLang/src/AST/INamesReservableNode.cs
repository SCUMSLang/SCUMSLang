using System.Collections.Generic;

namespace SCUMSLang.AST
{
    public interface INamesReservableNode
    {
        bool HasReservedNames { get; }

        /// <summary>
        /// Own name is not included.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node> GetReservedNames();
    }
}
