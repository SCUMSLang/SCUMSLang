using System.Collections.Generic;

namespace SCUMSLang.SyntaxTree.Definitions
{
    internal interface IAttributesHolder
    {
        IList<AttributeDefinition> Attributes { get; }
    }
}
