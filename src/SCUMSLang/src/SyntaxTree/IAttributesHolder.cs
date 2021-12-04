using System.Collections.Generic;
using SCUMSLang.SyntaxTree.Definitions;

namespace SCUMSLang.SyntaxTree
{
    internal interface IAttributesHolder
    {
        IList<AttributeDefinition> Attributes { get; }
    }
}
