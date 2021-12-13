using System;

namespace SCUMSLang.SyntaxTree.References
{
    public abstract class TypeSpecification : TypeReference
    {
        public override SyntaxTreeNodeType NodeType =>
            SyntaxTreeNodeType.TypeSpecification;

        public TypeReference ElementType { get; }

        protected TypeSpecification(TypeReference elementType)
            : base(elementType.Name) =>
            ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
    }
}
