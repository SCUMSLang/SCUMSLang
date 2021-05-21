using System.Collections.Generic;

namespace SCUMSLang.SyntaxTree
{
    internal interface IAttributesHolder
    {
        IList<AttributeDefinition> Attributes { get; }
    }
}
